using System;
using System.Collections.Generic;
using Game.Car;
using Game.Track;
using Simulation;
using UnityEngine;
using Random = System.Random;

namespace AI.Evolution {

/// <summary>
/// Singleton class for managing the evolutionary processes.
/// </summary>
public class EvolutionManager : MonoBehaviour {
	private static readonly Random randomizer = new Random();

	public static EvolutionManager instance { get; private set; }

	// After how many generations should the genetic algorithm be restart (0 for never), to be set in Unity Editor
	[SerializeField] private int restartAfter = 100;

	// Whether to use elitist selection or remainder stochastic sampling, to be set in Unity Editor
	[SerializeField] private bool elitistSelection;

	[SerializeField] private bool loadFromFile;

	// The current population of agents.
	private readonly List<Agent> agents = new List<Agent>();

	/// <summary>
	/// The amount of agents that are currently alive.
	/// </summary>
	private int agentsAliveCount { get; set; }

	/// <summary>
	/// Event for when all agents have died.
	/// </summary>
	public event Action allAgentsDied;

	private GeneticAlgorithm geneticAlgorithm;

	/// <summary>
	/// The age of the current generation.
	/// </summary>
	public uint generationCount => geneticAlgorithm.generationCount;

	private void Awake() {
		if (instance != null) Debug.LogError("More than one EvolutionManager in the Scene.");
		else instance = this;
	}

	/// <summary>
	/// Starts the evolutionary process.
	/// </summary>
	public void startEvolution() {
		geneticAlgorithm = new GeneticAlgorithm { evaluation = startEvaluation };

		if (elitistSelection) {
			// Second configuration
			geneticAlgorithm.selection = GeneticAlgorithm.defaultSelectionOperator;
			geneticAlgorithm.recombination = randomRecombination;
			geneticAlgorithm.mutation = mutateAllButBestTwo;
		} else {
			// First configuration
			geneticAlgorithm.selection = remainderStochasticSampling;
			geneticAlgorithm.recombination = randomRecombination;
			geneticAlgorithm.mutation = mutateAllButBestTwo;
		}

		allAgentsDied += geneticAlgorithm.evaluationFinished;

		geneticAlgorithm.fitnessCalculationFinished += checkForTrackFinished;

		// Restart logic
		if (restartAfter > 0) {
			geneticAlgorithm.terminationCriterion += checkGenerationTermination;
			geneticAlgorithm.algorithmTerminated += OnGATermination;
		}

		if (loadFromFile) geneticAlgorithm.startWithoutInitialization();
		else geneticAlgorithm.start();
	}

	private void checkForTrackFinished(IEnumerable<Genotype> currentPopulation) {
		foreach (Genotype genotype in currentPopulation)
			if (genotype.evaluation >= 0.8)
				ModelsManager.getInstance().pushRandomActiveModelUpdate(genotype);
			else return; // List should be sorted, so we can exit here
	}

	// Checks whether the termination criterion of generation count was met.
	private bool checkGenerationTermination(IEnumerable<Genotype> currentPopulation) {
		return geneticAlgorithm.generationCount >= restartAfter;
	}

	// To be called when the genetic algorithm was terminated
	private void OnGATermination(GeneticAlgorithm ga) {
		allAgentsDied -= ga.evaluationFinished;
		restartAlgorithm(5.0f);
	}

	// Restarts the algorithm after a specific wait time second wait
	private void restartAlgorithm(float wait) {
		Invoke(nameof(startEvolution), wait);
	}

	// Starts the evaluation by first creating new agents from the current population and then restarting the track manager.
	private void startEvaluation(IEnumerable<Genotype> currentPopulation) {
		// Create new agents from currentPopulation
		agents.Clear();
		agentsAliveCount = 0;

		foreach (Genotype genotype in currentPopulation)
			agents.Add(new Agent(genotype, ModelsManager.getInstance().topology));

		TrackManager.instance.setCarAmount(agents.Count);
		IEnumerator<CarController> carsEnum = TrackManager.instance.getCarEnumerator();
		foreach (Agent agent in agents) {
			if (!carsEnum.MoveNext()) {
				Debug.LogError("Cars enum ended before agents.");
				break;
			}

			carsEnum.Current.agent = agent;
			agentsAliveCount++;
			agent.agentDied += OnAgentDied;
		}

		TrackManager.instance.restart();
	}

	// Callback for when an agent died.
	private void OnAgentDied(Agent agent) {
		agentsAliveCount--;

		if (agentsAliveCount == 0 && allAgentsDied != null)
			allAgentsDied();
	}

	#region GA Operators

	// Selection operator for the genetic algorithm, using a method called remainder stochastic sampling.
	private static List<Genotype> remainderStochasticSampling(List<Genotype> currentPopulation) {
		List<Genotype> intermediatePopulation = new List<Genotype>();

		// Put integer portion of genotypes into intermediatePopulation
		// Assumes that currentPopulation is already sorted
		foreach (Genotype genotype in currentPopulation) {
			if (genotype.fitness < 1)
				break;
			for (int i = 0; i < (int) genotype.fitness; i++)
				intermediatePopulation.Add(new Genotype(genotype.getParameterCopy()));
		}

		// Put remainder portion of genotypes into intermediatePopulation
		foreach (Genotype genotype in currentPopulation) {
			float remainder = genotype.fitness - (int) genotype.fitness;
			if (randomizer.NextDouble() < remainder)
				intermediatePopulation.Add(new Genotype(genotype.getParameterCopy()));
		}

		return intermediatePopulation;
	}

	// Recombination operator for the genetic algorithm, recombining random genotypes of the intermediate population
	private static List<Genotype> randomRecombination(List<Genotype> intermediatePopulation, uint newPopulationSize) {
		// Check arguments
		if (intermediatePopulation.Count < 2)
			throw new ArgumentException("The intermediate population has to be at least of size 2 for this operator.");

		List<Genotype> newPopulation = new List<Genotype> {
			intermediatePopulation[0],
			intermediatePopulation[1]
		};
		// Always add best two (unmodified)


		while (newPopulation.Count < newPopulationSize) {
			// Get two random indices that are not the same
			int randomIndex1 = randomizer.Next(0, intermediatePopulation.Count), randomIndex2;
			do {
				randomIndex2 = randomizer.Next(0, intermediatePopulation.Count);
			} while (randomIndex2 == randomIndex1);

			GeneticAlgorithm.completeCrossover(intermediatePopulation[randomIndex1], intermediatePopulation[randomIndex2],
											   GeneticAlgorithm.defCrossSwapProb, out Genotype offspring1, out Genotype offspring2);

			newPopulation.Add(offspring1);
			if (newPopulation.Count < newPopulationSize)
				newPopulation.Add(offspring2);
		}

		return newPopulation;
	}

	// Mutates all members of the new population with the default probability, while leaving the first 2 genotypes in the list untouched.
	private static void mutateAllButBestTwo(List<Genotype> newPopulation) {
		for (int i = 2; i < newPopulation.Count; i++)
			if (randomizer.NextDouble() < GeneticAlgorithm.defMutationPerc)
				GeneticAlgorithm.mutateGenotype(newPopulation[i], GeneticAlgorithm.defMutationProb,
												GeneticAlgorithm.defMutationAmount);
	}

	// Mutates all members of the new population with the default parameters
	private static void mutateAll(IEnumerable<Genotype> newPopulation) {
		foreach (Genotype genotype in newPopulation)
			if (randomizer.NextDouble() < GeneticAlgorithm.defMutationPerc)
				GeneticAlgorithm.mutateGenotype(genotype, GeneticAlgorithm.defMutationProb,
												GeneticAlgorithm.defMutationAmount);
	}

	#endregion
}
}