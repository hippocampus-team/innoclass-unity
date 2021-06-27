using System;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Editor {
public static class MenuItems {
	private const int numberOfDigitsInScenesCode = 7;
	private const string emptyScenePath = "Assets/Scenes/Tracks/Empty.unity";

	[MenuItem("Simulation/Create New Track")]
	private static void createNewTrack() {
		string newScenePath = generateNewScenePath();
		AssetDatabase.CopyAsset(emptyScenePath, newScenePath);
		EditorBuildSettings.scenes = getNewBuildScenesArray(EditorBuildSettings.scenes, newScenePath);
		EditorSceneManager.SaveOpenScenes();
		EditorSceneManager.OpenScene(newScenePath);
	}

	private static string generateNewScenePath() {
		return "Assets/Scenes/Tracks/Track-" + getUniqueNumber() + ".unity";
	}

	// Not Unique at all
	private static int getUniqueNumber() {
		double milliseconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
		return Convert.ToInt32(Convert.ToInt64(milliseconds) % Math.Pow(10, numberOfDigitsInScenesCode));
	}

	private static EditorBuildSettingsScene[] getNewBuildScenesArray(EditorBuildSettingsScene[] original, string newScenePath) {
		EditorBuildSettingsScene[] modified = new EditorBuildSettingsScene[original.Length + 1];
		original.CopyTo(modified, 0);
		modified[modified.Length - 1] = new EditorBuildSettingsScene(newScenePath, true);
		return modified;
	}
}
}
