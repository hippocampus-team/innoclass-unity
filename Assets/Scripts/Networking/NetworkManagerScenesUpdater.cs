using System;
using System.Linq;
using MLAPI;
using UnityEditor;
using UnityEngine;

namespace Networking {
[ExecuteInEditMode]
public class NetworkManagerScenesUpdater : MonoBehaviour {
	private void Awake() {
		updateNetworkSettings();
	}

	private void OnValidate() {
		updateNetworkSettings();
	}

	private void updateNetworkSettings() {
		NetworkManager networkManager = GetComponent<NetworkManager>();
		if (networkManager.NetworkConfig == null) return;

		EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
		networkManager.NetworkConfig.RegisteredScenes.Clear();

		foreach (EditorBuildSettingsScene scene in scenes)
			networkManager.NetworkConfig.RegisteredScenes.Add(
				scene.path.Split('/').Last().Split(new[] { ".unity" }, StringSplitOptions.None).First());
	}
}
}