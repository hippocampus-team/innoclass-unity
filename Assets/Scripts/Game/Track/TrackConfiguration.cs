using MLAPI;
using MLAPI.NetworkVariable;
using UnityEngine;

namespace Game.Track {
public class TrackConfiguration : NetworkBehaviour {
	public static TrackConfiguration instance;

	public bool isNetworkedTrack;
	private NetworkVariable<bool> raceStarted;
	public bool raceStartedAccessor {
		set => raceStarted.Value = value;
	}

	private void Awake() {
		if (instance != null) {
			Debug.LogError("Multiple TrackConfigurations in one Scene");
			return;
		}
		instance = this;

		raceStarted = new NetworkVariable<bool>(false);
		raceStarted.OnValueChanged += onRaceStateUpdated;
	}

	private void onRaceStateUpdated(bool oldValue, bool newValue) {
		if (!oldValue && newValue) 
			GameStateManager.instance.onRaceStarted();
	}
}
}