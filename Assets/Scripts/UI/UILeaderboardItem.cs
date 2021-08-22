using UnityEngine;
using UnityEngine.UI;

namespace UI {
public class UILeaderboardItem : MonoBehaviour {
	public Text indexText;
	public Text nameText;
	public Text progressText;

	private SpriteRenderer sprite;

	private void Awake() {
		sprite = GetComponent<SpriteRenderer>();
	}

	public bool hasFinished {
		set => sprite.color = value ? Color.yellow : Color.white;
	}
}
}