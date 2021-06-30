using UnityEngine;
using UnityEngine.SceneManagement;

namespace Simulation {
    public class MainSceneLoader : MonoBehaviour {
        private void Awake() {
            SceneManager.LoadScene("Main", LoadSceneMode.Additive);
        }
    }
}
