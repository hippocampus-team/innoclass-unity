using System.Collections.Generic;
using General;
using MLAPI;
using UnityEngine;

public class NetworkAssistant : MonoBehaviour {
	[SerializeField] private List<GameObject> networkingButtons;

	public void OnHostButton() {
		HideNetworkingButtons();
		NetworkManager.Singleton.StartHost();
		GameStateManager.instance.begin();
	}

	public void OnClientButton() {
		HideNetworkingButtons();
		NetworkManager.Singleton.StartClient();
		GameStateManager.instance.begin();
	}

	private void HideNetworkingButtons() {
		foreach (GameObject button in networkingButtons)
			button.SetActive(false);
	}
}