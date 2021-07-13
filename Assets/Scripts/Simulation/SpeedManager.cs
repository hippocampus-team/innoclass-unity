using UnityEngine;

namespace Simulation {
public static class SpeedManager {
	public static void resetSpeed() { setSpeed(1); }

	public static void setSpeed(int value) {
		Time.timeScale = value;
	}
}
}