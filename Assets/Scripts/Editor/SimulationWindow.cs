using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor {
    public class SimulationWindow : EditorWindow {
        private const string windowTitle = "Simulation";
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

            IEnumerable<string> tracksNames = TracksAccessor.getTracksNamesOfType(TracksAccessor.TrackType.story);
            foreach (string trackName in tracksNames)
                if (GUILayout.Button(new GUIContent(trackName), GUILayout.MaxWidth(trackButtonFullWidth)))
                    TracksAccessor.openTrack(trackName, TracksAccessor.TrackType.story);
            
            GUILayout.EndVertical();
        }
        
        private void showUgcTracksGUI() {
            GUILayout.BeginVertical(GUILayout.MaxWidth(trackButtonFullWidth + editorGap));
            GUILayout.Label("Your Tracks:", EditorStyles.boldLabel);
            
            IEnumerable<string> tracksNames = TracksAccessor.getTracksNamesOfType(TracksAccessor.TrackType.ugc);
            foreach (string trackName in tracksNames) {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(trackName, GUILayout.MaxWidth(trackButtonBaseWidth)))
                    TracksAccessor.openTrack(trackName, TracksAccessor.TrackType.ugc);
                if (GUILayout.Button("Delete", GUILayout.MaxWidth(trackButtonIconWidth - editorGap)))
                    TracksAccessor.deleteTrack(trackName);
                GUILayout.EndHorizontal();
            }

            if (GUILayout.Button(new GUIContent("Create New Track"), GUILayout.MaxWidth(trackButtonFullWidth)))
                TracksAccessor.openTrack(TracksAccessor.createNewTrack(), TracksAccessor.TrackType.ugc);
            
            GUILayout.EndVertical();
        }
    }
}