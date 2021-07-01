using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Editor {
    public class TrackPickerWindow : EditorWindow {
        private const string windowTitle = "Track Picker";
        private const int numberOfDigitsInScenesCode = 7;

        private const int editorGap = 3;
        private const int trackButtonBaseWidth = 260;
        private const int trackButtonIconWidth = 68;
        private const int trackButtonFullWidth = trackButtonBaseWidth + trackButtonIconWidth;

        private void OnGUI() {
            titleContent = new GUIContent(windowTitle);
            GUILayout.BeginHorizontal();
            showStoryTracksGUI();
            showUgcTracksGUI();
            GUILayout.EndHorizontal();
        }

        private void showStoryTracksGUI() {
            GUILayout.BeginVertical(GUILayout.MaxWidth(trackButtonFullWidth + editorGap));
            GUILayout.Label("Story Tracks:", EditorStyles.boldLabel);

            IEnumerable<string> tracksNames = getStoryTracksNames();
            foreach (string trackName in tracksNames)
                if (GUILayout.Button(new GUIContent(trackName), GUILayout.MaxWidth(trackButtonFullWidth)))
                    openTrack(getTrackPathFromName(trackName, TrackType.story));
            
            GUILayout.EndVertical();
        }
        
        private void showUgcTracksGUI() {
            GUILayout.BeginVertical(GUILayout.MaxWidth(trackButtonFullWidth + editorGap));
            GUILayout.Label("Your Tracks:", EditorStyles.boldLabel);
            
            IEnumerable<string> tracksNames = getUgcTracksNames();
            foreach (string trackName in tracksNames) {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(trackName, GUILayout.MaxWidth(trackButtonBaseWidth)))
                    openTrack(getTrackPathFromName(trackName, TrackType.ugc));
                if (GUILayout.Button("Delete", GUILayout.MaxWidth(trackButtonIconWidth - editorGap)))
                    deleteTrack(getTrackPathFromName(trackName, TrackType.ugc));
                GUILayout.EndHorizontal();
            }

            if (GUILayout.Button(new GUIContent("Create New Track"), GUILayout.MaxWidth(trackButtonFullWidth)))
                openTrack(createNewTrack());
            
            GUILayout.EndVertical();
        }
        
        private static IEnumerable<string> getStoryTracksNames() {
            return getTracksNamesAtPath(Paths.trackScenesFolderPath + Paths.storyTracksPrefixPath);
        }

        private static IEnumerable<string> getUgcTracksNames() {
            return getTracksNamesAtPath(Paths.trackScenesFolderPath + Paths.ugcTracksPrefixPath);
        }

        private static IEnumerable<string> getTracksNamesAtPath(string path) {
            string[] tracksGuids = AssetDatabase.FindAssets("t:Scene", new [] { path });
            
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
        
        private static void deleteTrack(string path) {
            // TODO
        }
        
        private static string createNewTrack() {
            string newScenePath = generateNewScenePath();
            AssetDatabase.CopyAsset(Paths.emptyTrackScenePath, newScenePath);
            EditorBuildSettings.scenes = getNewBuildScenesArray(EditorBuildSettings.scenes, newScenePath);
            return newScenePath;
        }
        
        private static string generateNewScenePath() {
            return getTrackPathFromName("Track-" + getUniqueNumber(), TrackType.ugc);
        }
        
        private static string getTrackPathFromName(string name, TrackType trackType) {
            string folderPath = Paths.trackScenesFolderPath;
            folderPath += trackType switch {
                TrackType.story => Paths.storyTracksPrefixPath,
                TrackType.ugc => Paths.ugcTracksPrefixPath,
                _ => throw new ArgumentOutOfRangeException(nameof(trackType), trackType, null)
            };

            return folderPath + "/" + name + ".unity";
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

        private enum TrackType {
            story,
            ugc
        }
    }
}