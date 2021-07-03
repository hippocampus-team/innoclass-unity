using UnityEngine;

namespace Game.Track {
/// <summary>
/// Class representing a checkpoint of a race track.
/// </summary>
public class Checkpoint : MonoBehaviour {

	/// <summary>
	/// The radius in Unity units in which this checkpoint can be captured.
	/// </summary>
	public float captureRadius = 3;

	/// <summary>
	/// The reward value earned by capturing this checkpoint.
	/// </summary>
	public float rewardValue { get; set; }

	/// <summary>
	/// The distance in Unity units to the previous checkpoint on the track.
	/// </summary>
	public float distanceToPrevious { get; set; }

	/// <summary>
	/// The accumulated distance in Unity units from the first to this checkpoint.
	/// </summary>
	public float accumulatedDistance { get; set; }

	/// <summary>
	/// The accumulated reward earned for capturing all checkpoints from the first to this one.
	/// </summary>
	public float accumulatedReward { get; set; }

	/// <summary>
	/// Calculates the reward earned for the given distance to this checkpoint.
	/// </summary>
	/// <param name="currentDistance">The distance to this checkpoint.</param>
	public float getRewardValue(float currentDistance) {
		//Calculate how close the distance is to capturing this checkpoint, relative to the distance from the previous checkpoint
		float completePerc = (distanceToPrevious - currentDistance) / distanceToPrevious;

		//Reward according to capture percentage
		if (completePerc < 0) return 0;
		return completePerc * rewardValue;
	}

}
}