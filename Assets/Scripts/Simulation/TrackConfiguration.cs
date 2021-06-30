using UnityEngine;

namespace Simulation {
    public class TrackConfiguration : MonoBehaviour {
        public static TrackConfiguration instance;
        
        public bool isNetworkedTrack;

        private void Awake() {
            if (instance != null) {
                Debug.LogError("Multiple TrackConfigurations in one Scene");
                return;
            }
            instance = this;
        }
    }
}
