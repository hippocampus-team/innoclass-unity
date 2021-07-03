using System.Linq;
using UnityEditor;

namespace General {
public static class EditorBuildScenesHelper {
	public static void append(string newScenePath) {
		EditorBuildSettings.scenes = EditorBuildSettings.scenes.ToList()
			.Append(new EditorBuildSettingsScene(newScenePath, true)).ToArray();
	}

	public static void removeWithPath(string removeScenePath) {
		EditorBuildSettings.scenes = EditorBuildSettings.scenes.ToList()
			.Where(scene => !scene.path.Equals(removeScenePath)).ToArray();
	}
	
	public static bool isSceneWithNameExists(string name) {
		return EditorBuildSettings.scenes.ToList()
			.Any(scene => scene.path.Split('/').Last().Equals(name + ".unity"));
	}
}
}