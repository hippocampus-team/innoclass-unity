using UnityEditor;

namespace Editor {
public static class MenuItems {
	[MenuItem("Simulation/Open Simulation Window")]
	private static void openSimulationWindow() {
		EditorWindow.GetWindow(typeof(SimulationWindow)).Show();
	}
}
}