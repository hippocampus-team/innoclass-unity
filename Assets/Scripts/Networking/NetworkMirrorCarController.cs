using MLAPI;
using Simulation;
using UnityEngine;

namespace Networking {
public class NetworkMirrorCarController : MonoBehaviour {
	private new Transform transform;
	private NetworkObject networkObject;

	private void Awake() {
		transform = GetComponent<Transform>();
		networkObject = GetComponent<NetworkObject>();
	}
	
	private void FixedUpdate() {
		if (!networkObject.IsOwner || TrackManager.instance.bestCarAccessor == null) return;
		transform.position = TrackManager.instance.bestCarAccessor.transform.position;
		transform.rotation = TrackManager.instance.bestCarAccessor.transform.rotation;
	}
}
}
