using System.Globalization;
using AI.Evolution;
using Game.Car;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
/// <summary>
/// Class for controlling the various ui elements of the simulation
/// </summary>
public class UISimulationController : MonoBehaviour {
	[SerializeField] private Text[] inputTexts;
	[SerializeField] private Text evaluation;
	[SerializeField] private Text generationCount;

	[HideInInspector] public CarController target;

	private void Update() {
		if (target == null) return;

		//Display controls
		if (target.currentControlInputs != null)
			for (int i = 0; i < inputTexts.Length; i++)
				inputTexts[i].text = target.currentControlInputs[i].ToString(CultureInfo.InvariantCulture);

		//Display evaluation and generation count
		evaluation.text = target.agent.genotype.evaluation.ToString(CultureInfo.InvariantCulture);
		generationCount.text = EvolutionManager.instance.generationCount.ToString();
	}

	/// <summary>
	/// Starts to display the gui elements.
	/// </summary>
	public void show() {
		gameObject.SetActive(true);
	}

	/// <summary>
	/// Stops displaying the gui elements.
	/// </summary>
	public void hide() {
		gameObject.SetActive(false);
	}
}
}