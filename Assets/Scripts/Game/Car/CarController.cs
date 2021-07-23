using AI;
using UnityEngine;

namespace Game.Car {
/// <summary>
/// Class representing a controlling container for a 2D physical simulation
/// of a car with 5 front facing sensors, detecting the distance to obstacles.
/// </summary>
public class CarController : MonoBehaviour {

	#region IDGenerator

	// Used for unique ID generation
	private static int idGenerator;

	/// <summary>
	/// Returns the next unique id in the sequence.
	/// </summary>
	private static int NextID => idGenerator++;

	#endregion

	// Maximum delay in seconds between the collection of two checkpoints until this car dies.
	private const float maxCheckpointDelay = 7;

	/// <summary>
	/// The underlying AI agent of this car.
	/// </summary>
	public Agent agent { get; set; }

	public float currentCompletionReward {
		get => agent.genotype.evaluation;
		set => agent.genotype.evaluation = value;
	}

	/// <summary>
	/// Whether this car is controllable by user input (keyboard).
	/// </summary>
	public bool useUserInput;

	public CarMovement movement { get; private set; }
	private SpriteRenderer spriteRenderer { get; set; }
	public new Transform transform { get; private set; }

	/// <summary>
	/// The current inputs for controlling the CarMovement component.
	/// </summary>
	public double[] currentControlInputs => movement.currentInputs;

	private Sensor[] sensors;
	private float timeSinceLastCheckpoint;

	private void Awake() {
		movement = GetComponent<CarMovement>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		transform = GetComponent<Transform>();
		sensors = GetComponentsInChildren<Sensor>();
	}

	private void Start() {
		movement.hitWall += die;

		// Set name to be unique
		name = "Car (" + NextID + ")";
	}

	/// <summary>
	/// Restarts this car, making it movable again.
	/// </summary>
	public void restart() {
		movement.enabled = true;
		timeSinceLastCheckpoint = 0;

		foreach (Sensor s in sensors)
			s.show();

		agent.reset();
		movement.moveToStart();
		enabled = true;
	}

	// Unity method for normal update
	private void Update() {
		timeSinceLastCheckpoint += Time.deltaTime;
	}

	// Unity method for physics update
	private void FixedUpdate() {
		if (useUserInput || Time.frameCount % 2 == 0) return;
		// Get readings from sensors
		double[] sensorOutput = new double[sensors.Length];
		for (int i = 0; i < sensors.Length; i++)
			sensorOutput[i] = sensors[i].output;

		double[] controlInputs = agent.process(sensorOutput);
		movement.setInputs(controlInputs);
			
		if (timeSinceLastCheckpoint > maxCheckpointDelay) die();
	}

	// Makes this car die (making it unmovable and stops the Agent from calculating the controls for the car).
	private void die() {
		enabled = false;
		movement.stop();
		movement.enabled = false;

		foreach (Sensor s in sensors)
			s.hide();

		agent.kill();
	}

	public void checkpointCaptured() {
		timeSinceLastCheckpoint = 0;
	}

	public void setProgressSprite(Sprite sprite) {
		if (useUserInput) return;
		spriteRenderer.sprite = sprite;
	}
	
	public void setPlayerSprite(Sprite sprite) {
		spriteRenderer.sprite = sprite;
		spriteRenderer.sortingOrder = 11;
	}

}
}