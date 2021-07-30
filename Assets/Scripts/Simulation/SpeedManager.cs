using Game.Track;
using UnityEngine;

namespace Simulation {
public static class SpeedManager {
	public static void resetSpeed() { setSpeed(1); }

	public static void setSpeed(int value) {
		if (!Application.isPlaying) return;
		if (TrackConfiguration.instance != null && TrackConfiguration.instance.isNetworkedTrack) return;
		Time.timeScale = value;
	}
}
}