using Game;
using Game.Track;
using UnityEditor;
using UnityEngine;

namespace Editor {
[CustomEditor(typeof(GameStateManager))]
public class GameStateManagerEditor : UnityEditor.Editor {
	public override void OnInspectorGUI() {
		DrawDefaultInspector();
		if (GUILayout.Button("Start race")) 
			TrackConfiguration.instance.raceStartedAccessor = true;
	}
}
}