using UnityEditor;
using UnityEditor.SceneManagement;

namespace Editor {
public static class MenuItems {
	[MenuItem("Simulation/Go back to Main")]
	private static void goBackToMain() {
		EditorSceneManager.SaveOpenScenes();
		EditorSceneManager.OpenScene(Paths.mainScenePath);
	}
	
	[MenuItem("Simulation/Open Simulation Window")]
	private static void openSimulationWindow() {
		EditorWindow.GetWindow(typeof(SimulationWindow)).Show();
	}
}
}
