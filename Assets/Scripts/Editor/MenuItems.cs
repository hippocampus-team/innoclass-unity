using Simulation;
using UnityEditor;
using UnityEngine;

namespace Editor {
public static class MenuItems {
	[MenuItem("Window/Simulation")]
	private static void openSimulationWindow() {
		EditorWindow.GetWindow(typeof(SimulationWindow)).Show();
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