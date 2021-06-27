using System;
using General;

namespace AI.NeuralNetworks {
/// <summary>
/// Class representing a single layer of a fully connected feedforward neural network.
/// </summary>
public class NeuralLayer {
	private static readonly Random randomizer = new Random();

	/// <summary>
	/// Delegate representing the activation function of an artificial neuron.
	/// </summary>
	/// <param name="xValue">The input value of the function.</param>
	/// <returns>The calculated output value of the function.</returns>
	public delegate double ActivationFunction(double xValue);

	/// <summary>
	/// The activation function used by the neurons of this layer.
	/// </summary>
	/// <remarks>The default activation function is the sigmoid function (see <see cref="MathHelper.sigmoidFunction"/>).</remarks>
	public ActivationFunction neuronActivationFunction = MathHelper.softSignFunction;

	/// <summary>
	/// The amount of neurons in this layer.
	/// </summary>
	public uint neuronCount { get; }

	/// <summary>
	/// The amount of neurons this layer is connected to, i.e., the amount of neurons of the next layer.
	/// </summary>
	private uint outputCount { get; }

	/// <summary>
	/// The weights of the connections of this layer to the next layer.
	/// E.g., weight [i, j] is the weight of the connection from the i-th weight
	/// of this layer to the j-th weight of the next layer.
	/// </summary>
	public double[,] weights { get; private set; }

	/// <summary>
	/// Initialises a new neural layer for a fully connected feedforward neural network with given 
	/// amount of node and with connections to the given amount of nodes of the next layer.
	/// </summary>
	/// <param name="nodeCount">The amount of nodes in this layer.</param>
	/// <param name="outputCount">The amount of nodes in the next layer.</param>
	/// <remarks>All weights of the connections from this layer to the next are initialised with the default double value.</remarks>
	public NeuralLayer(uint nodeCount, uint outputCount) {
		neuronCount = nodeCount;
		this.outputCount = outputCount;
		weights = new double[nodeCount + 1, outputCount]; // + 1 for bias node
	}

	/// <summary>
	/// Sets the weights of this layer to the given values.
	/// </summary>
	/// <param name="weights">
	/// The values to set the weights of the connections from this layer to the next to.
	/// </param>
	/// <remarks>
	/// The values are ordered in neuron order. E.g., in a layer with two neurons with a next layer of three neurons 
	/// the values [0-2] are the weights from neuron 0 of this layer to neurons 0-2 of the next layer respectively and 
	/// the values [3-5] are the weights from neuron 1 of this layer to neurons 0-2 of the next layer respectively.
	/// </remarks>
	public void setWeights(double[] weights) {
		// Check arguments
		if (weights.Length != this.weights.Length)
			throw new ArgumentException("Input weights do not match layer weight count.");

		// Copy weights from given value array
		int k = 0;
		for (int i = 0; i < this.weights.GetLength(0); i++)
		for (int j = 0; j < this.weights.GetLength(1); j++)
			this.weights[i, j] = weights[k++];
	}

	/// <summary>
	/// Processes the given inputs using the current weights to the next layer.
	/// </summary>
	/// <param name="inputs">The inputs to be processed.</param>
	/// <returns>The calculated outputs.</returns>
	public double[] processInputs(double[] inputs) {
		// Check arguments
		if (inputs.Length != neuronCount)
			throw new ArgumentException("Given xValues do not match layer input count.");

		// Calculate sum for each neuron from weighted inputs and bias
		double[] sums = new double[outputCount];
		// Add bias (always on) neuron to inputs
		double[] biasedInputs = new double[neuronCount + 1];
		inputs.CopyTo(biasedInputs, 0);
		biasedInputs[inputs.Length] = 1.0;

		for (int j = 0; j < weights.GetLength(1); j++)
		for (int i = 0; i < weights.GetLength(0); i++)
			sums[j] += biasedInputs[i] * weights[i, j];

		// Apply activation function to sum
		for (int i = 0; i < sums.Length; i++)
			sums[i] = neuronActivationFunction(sums[i]);

		return sums;
	}

	/// <summary>
	/// Copies this NeuralLayer including its weights.
	/// </summary>
	/// <returns>A deep copy of this NeuralLayer</returns>
	public NeuralLayer deepCopy() {
		// Copy weights
		double[,] copiedWeights = new double[weights.GetLength(0), weights.GetLength(1)];

		for (int x = 0; x < weights.GetLength(0); x++)
		for (int y = 0; y < weights.GetLength(1); y++)
			copiedWeights[x, y] = weights[x, y];

		// Create copy
		NeuralLayer newLayer = new NeuralLayer(neuronCount, outputCount) {
			weights = copiedWeights,
			neuronActivationFunction = neuronActivationFunction
		};

		return newLayer;
	}

	/// <summary>
	/// Sets the weights of the connection from this layer to the next to random values in given range.
	/// </summary>
	/// <param name="minValue">The minimum value a weight may be set to.</param>
	/// <param name="maxValue">The maximum value a weight may be set to.</param>
	public void setRandomWeights(double minValue, double maxValue) {
		double range = Math.Abs(minValue - maxValue);
		for (int i = 0; i < weights.GetLength(0); i++)
		for (int j = 0; j < weights.GetLength(1); j++)
			weights[i, j] = minValue + randomizer.NextDouble() * range;
	}

	/// <summary>
	/// Returns a string representing this layer's connection weights.
	/// </summary>
	public override string ToString() {
		string output = "";

		for (int x = 0; x < weights.GetLength(0); x++) {
			for (int y = 0; y < weights.GetLength(1); y++)
				output += "[" + x + "," + y + "]: " + weights[x, y];

			output += "\n";
		}

		return output;
	}

}
}