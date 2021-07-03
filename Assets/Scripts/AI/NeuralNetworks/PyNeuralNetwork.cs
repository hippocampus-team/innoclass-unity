using System.Collections.Generic;
using System.Linq;
using IronPython.Runtime;

namespace AI.NeuralNetworks {
public class PyNeuralNetwork : NeuralNetwork {
	private readonly dynamic nn;

	public PyNeuralNetwork(uint[] topology) {
		this.topology = topology;

		// Calculate overall weight count
		weightCount = 0;
		for (int i = 0; i < topology.Length - 1; i++)
			weightCount += topology[i] * topology[i + 1];

		dynamic py = PythonExecutor.getInstance().getPythonEnv();
		nn = py.NeuralNetwork(topology);
	}

	public override void setWeights(IEnumerator<double> parameters) {
		for (int l = 0; l < nn.network.Count - 1; l++) {
			uint layerParams = nn.network[l].size * nn.network[l + 1].size;
			double[] layerWeights = new double[layerParams];
			for (int i = 0; i < layerParams; i++) {
				layerWeights[i] = parameters.Current;
				parameters.MoveNext();
			}

			nn.network[l].set_weights(layerWeights);
		}
	}

	public override double[] process(double[] inputs) {
		return ((List) nn.calculate(inputs)).Cast<double>().ToArray();
	}
}
}