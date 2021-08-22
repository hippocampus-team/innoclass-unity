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
		bool runMultiplayerAsHost = PlayerPrefs.GetInt("runMultiplayerAsHost", 0) == 1;
		if (runMultiplayerAsHost) {
			NetworkManager.Singleton.OnClientDisconnectCallback += onClientDisconnected;
			NetworkManager.Singleton.ConnectionApprovalCallback += connectionApprovalCheck;
			NetworkManager.Singleton.StartHost();
		} else {
			UserManager.userControl = false;
			Time.timeScale = 1f;
			NetworkManager.Singleton.StartClient();
		}
		UILobby.instance.show(runMultiplayerAsHost);
	}

	private void onClientDisconnected(ulong clientId) {
		Debug.Log("Disconnect detected");
		// ReSharper disable once Unity.NoNullPropagation
		// NetworkManager.Singleton.ConnectedClients[clientId]?.PlayerObject?.GetComponent<NetworkMirrorCarController>()?.prepareForRemoval();
		NetworkPlayersLeaderboardCollector.instance.removePlayerWithClientId(clientId);
	}
	
	private static void connectionApprovalCheck(byte[] connectionData, ulong clientId, NetworkManager.ConnectionApprovedDelegate callback) {
		callback(true, null, isAllowedToConnect(), null, null);
	}

	private static bool isAllowedToConnect() {
		return NetworkManager.Singleton.ConnectedClients.Count < PlayerPrefs.GetInt("lobby_players_limit");
	}

	private static bool isSimulationConfiguredCorrectly() {
		return true;
	}

	public void onNetworkCarFinished(int index) {
		CarController car = TrackManager.instance.getCar(index);
		car.transform.position = Vector3.zero;
		car.gameObject.SetActive(false);
		TrackManager.instance.removeCarAt(index);
		UICelebration.instance.celebrate("Test");
	}
	
	public void onMirrorCarCreated(NetworkMirrorCarController carController) {
		if (!NetworkManager.Singleton.IsHost) return;
		CameraManager.instance.addToTrackingGroup(carController.transform);
	}

	public void startCountdown() {
		UILobby.instance.hide();
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
		if (TrackConfiguration.instance.isNetworkedTrack && NetworkManager.Singleton.IsHost) return;

		if (bestCar != null && !userControl)
			CameraManager.instance.trackSolo(bestCar.transform);

		if (uiController != null) uiController.setDisplayTarget(bestCar);
	}
}
}