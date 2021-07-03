using MLAPI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Networking {
[ExecuteInEditMode]
public class NetworkManagerScenesUpdater : MonoBehaviour {
	private void OnValidate() {
		updateNetworkSettings();
	}

	private void updateNetworkSettings() {
		NetworkManager networkManager = GetComponent<NetworkManager>();
		if (networkManager.NetworkConfig == null) return;
		
		networkManager.NetworkConfig.RegisteredScenes.Clear();
		networkManager.NetworkConfig.RegisteredScenes.Add("Main");
		networkManager.NetworkConfig.RegisteredScenes.Add(SceneManager.GetActiveScene().name);
	}
}
}