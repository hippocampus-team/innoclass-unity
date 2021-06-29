using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Simulation;

/// <summary>
/// Singleton class managing the current track and all cars racing on it, evaluating each individual.
/// </summary>
public class TrackManager : MonoBehaviour {

	public static TrackManager instance {
		get;
		private set;
	}

	// Sprites for visualising best and second best cars. To be set in Unity Editor.
	[SerializeField] private Sprite bestCarSprite;
	[SerializeField] private Sprite secondBestSprite;
	[SerializeField] private Sprite normalCarSprite;

	private Checkpoint[] checkpoints;

	/// <summary>
	/// Car used to create new cars and to set start position.
	/// </summary>
	public CarController prototypeCar;

	// Start position for cars
	private Vector3 startPosition;
	private Quaternion startRotation;

	// Struct for storing the current cars and their position on the track.
	private class RaceCar {
		public RaceCar(CarController car = null, uint checkpointIndex = 1) {
			this.car = car;
			this.checkpointIndex = checkpointIndex;
		}
		public readonly CarController car;
		public uint checkpointIndex;
	}

	private readonly List<RaceCar> cars = new List<RaceCar>();

	/// <summary>
	/// The amount of cars currently on the track.
	/// </summary>
	private int carCount => cars.Count;

	private CarController bestCar;

	/// <summary>
	/// The current best car (furthest in the track).
	/// </summary>
	public CarController BestCar {
		get => bestCar;
		private set {
			if (bestCar == value) return;
			
			// Update appearance
			if (BestCar != null)
				BestCar.spriteRenderer.sprite = normalCarSprite;
			if (value != null)
				value.spriteRenderer.sprite = bestCarSprite;

			// Set previous best to be second best now
			CarController previousBest = bestCar;
			bestCar = value;
			bestCarChanged?.Invoke(previousBest, bestCar);

			SecondBestCar = previousBest;
		}
	}

	/// <summary>
	/// Event for when the best car has changed.
	/// </summary>
	public event Action<CarController, CarController> bestCarChanged;

	private CarController secondBestCar;

	/// <summary>
	/// The current second best car (furthest in the track).
	/// </summary>
	private CarController SecondBestCar {
		get => secondBestCar;
		set {
			if (SecondBestCar == value) return;
			
			if (SecondBestCar != null && SecondBestCar != BestCar)
				SecondBestCar.spriteRenderer.sprite = normalCarSprite;
			if (value != null)
				value.spriteRenderer.sprite = secondBestSprite;

			secondBestCar = value;
		}
	}


	/// <summary>
	/// The length of the current track in Unity units (accumulated distance between successive checkpoints).
	/// </summary>
	private float trackLength { get; set; }

	private void Awake() {
		if (instance != null) {
			Debug.LogError("Mulitple instance of TrackManager are not allowed in one Scene.");
			return;
		}

		instance = this;

		// Get all checkpoints
		checkpoints = GetComponentsInChildren<Checkpoint>();

		// Set start position and hide prototype
		GameObject carGameObject = prototypeCar.gameObject;
		startPosition = carGameObject.transform.position;
		startRotation = carGameObject.transform.rotation;
		carGameObject.SetActive(false);

		calculateCheckpointPercentages();
	}

	#region Methods

	// Unity method for updating the simulation
	private void Update() {
		// Update reward for each enabled car on the track
		foreach (RaceCar car in cars.Where (car => car.car.enabled)) {
			car.car.currentCompletionReward = getCompletePerc(car.car, ref car.checkpointIndex);

			// Update best
			if (BestCar == null || car.car.currentCompletionReward >= BestCar.currentCompletionReward)
				BestCar = car.car;
			else if (SecondBestCar == null || car.car.currentCompletionReward >= SecondBestCar.currentCompletionReward)
				SecondBestCar = car.car;
		}
	}

	public void setCarAmount(int amount) {
		if (amount < 0) throw new ArgumentException("Amount may not be less than zero.");

		if (amount == carCount) return;

		if (amount > cars.Count) {
			//Add new cars
			for (int toBeAdded = amount - cars.Count; toBeAdded > 0; toBeAdded--) {
				GameObject carCopy = Instantiate(prototypeCar.gameObject);
				carCopy.transform.position = startPosition;
				carCopy.transform.rotation = startRotation;
				CarController controllerCopy = carCopy.GetComponent<CarController>();
				cars.Add(new RaceCar(controllerCopy));
				carCopy.SetActive(true);
			}
		}
		else if (amount < cars.Count) {
			//Remove existing cars
			for (int toBeRemoved = cars.Count - amount; toBeRemoved > 0; toBeRemoved--) {
				RaceCar last = cars[cars.Count - 1];
				cars.RemoveAt(cars.Count - 1);
				Destroy(last.car.gameObject);
			}
		}
	}

	/// <summary>
	/// Restarts all cars and puts them at the track start.
	/// </summary>
	public void restart() {
		foreach (RaceCar car in cars) {
			car.car.transform.position = startPosition;
			car.car.transform.rotation = startRotation;
			car.car.restart();
			car.checkpointIndex = 1;
		}

		BestCar = null;
		SecondBestCar = null;
	}

	/// <summary>
	/// Returns an Enumerator for iterator through all cars currently on the track.
	/// </summary>
	public IEnumerator<CarController> getCarEnumerator() {
		return cars.Select(t => t.car).GetEnumerator();
	}

	/// <summary>
	/// Calculates the percentage of the complete track a checkpoint accounts for. This method will
	/// also refresh the <see cref="trackLength"/> property.
	/// </summary>
	private void calculateCheckpointPercentages() {
		checkpoints[0].accumulatedDistance = 0; //First checkpoint is start
		//Iterate over remaining checkpoints and set distance to previous and accumulated track distance.
		for (int i = 1; i < checkpoints.Length; i++) {
			checkpoints[i].distanceToPrevious =
				Vector2.Distance(checkpoints[i].transform.position, checkpoints[i - 1].transform.position);
			checkpoints[i].accumulatedDistance = checkpoints[i - 1].accumulatedDistance + checkpoints[i].distanceToPrevious;
		}

		//Set track length to accumulated distance of last checkpoint
		trackLength = checkpoints[checkpoints.Length - 1].accumulatedDistance;

		//Calculate reward value for each checkpoint
		for (int i = 1; i < checkpoints.Length; i++) {
			checkpoints[i].rewardValue =
				checkpoints[i].accumulatedDistance / trackLength - checkpoints[i - 1].accumulatedReward;
			checkpoints[i].accumulatedReward = checkpoints[i - 1].accumulatedReward + checkpoints[i].rewardValue;
		}
	}

	// Calculates the completion percentage of given car with given completed last checkpoint.
	// This method will update the given checkpoint index accordingly to the current position.
	private float getCompletePerc(CarController car, ref uint curCheckpointIndex) {
		//Already all checkpoints captured
		if (curCheckpointIndex >= checkpoints.Length)
			return 1;

		//Calculate distance to next checkpoint
		float checkPointDistance =
			Vector2.Distance(car.transform.position, checkpoints[curCheckpointIndex].transform.position);

		//Check if checkpoint can be captured
		if (checkPointDistance <= checkpoints[curCheckpointIndex].captureRadius) {
			curCheckpointIndex++;
			car.checkpointCaptured(); //Inform car that it captured a checkpoint
			return getCompletePerc(car, ref curCheckpointIndex); //Recursively check next checkpoint
		}
		
		// Return accumulated reward of last checkpoint + reward of distance to next checkpoint
		return checkpoints[curCheckpointIndex - 1].accumulatedReward +
			   checkpoints[curCheckpointIndex].getRewardValue(checkPointDistance);
	}

	#endregion

}