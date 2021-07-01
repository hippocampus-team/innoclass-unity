using System;
using System.Collections.Generic;

namespace AI.NeuralNetworks {
/// <summary>
/// Class representing a fully connected feedforward neural network.
/// </summary>
public class CsNeuralNetwork : NeuralNetwork {
	/// <summary>
	/// The individual neural layers of this network.
	/// </summary>
	private NeuralLayer[] layers { get; }

	/// <summary>
	/// Initialises a new fully connected feedforward neural network with given topology.
	/// </summary>
	/// <param name="topology">An array of unsigned integers representing the node count of each layer from input to output layer.</param>
	public CsNeuralNetwork(params uint[] topology) {
		this.topology = topology;

		// Calculate overall weight count
		weightCount = 0;
		for (int i = 0; i < topology.Length - 1; i++)
			weightCount += (topology[i] + 1) * topology[i + 1]; // + 1 for bias node

		// Initialise layers
		layers = new NeuralLayer[topology.Length - 1];
		for (int i = 0; i < layers.Length; i++)
			layers[i] = new NeuralLayer(topology[i], topology[i + 1]);
	}

	public override void setWeights(IEnumerator<double> parameters) {
		foreach (NeuralLayer layer in layers) {
			// Loop over all nodes of current layer
			for (int i = 0; i < layer.weights.GetLength(0); i++) {
				// Loop over all nodes of next layer
				for (int j = 0; j < layer.weights.GetLength(1); j++) {
					layer.weights[i, j] = parameters.Current;
					parameters.MoveNext();
				}
			}
		}
	}

	/// <summary>
	/// Processes the given inputs using the current network's weights.
	/// </summary>
	/// <param name="inputs">The inputs to be processed.</param>
	/// <returns>The calculated outputs.</returns>
	public override double[] process(double[] inputs) {
		// Check arguments
		if (inputs.Length != layers[0].neuronCount)
			throw new ArgumentException("Given inputs do not match network input amount.");

		// Process inputs by propagating values through all layers
		double[] outputs = inputs;
		foreach (NeuralLayer layer in layers)
			outputs = layer.processInputs(outputs);

		return outputs;

	}

	/// <summary>
	/// Sets the weights of this network to random values in given range.
	/// </summary>
	/// <param name="minValue">The minimum value a weight may be set to.</param>
	/// <param name="maxValue">The maximum value a weight may be set to.</param>
	public void setRandomWeights(double minValue, double maxValue) {
		if (layers == null) return;
		foreach (NeuralLayer layer in layers)
			layer.setRandomWeights(minValue, maxValue);
	}

	/// <summary>
	/// Returns a new NeuralNetwork instance with the same topology and 
	/// activation functions, but the weights set to their default value.
	/// </summary>
	public CsNeuralNetwork getTopologyCopy() {
		CsNeuralNetwork copy = new CsNeuralNetwork(topology);
		for (int i = 0; i < layers.Length; i++)
			copy.layers[i].neuronActivationFunction = layers[i].neuronActivationFunction;

		return copy;
	}

	/// <summary>
	/// Copies this NeuralNetwork including its topology and weights.
	/// </summary>
	/// <returns>A deep copy of this NeuralNetwork</returns>
	public CsNeuralNetwork deepCopy() {
		CsNeuralNetwork newNet = new CsNeuralNetwork(topology);
		for (int i = 0; i < layers.Length; i++)
			newNet.layers[i] = layers[i].deepCopy();

		return newNet;
	}

	/// <summary>
	/// Returns a string representing this network in layer order.
	/// </summary>
	public override string ToString() {
		string output = "";

		for (int i = 0; i < layers.Length; i++)
			output += "Layer " + i + ":\n" + layers[i];

		return output;
	}
}
}