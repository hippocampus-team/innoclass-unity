using MLAPI;
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
		if (!networkObject.IsOwner) return;
		transform.position = TrackManager.Instance.BestCar.transform.position;
		transform.rotation = TrackManager.Instance.BestCar.transform.rotation;
	}
}
}
