using System.Collections.Generic;
using System.Linq;

namespace AI.NeuralNetworks {
public class PyNeuralNetwork : NeuralNetwork {
	private readonly List<PyNeuralLayer> layers;

	public PyNeuralNetwork(uint[] topology) {
		this.topology = topology;
		layers = new List<PyNeuralLayer>();

		// Calculate overall weight count
		weightCount = 0;
		for (int i = 0; i < topology.Length - 1; i++)
			weightCount += (topology[i] + 1) * topology[i + 1];

		layers.Add(new PyNeuralLayer(topology.First()));
		for (int i = 1; i < topology.Length; i++)
			layers.Add(new PyNeuralLayer(topology[i], layers.Last()));
	}

	public override void setWeights(IEnumerator<double> parameters) {
		for (int l = 1; l < layers.Count; l++) {
			for (int i = 0; i < layers[l].values.Length; i++) {
				for (int j = 0; j < layers[l - 1].values.Length; j++) {
					layers[l].weights[i][j] = parameters.Current;
					parameters.MoveNext();
				}
			}

			for (int i = 0; i < layers[l].biases.Length; i++) {
				layers[l].biases[i] = parameters.Current;
				parameters.MoveNext();
			}
		}
	}

	public override double[] process(double[] inputs) {
		double[] inputValues = layers.First().values;
		for (int i = 0; i < inputValues.Length; i++)
			inputValues[i] = inputs[i];

		for (int i = 1; i < layers.Count; i++)
			layers[i].process();

		return layers.Last().values;
	}
}
}