using AI.Evolution;
using Cinemachine;
using Simulation;
using UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace General {
/// <summary>
/// Singleton class managing the overall simulation.
/// </summary>
public class GameStateManager : MonoBehaviour {
	// The camera object, to be referenced in Unity Editor.
	[SerializeField] private new CinemachineVirtualCamera camera;

	// The name of the track to be loaded
	// [SerializeField] private SceneAsset trackAsset;
	[SerializeField] private string trackName;

	/// <summary>
	/// The UIController object.
	/// </summary>
	public UIController uiController { get; set; }

	public static GameStateManager instance { get; private set; }

	private void Awake() {
		if (instance != null) {
			Debug.LogError("Multiple GameStateManagers in the Scene.");
			return;
		}
		instance = this;

		// Load track
		SceneManager.LoadScene(trackName, LoadSceneMode.Additive);
	}

	public void begin() {
		TrackManager.instance.bestCarChanged += OnBestCarChanged;
		EvolutionManager.instance.startEvolution();
	}

	// Callback method for when the best car has changed.
	private void OnBestCarChanged(CarController formerBestCar, CarController bestCar) {
		if (bestCar != null) {
			camera.LookAt = bestCar.transform;
			camera.Follow = bestCar.transform;
		}
		
		if (uiController != null) uiController.setDisplayTarget(bestCar);
	}
}
}