using System.Linq;
using UnityEditor;

namespace General {
public static class EditorBuildScenesHelper {
	public static void append(string newScenePath) {
		// Legacy code:
		// EditorBuildSettingsScene[] modified = new EditorBuildSettingsScene[EditorBuildSettings.scenes.Length + 1];
		// EditorBuildSettings.scenes.CopyTo(modified, 0);
		// modified[modified.Length - 1] = new EditorBuildSettingsScene(newScenePath, true);
		// EditorBuildSettings.scenes = modified;

		EditorBuildSettings.scenes = EditorBuildSettings.scenes.ToList().Append(new EditorBuildSettingsScene(newScenePath, true)).ToArray();
	}

	public static void removeWithPath(string removeScenePath) {
		// Legacy code:
		// EditorBuildSettingsScene[] modified = new EditorBuildSettingsScene[EditorBuildSettings.scenes.Length - 1];
		//
		// IEnumerator<EditorBuildSettingsScene> enumerator = (IEnumerator<EditorBuildSettingsScene>)EditorBuildSettings.scenes.GetEnumerator();
		// for (int i = 0; i < modified.Length; i++) {
		//     if (enumerator.Current.path.Equals(removeScenePath)) enumerator.MoveNext();
		//     if (enumerator.Current != null) modified[i] = enumerator.Current;
		//     enumerator.MoveNext();
		// }
		// enumerator.Dispose();
		//
		// EditorBuildSettings.scenes = modified;

		EditorBuildSettings.scenes = EditorBuildSettings.scenes.ToList().Where(scene => !scene.path.Equals(removeScenePath)).ToArray();
	}
}
}