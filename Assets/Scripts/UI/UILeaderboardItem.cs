using UnityEngine;
using UnityEngine.UI;

namespace UI {
public class UILeaderboardItem : MonoBehaviour {
	public Text indexText;
	public Text nameText;
	public Text progressText;

	private Image image;

	private void Awake() {
		image = GetComponent<Image>();
	}

	public bool hasFinished {
		set => image.color = value ? Color.yellow : Color.white;
	}
}
}