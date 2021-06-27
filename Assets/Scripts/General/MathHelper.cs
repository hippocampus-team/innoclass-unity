using System;

namespace General {
/// <summary>
/// Static class for different Math operations and constants.
/// </summary>
public static class MathHelper {

	/// <summary>
	/// The standard sigmoid function.
	/// </summary>
	/// <param name="xValue">The input value.</param>
	/// <returns>The calculated output.</returns>
	public static double sigmoidFunction(double xValue) {
		if (xValue > 10) return 1.0;
		else if (xValue < -10) return 0.0;
		else return 1.0 / (1.0 + Math.Exp(-xValue));
	}

	/// <summary>
	/// The standard TanH function.
	/// </summary>
	/// <param name="xValue">The input value.</param>
	/// <returns>The calculated output.</returns>
	public static double tanHFunction(double xValue) {
		if (xValue > 10) return 1.0;
		else if (xValue < -10) return -1.0;
		else return Math.Tanh(xValue);
	}

	/// <summary>
	/// The SoftSign function as proposed by Xavier Glorot and Yoshua Bengio (2010): 
	/// "Understanding the difficulty of training deep feedforward neural networks".
	/// </summary>
	/// <param name="xValue">The input value.</param>
	/// <returns>The calculated output.</returns>
	public static double softSignFunction(double xValue) {
		return xValue / (1 + Math.Abs(xValue));
	}

}
}