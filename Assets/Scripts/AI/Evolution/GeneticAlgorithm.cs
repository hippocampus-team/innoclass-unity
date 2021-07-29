using System;
using System.Collections.Generic;
using Game;
using Simulation;

namespace AI.Evolution {

/// <summary>
/// Class implementing a modified genetic algorithm
/// </summary>
public class GeneticAlgorithm {

	#region Default Parameters

	/// <summary>
	/// Default probability of a parameter being swapped during crossover.
	/// </summary>
	public const float defCrossSwapProb = 0.6f;

	/// <summary>
	/// Default probability of a parameter being mutated.
	/// </summary>
	public const float defMutationProb = 0.3f;

	/// <summary>
	/// Default amount by which parameters may be mutated.
	/// </summary>
	public const float defMutationAmount = 2.0f;

	/// <summary>
	/// Default percent of genotypes in a new population that are mutated.
	/// </summary>
	public const float defMutationPerc = 1.0f;

	#endregion

	#region Operator Delegates

	/// <summary>
	/// Method template for methods used to evaluate (or start the evluation process of) the current population.
	/// </summary>
	/// <param name="currentPopulation">The current population.</param>
	public delegate void EvaluationOperator(IEnumerable<Genotype> currentPopulation);

	/// <summary>
	/// Method template for methods used to calculate the fitness value of each genotype of the current population.
	/// </summary>
	/// <param name="currentPopulation"></param>
	public delegate void FitnessCalculation(IEnumerable<Genotype> currentPopulation);

	/// <summary>
	/// Method template for methods used to select genotypes of the current population and create the intermediate population.
	/// </summary>
	/// <param name="currentPopulation">The current population,</param>
	/// <returns>The intermediate population.</returns>
	public delegate List<Genotype> SelectionOperator(List<Genotype> currentPopulation);

	/// <summary>
	/// Method template for methods used to recombine the intermediate population to generate a new population.
	/// </summary>
	/// <param name="intermediatePopulation">The intermediate population.</param>
	/// <returns>The new population.</returns>
	public delegate List<Genotype> RecombinationOperator(List<Genotype> intermediatePopulation, uint newPopulationSize);

	/// <summary>
	/// Method template for methods used to mutate the new population.
	/// </summary>
	/// <param name="newPopulation">The mutated new population.</param>
	public delegate void MutationOperator(List<Genotype> newPopulation);

	/// <summary>
	/// Method template for method used to check whether any termination criterion has been met.
	/// </summary>
	/// <param name="currentPopulation">The current population.</param>
	/// <returns>Whether the algorithm shall be terminated.</returns>
	public delegate bool CheckTerminationCriterion(IEnumerable<Genotype> currentPopulation);

	#endregion

	#region Operator Methods

	/// <summary>
	/// Method used to evaluate (or start the evaluation process of) the current population.
	/// </summary>
	public EvaluationOperator evaluation = asyncEvaluation;

	/// <summary>
	/// Method used to calculate the fitness value of each genotype of the current population.
	/// </summary>
	public readonly FitnessCalculation fitnessCalculationMethod = defaultFitnessCalculation;

	/// <summary>
	/// Method used to select genotypes of the current population and create the intermediate population.
	/// </summary>
	public SelectionOperator selection = defaultSelectionOperator;

	/// <summary>
	/// Method used to recombine the intermediate population to generate a new population.
	/// </summary>
	public RecombinationOperator recombination = defaultRecombinationOperator;

	/// <summary>
	/// Method used to mutate the new population.
	/// </summary>
	public MutationOperator mutation = defaultMutationOperator;

	/// <summary>
	/// Method used to check whether any termination criterion has been met.
	/// </summary>
	public CheckTerminationCriterion terminationCriterion = null;

	#endregion

	private static readonly Random randomizer = new Random();

	private List<Genotype> currentPopulation;

	/// <summary>
	/// The amount of genotypes in a population.
	/// </summary>
	private uint populationSize { get; }

	/// <summary>
	/// The amount of generations that have already passed.
	/// </summary>
	public uint generationCount { get; private set; }

	/// <summary>
	/// Whether the current population shall be sorted before calling the termination criterion operator.
	/// </summary>
	private bool sortPopulation { get; }

	/// <summary>
	/// Event for when the algorithm is eventually terminated.
	/// </summary>
	public event Action<GeneticAlgorithm> algorithmTerminated;

	/// <summary>
	/// Event for when the algorithm has finished fitness calculation. Given parameter is the
	/// current population sorted by fitness if sorting is enabled (see <see cref="sortPopulation"/>).
	/// </summary>
	public event Action<List<Genotype>> fitnessCalculationFinished;

	public GeneticAlgorithm() {
		populationSize = ModelsManager.getInstance().desiredPopulationSize;
		if (GameStateManager.userControl) populationSize++;

		List<SimulationModel> activeModels = ModelsManager.getInstance().getActiveModels();
		currentPopulation = new List<Genotype>(activeModels.Count);
		foreach (SimulationModel model in activeModels)
			currentPopulation.Add(model.genotype);

		if (currentPopulation.Count == 1)
			currentPopulation.Add(currentPopulation[0]);

		generationCount = 1;
		sortPopulation = true;
	}

	public void start() {
		List<Genotype> initialPopulation = recombination(currentPopulation, populationSize);
		mutation(initialPopulation);
		currentPopulation = initialPopulation;
		evaluation(currentPopulation);
	}

	public void startWithoutInitialization() {
		evaluation(currentPopulation);
	}

	public void evaluationFinished() {
		// Calculate fitness from evaluation
		fitnessCalculationMethod(currentPopulation);

		// Sort population if flag was set
		if (sortPopulation) currentPopulation.Sort();

		// Fire fitness calculation finished event
		fitnessCalculationFinished?.Invoke(currentPopulation);

		// Check termination criterion
		if (terminationCriterion != null && terminationCriterion(currentPopulation)) {
			terminate();
			return;
		}

		// Apply Selection
		List<Genotype> intermediatePopulation = selection(currentPopulation);

		// Apply Recombination
		List<Genotype> newPopulation = recombination(intermediatePopulation, populationSize);

		// Apply Mutation
		mutation(newPopulation);


		// Set current population to newly generated one and start evaluation again
		currentPopulation = newPopulation;
		generationCount++;

		evaluation(currentPopulation);
	}

	private void terminate() {
		algorithmTerminated?.Invoke(this);
	}

	private static void asyncEvaluation(IEnumerable<Genotype> currentPopulation) {
		//At this point the async evaluation should be started and after it is finished EvaluationFinished should be called
	}

	/// <summary>
	/// Calculates the fitness of each genotype by the formula: fitness = evaluation / averageEvaluation.
	/// </summary>
	/// <param name="currentPopulation">The current population.</param>
	private static void defaultFitnessCalculation(IEnumerable<Genotype> currentPopulation) {
		//First calculate average evaluation of whole population
		uint populationSize = 0;
		float overallEvaluation = 0;
		foreach (Genotype genotype in currentPopulation) {
			overallEvaluation += genotype.evaluation;
			populationSize++;
		}

		float averageEvaluation = overallEvaluation / populationSize;

		//Now assign fitness with formula fitness = evaluation / averageEvaluation
		foreach (Genotype genotype in currentPopulation)
			genotype.fitness = genotype.evaluation / averageEvaluation;
	}

	/// <summary>
	/// Only selects the best three genotypes of the current population and copies them to the intermediate population.
	/// </summary>
	/// <param name="currentPopulation">The current population.</param>
	/// <returns>The intermediate population.</returns>
	public static List<Genotype> defaultSelectionOperator(List<Genotype> currentPopulation) {
		return new List<Genotype> {
			currentPopulation[0],
			currentPopulation[1],
			currentPopulation[2]
		};
	}

	/// <summary>
	/// Simply crosses the first with the second genotype of the intermediate population until the new 
	/// population is of desired size.
	/// </summary>
	/// <param name="intermediatePopulation">The intermediate population that was created from the selection process.</param>
	/// <returns>The new population.</returns>
	private static List<Genotype> defaultRecombinationOperator(List<Genotype> intermediatePopulation, uint newPopulationSize) {
		if (intermediatePopulation.Count < 2)
			throw new ArgumentException("Intermediate population size must be greater than 2 for this operator.");

		List<Genotype> newPopulation = new List<Genotype>();
		while (newPopulation.Count < newPopulationSize) {
			completeCrossover(intermediatePopulation[0], intermediatePopulation[1], defCrossSwapProb, out Genotype offspring1,
							  out Genotype offspring2);

			newPopulation.Add(offspring1);
			if (newPopulation.Count < newPopulationSize)
				newPopulation.Add(offspring2);
		}

		return newPopulation;
	}

	/// <summary>
	/// Simply mutates each genotype with the default mutation probability and amount.
	/// </summary>
	/// <param name="newPopulation">The mutated new population.</param>
	private static void defaultMutationOperator(List<Genotype> newPopulation) {
		foreach (Genotype genotype in newPopulation)
			if (randomizer.NextDouble() < defMutationPerc)
				mutateGenotype(genotype, defMutationProb, defMutationAmount);
	}

	public static void completeCrossover(Genotype parent1, Genotype parent2, float swapChance, out Genotype offspring1,
										 out Genotype offspring2) {
		// Initialise new parameter vectors
		int parameterCount = parent1.parameterCount;
		double[] off1Parameters = new double[parameterCount];
		double[] off2Parameters = new double[parameterCount];

		// Iterate over all parameters randomly swapping
		for (int i = 0; i < parameterCount; i++)
			if (randomizer.Next() < swapChance) {
				// Swap parameters
				off1Parameters[i] = parent2[i];
				off2Parameters[i] = parent1[i];
			} else {
				// Don't swap parameters
				off1Parameters[i] = parent1[i];
				off2Parameters[i] = parent2[i];
			}

		offspring1 = new Genotype(off1Parameters);
		offspring2 = new Genotype(off2Parameters);
	}

	/// <summary>
	/// Mutates the given genotype by adding a random value in range [-mutationAmount, mutationAmount] to each parameter with a probability of mutationProb.
	/// </summary>
	/// <param name="genotype">The genotype to be mutated.</param>
	/// <param name="mutationProb">The probability of a parameter being mutated.</param>
	/// <param name="mutationAmount">A parameter may be mutated by an amount in range [-mutationAmount, mutationAmount].</param>
	public static void mutateGenotype(Genotype genotype, float mutationProb, float mutationAmount) {
		for (int i = 0; i < genotype.parameterCount; i++)
			if (randomizer.NextDouble() < mutationProb) // Mutate by random amount in range [-mutationAmount, mutationAmoun]
				genotype[i] += (float) (randomizer.NextDouble() * (mutationAmount * 2) - mutationAmount);
	}

}
}