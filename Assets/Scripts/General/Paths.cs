using UnityEngine;

namespace General {
    public static class Paths {
        public const string emptyTrackScenePath = "Assets/Scenes/Tracks/Empty.unity";
        public const string mainScenePath = "Assets/Scenes/Main.unity";
        public static readonly string topologySavePath = Application.streamingAssetsPath + "/Saves/Topology";
        public static readonly string modelsFolderPath = Application.streamingAssetsPath + "/Saves";

        public const string trackScenesFolderPath = "Assets/Scenes/Tracks";
        public const string storyTracksPrefixPath = "/Story";
        public const string ugcTracksPrefixPath = "/UGC";
    }
}