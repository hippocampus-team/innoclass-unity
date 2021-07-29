using UnityEngine;

namespace General {
public static class Paths {
	public const string emptyTrackScenePath = "Assets/Scenes/Tracks/Empty.unity";
	public static readonly string topologySavePath = Application.streamingAssetsPath + "/Saves/Topology";
	public static readonly string modelsFolderPath = Application.streamingAssetsPath + "/Saves";
	
	public static readonly string aiCodeTargetPath = Application.streamingAssetsPath + @"\ai.py";
	public const string aiCodeSourcePath = "C://VSCode/VSCode/VSCodeProjects/AI-MasterClass/task.py";

	public const string trackScenesFolderPath = "Assets/Scenes/Tracks";
	public const string storyTracksPrefixPath = "/Story";
	public const string ugcTracksPrefixPath = "/UGC";
}
}