using UnityEngine;

namespace Simulation {
public class BossLevelsLocker {
	private const string stateCode = "are_boss_levels_opened";
	
	public static void lockBossLevels() {
		PlayerPrefs.SetInt(stateCode, 0);
		PlayerPrefs.Save();
	}
	
	public static void unlockBossLevels() {
		PlayerPrefs.SetInt(stateCode, 1);
		PlayerPrefs.Save();
	}

	public static bool areBossLevelsOpened() {
		return PlayerPrefs.GetInt(stateCode, 0) == 1;
	}
}
}