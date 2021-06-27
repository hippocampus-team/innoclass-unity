using General;
using UnityEngine;

namespace UI {
/// <summary>
/// Class for controlling the overall GUI.
/// </summary>
public class UIController : MonoBehaviour {
	private UISimulationController simulationUI;

	private void Awake() {
		simulationUI = GetComponentInChildren<UISimulationController>(true);
		simulationUI.show();
	}

	private void Start() {
		GameStateManager.instance.uiController = this;
	}

	/// <summary>
	/// Sets the CarController from which to get the data from to be displayed.
	/// </summary>
	/// <param name="target">The CarController to display the data of.</param>
	public void setDisplayTarget(CarController target) {
		simulationUI.target = target;
	}

}
}