using Game.Track;
using UnityEditor;
using UnityEngine;

namespace Editor {
public class SimulationControlWindow  : EditorWindow {
	private const string windowTitle = "Simulation Control";
	
	private void OnGUI() {
		titleContent = new GUIContent(windowTitle);
		GUILayout.BeginVertical();
		drawRunAsHost();
		drawPlayersLimit();
		drawStartRaceButton();
		GUILayout.EndVertical();
	}

	private static void drawRunAsHost() {
		PlayerPrefs.SetInt("runMultiplayerAsHost", EditorGUILayout.Toggle(new GUIContent("Run As Host"), PlayerPrefs.GetInt("runMultiplayerAsHost", 0) == 1) ? 1 : 0);
	}

	private static void drawPlayersLimit() {
		GUILayout.BeginHorizontal();
		GUILayout.Label("Лимит подключений:", EditorStyles.label);
		PlayerPrefs.SetInt("lobby_players_limit", EditorGUILayout.IntField(PlayerPrefs.GetInt("lobby_players_limit"), GUILayout.ExpandWidth(false)));
		GUILayout.EndHorizontal();
	}

	private static void drawStartRaceButton() {
		if (GUILayout.Button("Start race")) 
			TrackConfiguration.instance.raceStartedAccessor = true;
	}
}
}