using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using General;
using UnityEditor;
using UnityEngine;

namespace Editor {
    public class SimulationWindow : EditorWindow {
        private const string windowTitle = "Simulation";
        private const string modelsHintText = "Это ваши модели. Выбирайте те, которые хотите тренировать и загружайте карту. Натренируйте модели разнообразно.";
        private const int editorGap = 3;
        private const int trackButtonBaseWidth = 260;
        private const int trackButtonIconWidth = 68;
        private const int trackButtonFullWidth = trackButtonBaseWidth + trackButtonIconWidth;

        private void OnGUI() {
            titleContent = new GUIContent(windowTitle);
            GUILayout.BeginHorizontal();
            showStoryTracksGUI();
            showUgcTracksGUI();
            showModelsControlGUI();
            GUILayout.EndHorizontal();
        }

        private static void showStoryTracksGUI() {
            GUILayout.BeginVertical(GUILayout.MaxWidth(trackButtonFullWidth + editorGap));
            GUILayout.Label("Story Tracks:", EditorStyles.boldLabel);

            IEnumerable<string> tracksNames = TracksAccessor.getTracksNamesOfType(TracksAccessor.TrackType.story);
            foreach (string trackName in tracksNames)
                if (GUILayout.Button(new GUIContent(trackName), GUILayout.MaxWidth(trackButtonFullWidth)))
                    TracksAccessor.openTrack(trackName, TracksAccessor.TrackType.story);
            
            GUILayout.EndVertical();
        }
        
        private static void showUgcTracksGUI() {
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
        
        private static void showModelsControlGUI() {
            GUILayout.BeginVertical(GUILayout.MaxWidth(trackButtonFullWidth + editorGap));
            GUILayout.Label("Your Models:", EditorStyles.boldLabel);
            
            EditorGUILayout.HelpBox(new GUIContent(modelsHintText));
            
            foreach (SimulationModel model in ModelsManager.getInstance().models) {
                bool newIsActivated = GUILayout.Toggle(model.isActivated, new GUIContent(model.name));
                if (newIsActivated == model.isActivated) continue;
                model.isActivated = newIsActivated;
                ModelsManager.getInstance().saveEverything();
            }
            
            // Possible addition to simulation control
            //
            // GUILayout.Label("Topology:", EditorStyles.boldLabel);
            //
            // GUILayout.BeginHorizontal();
            // uint[] layers = ModelsManager.getInstance().topology;
            // for (int i = 0; i < layers.Length; i++) {
            //     uint newLayerSize = Convert.ToUInt32(EditorGUILayout.IntField(Convert.ToInt32(layers[i]), GUILayout.ExpandWidth(false)));
            //     if (newLayerSize == layers[i]) continue;
            //     ModelsManager.getInstance().topology[i] = newLayerSize;
            //     ModelsManager.getInstance().saveTopology();
            // }
            // GUILayout.EndHorizontal();
            
            GUILayout.EndVertical();
        }
    }
}