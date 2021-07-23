using Simulation;
using UnityEditor;

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
}
}