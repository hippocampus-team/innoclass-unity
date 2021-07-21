using Game;
using Game.Track;
using MLAPI;
using MLAPI.NetworkVariable;
using Simulation;
using UnityEngine;

namespace Networking {
public class NetworkMirrorCarController : MonoBehaviour {
	public new Transform transform;
	private NetworkObject networkObject;
	
	public NetworkVariable<string> username;
	public NetworkVariable<float> progress;

	private void Awake() {
		transform = GetComponent<Transform>();
		networkObject = GetComponent<NetworkObject>();
		username = new NetworkVariable<string>();
		progress = new NetworkVariable<float>();
		
		if (!networkObject.IsOwner) return;
		username.Value = UserManager.username;
		progress.Value = 0f;
	}

	private void Start() {
		GameStateManager.instance.onMirrorCarCreated(this);
		// if (!networkObject.IsOwner) return;
		// GetComponentInChildren<TextMesh>().text = UserManager.username;
	}

	private void FixedUpdate() {
		if (!networkObject.IsOwner || TrackManager.instance.bestCarAccessor == null) return;
		transform.position = TrackManager.instance.bestCarAccessor.transform.position;
		transform.rotation = TrackManager.instance.bestCarAccessor.transform.rotation;
	}
}
}