using System;
using System.Collections.Generic;
using System.Linq;
using General;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Simulation {
public static class TracksManager {
	public static IEnumerable<string> getTracksNamesOfType(TrackType trackType) {
		IEnumerable<string> tracks = getTracksNamesAtPath(Paths.trackScenesFolderPath + trackType switch {
			TrackType.story => Paths.storyTracksPrefixPath,
			TrackType.ugc => Paths.ugcTracksPrefixPath,
			_ => throw new ArgumentOutOfRangeException(nameof(trackType), trackType, null)
		});

		if (trackType == TrackType.story && !BossLevelsLocker.areBossLevelsOpened()) 
			tracks = tracks.Where(name => !name.StartsWith("11 "));
		
		return tracks;
	}

	public static void openTrack(string name, TrackType trackType) {
		if (isTrackInAssets(SceneManager.GetActiveScene().path)) EditorSceneManager.SaveOpenScenes();
		EditorSceneManager.OpenScene(getTrackPathFromName(name, trackType));
	}

	public static string createNewTrack() {
		string newSceneName = TrackNameGenerator.getRandomName();
		string newScenePath = getTrackPathFromName(newSceneName, TrackType.ugc);
		AssetDatabase.CopyAsset(Paths.emptyTrackScenePath, newScenePath);
		EditorBuildScenesHelper.append(newScenePath);
		return newSceneName;
	}

	public static void deleteTrack(string name) {
		string scenePath = getTrackPathFromName(name, TrackType.ugc);
		EditorBuildScenesHelper.removeWithPath(scenePath);
		AssetDatabase.DeleteAsset(scenePath);
	}

	private static IEnumerable<string> getTracksNamesAtPath(string path) {
		string[] tracksGuids = AssetDatabase.FindAssets("t:Scene", new[] { path });

		List<string> tracksNames = new List<string>();
		foreach (string guid in tracksGuids) {
			string[] assetPath = AssetDatabase.GUIDToAssetPath(guid).Split('/');
			tracksNames.Add(assetPath[assetPath.Length - 1].Split('.')[0]);
		}

		return tracksNames;
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

	private static bool isTrackInAssets(string path) {
		return AssetDatabase.LoadAssetAtPath<SceneAsset>(path) != null;
	}

	public enum TrackType {
		story,
		ugc
	}
}
}