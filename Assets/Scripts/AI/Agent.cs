using System;
using System.Collections.Generic;
using AI.NeuralNetworks;

namespace AI {
/// <summary>
/// Class that combines a genotype and a feedforward neural network (FNN).
/// </summary>
public class Agent : IComparable<Agent> {

	/// <summary>
	/// The underlying genotype of this agent.
	/// </summary>
	public Genotype genotype { get; }

	/// <summary>
	/// The feedforward neural network which was constructed from the genotype of this agent.
	/// </summary>
	private NeuralNetwork nn { get; }

	private bool isAlive;

	/// <summary>
	/// Whether this agent is currently alive (actively participating in the simulation).
	/// </summary>
	private bool IsAlive {
		set {
			if (isAlive == value) return;
			isAlive = value;
			if (!isAlive && agentDied != null) agentDied(this);
		}
	}

	/// <summary>
	/// Event for when the agent died (stopped participating in the simulation).
	/// </summary>
	public event Action<Agent> agentDied;

	/// <summary>
	/// Initialises a new agent from given genotype, constructing a new feedfoward neural network from
	/// the parameters of the genotype.
	/// </summary>
	/// <param name="genotype">The genotype to initialise this agent from.</param>
	/// <param name="topology">The topology of the feedforward neural network to be constructed from given genotype.</param>
	public Agent(Genotype genotype, params uint[] topology) {
		IsAlive = false;
		this.genotype = genotype;
		nn = new PyNeuralNetwork(topology);

		// Check if topology is valid
		if (nn.weightCount != genotype.parameterCount)
			throw new ArgumentException(
				"The given genotype's parameter count must match the neural network topology's weight count.");

		// Construct FNN from genotype
		IEnumerator<double> parameters = genotype.GetEnumerator();
		nn.setWeights(parameters);
	}

	public double[] process(double[] input) {
		return nn.process(input);
	}

	/// <summary>
	/// Resets this agent to be alive again.
	/// </summary>
	public void reset() {
		genotype.evaluation = 0;
		genotype.fitness = 0;
		IsAlive = true;
	}

	/// <summary>
	/// Kills this agent (sets IsAlive to false).
	/// </summary>
	public void kill() {
		IsAlive = false;
	}

	#region IComparable

	/// <summary>
	/// Compares this agent to another agent, by comparing their underlying genotypes.
	/// </summary>
	/// <param name="other">The agent to compare this agent to.</param>
	/// <returns>The result of comparing the underlying genotypes of this agent and the given agent.</returns>
	public int CompareTo(Agent other) {
		return genotype.CompareTo(other.genotype);
	}

	#endregion

}
}