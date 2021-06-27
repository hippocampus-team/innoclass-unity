using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

/// <summary>
/// Class representing one member of a population
/// </summary>
public class Genotype : IComparable<Genotype>, IEnumerable<double> {

	private static readonly Random randomizer = new Random();

	/// <summary>
	/// The current evaluation of this genotype.
	/// </summary>
	public float evaluation { get; set; }

	/// <summary>
	/// The current fitness (e.g, the evaluation of this genotype relative 
	/// to the average evaluation of the whole population) of this genotype.
	/// </summary>
	public float fitness { get; set; }

	// The vector of parameters of this genotype.
	private readonly double[] parameters;

	/// <summary>
	/// The amount of parameters stored in the parameter vector of this genotype.
	/// </summary>
	public int parameterCount => parameters?.Length ?? 0;

	// Overridden indexer for convenient parameter access.
	public double this[int index] {
		get => parameters[index];
		set => parameters[index] = value;
	}

	/// <summary>
	/// Instance of a new genotype with given parameter vector and initial fitness of 0.
	/// </summary>
	/// <param name="parameters">The parameter vector to initialise this genotype with.</param>
	public Genotype(double[] parameters) {
		this.parameters = parameters;
		fitness = 0;
	}

	#region IComparable

	/// <summary>
	/// Compares this genotype with another genotype depending on their fitness values.
	/// </summary>
	/// <param name="other">The genotype to compare this genotype with.</param>
	/// <returns>The result of comparing the two floating point values representing the genotypes fitness in reverse order.</returns>
	public int CompareTo(Genotype other) {
		return other.fitness.CompareTo(this.fitness); //in reverse order for larger fitness being first in list
	}

	#endregion

	#region IEnumerable

	/// <summary>
	/// Gets an Enumerator to iterate over all parameters of this genotype.
	/// </summary>
	public IEnumerator<double> GetEnumerator() {
		for (int i = 0; i < parameters.Length; i++)
			yield return parameters[i];
	}

	/// <summary>
	/// Gets an Enumerator to iterate over all parameters of this genotype.
	/// </summary>
	IEnumerator IEnumerable.GetEnumerator() {
		for (int i = 0; i < parameters.Length; i++)
			yield return parameters[i];
	}

	#endregion

	/// <summary>
	/// Sets the parameters of this genotype to random values in given range.
	/// </summary>
	/// <param name="minValue">The minimum inclusive value a parameter may be initialised with.</param>
	/// <param name="maxValue">The maximum exclusive value a parameter may be initialised with.</param>
	public void setRandomParameters(float minValue, float maxValue) {
		// Check arguments
		if (minValue > maxValue) throw new ArgumentException("Minimum value may not exceed maximum value.");

		// Generate random parameter vector
		float range = maxValue - minValue;
		for (int i = 0; i < parameters.Length; i++)
			parameters[i] = (float) (randomizer.NextDouble() * range + minValue);
	}

	/// <summary>
	/// Returns a copy of the parameter vector.
	/// </summary>
	public double[] getParameterCopy() {
		double[] copy = new double[parameterCount];
		for (int i = 0; i < parameterCount; i++)
			copy[i] = parameters[i];

		return copy;
	}

	/// <summary>
	/// Saves the parameters of this genotype to a file at given file path.
	/// </summary>
	/// <param name="filePath">The path of the file to save this genotype to.</param>
	/// <remarks>This method will override existing files or attempt to create new files, if the file at given file path does not exist.</remarks>
	public void saveToFile(string filePath) {
		StringBuilder builder = new StringBuilder();
		foreach (double parameter in parameters)
			builder.Append(((float) parameter).ToString(CultureInfo.InvariantCulture)).Append(";");

		builder.Remove(builder.Length - 1, 1);
		File.WriteAllText(filePath, builder.ToString());
	}

	#region Static Methods

	/// <summary>
	/// Generates a random genotype with parameters in given range.
	/// </summary>
	/// <param name="parameterCount">The amount of parameters the genotype consists of.</param>
	/// <param name="minValue">The minimum inclusive value a parameter may be initialised with.</param>
	/// <param name="maxValue">The maximum exclusive value a parameter may be initialised with.</param>
	/// <returns>A genotype with random parameter values</returns>
	public static Genotype generateRandom(uint parameterCount, float minValue, float maxValue) {
		// Check arguments
		if (parameterCount == 0) return new Genotype(new double[0]);

		Genotype randomGenotype = new Genotype(new double[parameterCount]);
		randomGenotype.setRandomParameters(minValue, maxValue);

		return randomGenotype;
	}

	/// <summary>
	/// Loads a genotype from a file with given file path.
	/// </summary>
	/// <param name="filePath">The path of the file to load the genotype from.</param>
	/// <returns>The genotype loaded from the file at given file path.</returns>
	public static Genotype loadFromFile(string filePath) {
		string data = File.ReadAllText(filePath);

		List<double> parameters = new List<double>();
		string[] paramStrings = data.Split(';');

		foreach (string parameter in paramStrings) {
			if (!double.TryParse(parameter, out double parsed))
				throw new ArgumentException("The file at given file path does not contain a valid genotype serialisation.");
			parameters.Add(parsed);
		}

		return new Genotype(parameters.ToArray());
	}

	#endregion

}