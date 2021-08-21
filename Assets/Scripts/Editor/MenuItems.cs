using System.Globalization;
using Simulation;
using UnityEditor;
using UnityEngine;

namespace Editor {
public static class MenuItems {
	[MenuItem("Window/Simulation")]
	private static void openSimulationWindow() {
		EditorWindow.GetWindow(typeof(SimulationWindow)).Show();
	}
	
	[MenuItem("Window/Simulation Control")]
	private static void openSimulationControlWindow() {
		EditorWindow.GetWindow(typeof(SimulationControlWindow)).Show();
	}
	
	[MenuItem("Edit/Restart everything")]
	private static void restartEverything() {
		ModelsManager.getInstance().regenerateModels();
		lockBossLevel();
		PythonCodeSyncer.removeSyncedFile();
		UserManager.userControl = false;
		Time.timeScale = 1f;
		PlayerPrefs.SetInt("runMultiplayerAsHost", 0);
		UserManager.username = Random.Range(100000, 99999999).ToString(CultureInfo.InvariantCulture);
		TracksManager.deleteAllUgcTracks();
		TracksManager.openTrack("01 - Начало", TracksManager.TrackType.story);
	}
	
	[MenuItem("Edit/Regenerate Saved Models")]
	private static void regenerateSavedModels() {
		ModelsManager.getInstance().regenerateModels();
	}
	
	[MenuItem("Edit/Lock Boss Level")]
	private static void lockBossLevel() {
		BossLevelsLocker.lockBossLevels();
	}
	
	[MenuItem("Help/Разблокировать финальный уровень")]
	private static void unlockBossLevel() { 
		BossLevelsLocker.unlockBossLevels();
	}
}
}