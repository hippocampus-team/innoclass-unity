using UnityEngine;
using UnityEngine.SceneManagement;

namespace General {
public class MainSceneLoader : MonoBehaviour {
	private void Awake() {
		SceneManager.LoadScene("Main", LoadSceneMode.Additive);
	}
}
}