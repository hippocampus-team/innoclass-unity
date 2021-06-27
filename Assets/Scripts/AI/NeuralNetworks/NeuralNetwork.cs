using System.Collections.Generic;

namespace AI.NeuralNetworks {
public abstract class NeuralNetwork {
	/// <summary>
	/// An array of unsigned integers representing the node count 
	/// of each layer of the network from input to output layer.
	/// </summary>
	protected uint[] topology { get; set; }

	/// <summary>
	/// The amount of overall weights of the connections of this network.
	/// </summary>
	public int weightCount { get; protected set; }

	public abstract void setWeights(IEnumerator<double> parameters);
	public abstract double[] process(double[] inputs);
}
}