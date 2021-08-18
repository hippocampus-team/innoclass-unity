using MLAPI;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
public class UILobby : MonoBehaviour {
	[SerializeField] private Text countText;
	[SerializeField] private GameObject container;

	public static UILobby instance;

	private void Awake() {
		instance = this;
	}

	public void show(bool isAsHost) {
		if (!isAsHost) {
			countText.text = "Успешное подключение";
		}
		
		container.SetActive(true);
	}

	public void hide() {
		container.SetActive(false);
	}

	public void updateValue() {
		countText.text = NetworkManager.Singleton.ConnectedClients.Count + " / " + PlayerPrefs.GetInt("lobby_players_limit");
	}

	private void Update() {
		if (Time.frameCount % 8 != 0) return;
		updateValue();
	}
}
}