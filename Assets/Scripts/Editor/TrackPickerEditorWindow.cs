using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Editor {
    public class TrackPickerEditorWindow : EditorWindow {
        private const string windowTitle = "Track Picker";
        private const int numberOfDigitsInScenesCode = 7;

        private void OnGUI() {
            titleContent = new GUIContent(windowTitle);
            
            GUILayout.Label("Tracks:", EditorStyles.boldLabel);
            
            IEnumerable<string> tracksNames = getTracksNames();
            foreach (string trackName in tracksNames)
                if (GUILayout.Button(new GUIContent(trackName), GUILayout.MaxWidth(300)))
                    openTrack(getTrackPathFromName(trackName));

            if (GUILayout.Button(new GUIContent("Create New Track"), GUILayout.MaxWidth(300)))
                openTrack(createNewTrack());
        }

        private static IEnumerable<string> getTracksNames() {
            string[] tracksGuids = AssetDatabase.FindAssets("Track-", new [] { Paths.trackScenesFolderPath });

            List<string> tracksNames = new List<string>();
            foreach (string guid in tracksGuids) {
                string[] assetPath = AssetDatabase.GUIDToAssetPath(guid).Split('/');
                tracksNames.Add(assetPath[assetPath.Length - 1].Split('.')[0]);
            }

            return tracksNames;
        }

        private static void openTrack(string path) {
            EditorSceneManager.SaveOpenScenes();
            EditorSceneManager.OpenScene(path);
        }
        
        private static string createNewTrack() {
            string newScenePath = generateNewScenePath();
            AssetDatabase.CopyAsset(Paths.emptyTrackScenePath, newScenePath);
            EditorBuildSettings.scenes = getNewBuildScenesArray(EditorBuildSettings.scenes, newScenePath);
            return newScenePath;
        }
        
        private static string generateNewScenePath() {
            return getTrackPathFromName("Track-" + getUniqueNumber());
        }
        
        private static string getTrackPathFromName(string name) {
            return Paths.trackScenesFolderPath + "/" + name + ".unity";
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