using Game;
using Game.Track;
using MLAPI;
using MLAPI.NetworkVariable;
using Simulation;
using UnityEngine;

namespace Networking {
public class NetworkMirrorCarController : NetworkBehaviour {
	public new Transform transform;
	private NetworkObject networkObject;
	
	private NetworkVariable<string> username;
	private NetworkVariable<float> progress;
	public string usernameAccessor => username?.Value ?? "";
	public float progressAccessor {
		get => progress?.Value ?? 0f;
		set => progress.Value = value;
	}

	private void Awake() {
		transform = GetComponent<Transform>();
		networkObject = GetComponent<NetworkObject>();
		username = new NetworkVariable<string>(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.OwnerOnly });
		progress = new NetworkVariable<float>(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.OwnerOnly });

		username.OnValueChanged += onUsernameSet;
	}

	private void Start() {
		if (networkObject.IsOwner) {
			username.Value = UserManager.username;
			progress.Value = 0f;
			TrackManager.instance.networkCar = this;
		}
		
		if (NetworkManager.Singleton.IsHost && networkObject.IsOwner) return;
		GameStateManager.instance.onMirrorCarCreated(this);
	}

	private void FixedUpdate() {
		if (!networkObject.IsOwner || TrackManager.instance.bestCarAccessor == null) return;
		transform.position = TrackManager.instance.bestCarAccessor.transform.position;
		transform.rotation = TrackManager.instance.bestCarAccessor.transform.rotation;
	}

	private void onUsernameSet(string oldValue, string newValue) {
		GetComponentInChildren<TextMesh>().text = newValue;
		if (!NetworkManager.Singleton.IsHost || networkObject.IsOwner) return;
		NetworkPlayersLeaderboardCollector.instance.addPlayer(this);
	}

	public override void OnLostOwnership() {
		Debug.Log("Yesss");
	}
}
}