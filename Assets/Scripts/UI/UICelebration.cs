using UnityEngine;

namespace UI {
public class UICelebration : MonoBehaviour {
	public static UICelebration instance;

	private void Awake() {
		instance = this;
	}

	public void celebrate(string name) {
		
	}
}
}