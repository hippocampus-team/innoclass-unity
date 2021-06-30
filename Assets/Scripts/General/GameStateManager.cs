using AI.Evolution;
using Cinemachine;
using MLAPI;
using Simulation;
using UI;
using UnityEngine;

namespace General {
/// <summary>
/// Singleton class managing the overall simulation.
/// </summary>
public class GameStateManager : MonoBehaviour {
	// The camera object, to be referenced in Unity Editor.
	[SerializeField] private new CinemachineVirtualCamera camera;
	
	[SerializeField] private bool runMultiplayerAsHost;

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
	}

	private void Start() {
		if (TrackConfiguration.instance.isNetworkedTrack) setupNetworking();
		TrackManager.instance.bestCarChanged += OnBestCarChanged;
		EvolutionManager.instance.startEvolution();
	}

	private void setupNetworking() {
		if (runMultiplayerAsHost) NetworkManager.Singleton.StartHost();
		else NetworkManager.Singleton.StartClient();
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