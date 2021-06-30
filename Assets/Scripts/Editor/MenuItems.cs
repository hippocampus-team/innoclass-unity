using UnityEditor;
using UnityEditor.SceneManagement;

namespace Editor {
public static class MenuItems {
	[MenuItem("Simulation/Go back to Main")]
	private static void goBackToMain() {
		EditorSceneManager.SaveOpenScenes();
		EditorSceneManager.OpenScene(Paths.mainScenePath);
	}
	
	[MenuItem("Simulation/Open Track Picker")]
	private static void openTrackPicker() {
		TrackPickerEditorWindow window = (TrackPickerEditorWindow)EditorWindow.GetWindow(typeof(TrackPickerEditorWindow));
		window.Show();
	}
}
}
