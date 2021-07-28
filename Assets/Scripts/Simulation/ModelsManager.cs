using System.Collections.Generic;
using System.IO;
using System.Linq;
using AI.Evolution;
using General;
using UnityEngine;

namespace Simulation {
public class ModelsManager {
	private static readonly string[] modelsNames = { "Alpha", "Beta" };
	private static readonly uint[] defaultTopology = { 5, 4, 4, 2 };
	private const uint defaultPopulationSize = 24;

	public readonly List<SimulationModel> models;
	public readonly uint[] topology;
	public readonly uint desiredPopulationSize;

	private static ModelsManager instance;

	private ModelsManager() {
		models = new List<SimulationModel>();
		foreach (string name in modelsNames)
			models.Add(doesModelSaveExist(name)
						   ? loadModelFromName(name)
						   : SimulationModel.generateFromTopology(name, defaultTopology));

		topology = doesTopologySaveExist() ? loadTopologyFromSave() : defaultTopology;
		desiredPopulationSize = defaultPopulationSize;

		saveEverything();
	}

	public List<SimulationModel> getActiveModels() {
		return models.ToList();
	}

	public void regenerateModels() {
		models.Clear();
		foreach (string name in modelsNames)
			models.Add(SimulationModel.generateFromTopology(name, defaultTopology));
	}

	public bool isNumberOfActiveModelsValid() {
		int numberOfActiveModels = getActiveModels().Count;
		return numberOfActiveModels == 1 || numberOfActiveModels == 2;
	}

	public void pushRandomActiveModelUpdate(Genotype newGenotype) {
		List<SimulationModel> activeModels = getActiveModels();
		SimulationModel randomModel = activeModels[Random.Range(0, activeModels.Count)];
		randomModel.genotype = newGenotype;
		randomModel.saveToFile(getPathToModelSaveFromName(randomModel.name));
	}

	public void saveEverything() {
		foreach (SimulationModel model in models)
			model.saveToFile(getPathToModelSaveFromName(model.name));

		saveTopology();
	}

	public void saveTopology() {
		File.WriteAllText(Paths.topologySavePath, getRawTopology());
	}

	private string getRawTopology() {
		string raw = "";

		for (int i = 0; i < topology.Length - 1; i++)
			raw += topology[i] + ";";
		raw += topology.Last();

		return raw;
	}

	private static bool doesModelSaveExist(string name) {
		return File.Exists(getPathToModelSaveFromName(name));
	}

	private static bool doesTopologySaveExist() {
		return File.Exists(Paths.topologySavePath);
	}

	private static SimulationModel loadModelFromName(string name) {
		return SimulationModel.loadFromFile(getPathToModelSaveFromName(name), name);
	}

	private static uint[] loadTopologyFromSave() {
		return parseRawTopology(File.ReadAllText(Paths.topologySavePath));
	}

	private static uint[] parseRawTopology(string raw) {
		return raw.Split(';').Select(uint.Parse).ToArray();
	}

	private static string getPathToModelSaveFromName(string name) {
		return Paths.modelsFolderPath + "/" + name + ".save";
	}

	public static ModelsManager getInstance() {
		return instance ??= new ModelsManager();
	}
}
}