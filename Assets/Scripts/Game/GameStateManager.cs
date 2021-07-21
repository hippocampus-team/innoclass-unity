using System.Collections.Generic;
using AI.Evolution;
using Cinemachine;
using Game.Car;
using Game.Track;
using MLAPI;
using Networking;
using Simulation;
using UI;
using UnityEditor;
using UnityEngine;

namespace Game {
public class GameStateManager : MonoBehaviour {
	[SerializeField] private bool runMultiplayerAsHost;

	[SerializeField] private UICountdown countdown;
	public UIController uiController { get; set; }

	public static GameStateManager instance { get; private set; }

	private void Awake() {
		if (instance != null) {
			Debug.LogError("Multiple GameStateManagers in the Scene!");
			return;
		}
		instance = this;

		countdown.onCountdownEnded += startRace;
	}

	private void Start() {
		if (!isSimulationConfiguredCorrectly()) {
			EditorApplication.ExitPlaymode();
			return;
		}

		if (TrackConfiguration.instance.isNetworkedTrack) setupNetworking();
		else startRace();
	}

	private void setupNetworking() {
		if (runMultiplayerAsHost) NetworkManager.Singleton.StartHost();
		else NetworkManager.Singleton.StartClient();
	}

	private static bool isSimulationConfiguredCorrectly() {
		return ModelsManager.getInstance().isNumberOfActiveModelsValid();
	}
	
	public void onMirrorCarCreated(NetworkMirrorCarController carController) {
		if (!NetworkManager.Singleton.IsHost) return;
		CameraManager.instance.addToTrackingGroup(carController.transform);
		NetworkPlayersLeaderboardCollector.instance.addPlayer(carController);
	}

	public void startCountdown() {
		countdown.count();
	}
	
	public void startRace() {
		if (NetworkManager.Singleton.IsHost) return;
		TrackManager.instance.bestCarChanged += OnBestCarChanged;
		EvolutionManager.instance.startEvolution();
		NetworkPlayersLeaderboardCollector.instance.initiate();
	}
	
	private void OnBestCarChanged(CarController formerBestCar, CarController bestCar) {
		if (TrackConfiguration.instance.isNetworkedTrack && runMultiplayerAsHost) return;

		if (bestCar != null && !UserManager.userControl)
			CameraManager.instance.trackSolo(bestCar.transform);

		if (uiController != null) uiController.setDisplayTarget(bestCar);
	}
}
}