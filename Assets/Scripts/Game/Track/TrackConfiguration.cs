using MLAPI;
using UnityEngine;

namespace Game.Track {
public class TrackConfiguration : NetworkBehaviour {
	public static TrackConfiguration instance;

	public bool isNetworkedTrack;
	private bool raceStarted;
	public bool raceStartedAccessor {
		set {
			bool oldValue = raceStarted;
			raceStarted = value;
			if (!oldValue && raceStarted) 
				GameStateManager.instance.onRaceStarted();
		}
	}

	private void Awake() {
		if (instance != null) {
			Debug.LogError("Multiple TrackConfigurations in one Scene");
			return;
		}
		instance = this;
	}
}
}