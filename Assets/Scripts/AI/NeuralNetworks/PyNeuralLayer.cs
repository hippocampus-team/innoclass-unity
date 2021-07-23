namespace AI.NeuralNetworks {
public class PyNeuralLayer {
	public readonly double[] values;
	public readonly double[][] weights;
	public readonly double[] biases;
	
	private readonly PyNeuralLayer previousLayer;

	public PyNeuralLayer(uint length, PyNeuralLayer previousLayer) : this(length) {
		this.previousLayer = previousLayer;
		biases = new double[length];
		weights = new double[length][];
		for (int i = 0; i < weights.Length; i++)
			weights[i] = new double[previousLayer.values.Length];
	}
	
	public PyNeuralLayer(uint length) {
		values = new double[length];
	}
	
	public void process() {
		for (int i = 0; i < values.Length; i++) {
			values[i] = PythonExecutor.getInstance().getPythonEnv()
				.calculate(previousLayer.values, weights[i], biases[i]);
		}
	}
}
}