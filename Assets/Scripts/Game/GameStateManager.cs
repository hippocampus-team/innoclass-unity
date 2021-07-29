using System;
using AI.Evolution;
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
	
	public static bool userControl => 
		!TrackConfiguration.instance.isNetworkedTrack && UserManager.userControl;

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

		if (userControl) Time.timeScale = 1f;

		if (TrackConfiguration.instance.isNetworkedTrack) setupNetworking();
		else startRace();
	}

	private void setupNetworking() {
		if (runMultiplayerAsHost) NetworkManager.Singleton.StartHost();
		else NetworkManager.Singleton.StartClient();
		NetworkManager.Singleton.OnClientDisconnectCallback += onClientDisconnected;
		UserManager.userControl = false;
		Time.timeScale = 1f;
	}

	private void onClientDisconnected(ulong clientId) {
		Debug.Log("Disconnect detected");
		// ReSharper disable once Unity.NoNullPropagation
		NetworkManager.Singleton.ConnectedClients[clientId]?.PlayerObject?.GetComponent<NetworkMirrorCarController>()?.prepareForRemoval();
	}

	private static bool isSimulationConfiguredCorrectly() {
		return true;
	}
	
	public void onMirrorCarCreated(NetworkMirrorCarController carController) {
		if (!NetworkManager.Singleton.IsHost) return;
		CameraManager.instance.addToTrackingGroup(carController.transform);
	}

	public void startCountdown() {
		countdown.count();
	}
	
	public void startRace() {
		if (NetworkManager.Singleton.IsHost) 
			NetworkPlayersLeaderboardCollector.instance.initiate();
		else {
			TrackManager.instance.bestCarChanged += OnBestCarChanged;
			EvolutionManager.instance.startEvolution();
		}
	}
	
	private void OnBestCarChanged(CarController formerBestCar, CarController bestCar) {
		if (TrackConfiguration.instance.isNetworkedTrack && runMultiplayerAsHost) return;

		if (bestCar != null && !userControl)
			CameraManager.instance.trackSolo(bestCar.transform);

		if (uiController != null) uiController.setDisplayTarget(bestCar);
	}
}
}