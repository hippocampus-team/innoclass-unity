using UnityEngine;

namespace Simulation {
	/// <summary>
	/// Component for car movement and collision detection.
	/// </summary>
	public class CarMovement : MonoBehaviour {

		/// <summary>
		/// Event for when the car hit a wall.
		/// </summary>
		public event System.Action hitWall;

		//Movement constants
		private const float maxVel = 20f;
		private const float acceleration = 8f;
		private const float velFriction = 2f;
		private const float turnSpeed = 100;

		private new Transform transform;
		private CarController controller;

		/// <summary>
		/// The current velocity of the car.
		/// </summary>
		private float velocity { get; set; }

		/// <summary>
		/// The current rotation of the car.
		/// </summary>
		private Quaternion rotation { get; set; }

		private double horizontalInput, verticalInput; // Horizontal = engine force, Vertical = turning force
		private Vector3 lastPosition;

		/// <summary>
		/// The current inputs for turning and engine force in this order.
		/// </summary>
		public double[] currentInputs { get { return new[] { horizontalInput, verticalInput }; } }
		
		private Vector3 startPosition;
		private Quaternion startRotation;

		private void Awake() {
			transform = GetComponent<Transform>();
			controller = GetComponent<CarController>();
			
			startPosition = transform.position;
			startRotation = transform.rotation;

			lastPosition = startPosition;
		}

		// Unity method for physics updates
		private void FixedUpdate() {
			//nGet user input if controller tells us to
			if (controller != null && controller.useUserInput)
				checkInput();

			applyInput();
			applyVelocity();
			applyFriction();
		}

		// Checks for user input
		private void checkInput() {
			horizontalInput = Input.GetAxis("Horizontal");
			verticalInput = Input.GetAxis("Vertical");
		}

		// Applies the currently set input
		private void applyInput() {
			if (verticalInput > 1)
				verticalInput = 1;
			else if (verticalInput < -1)
				verticalInput = -1;

			if (horizontalInput > 1)
				horizontalInput = 1;
			else if (horizontalInput < -1)
				horizontalInput = -1;

			//Car can only accelerate further if velocity is lower than engineForce * MAX_VEL
			bool canAccelerate = false;
			if (verticalInput < 0)
				canAccelerate = velocity > verticalInput * maxVel;
			else if (verticalInput > 0)
				canAccelerate = velocity < verticalInput * maxVel;

			//Set velocity
			if (canAccelerate) {
				velocity += (float) verticalInput * acceleration * Time.deltaTime;

				//Cap velocity
				if (velocity > maxVel)
					velocity = maxVel;
				else if (velocity < -maxVel)
					velocity = -maxVel;
			}

			//Set rotation
			rotation = transform.rotation;
			rotation *= Quaternion.AngleAxis((float) -horizontalInput * turnSpeed * Time.deltaTime, new Vector3(0, 0, 1));
		}

		/// <summary>
		/// Sets the engine and turning input according to the given values.
		/// </summary>
		/// <param name="input">The inputs for turning and engine force in this order.</param>
		public void setInputs(double[] input) {
			horizontalInput = input[0];
			verticalInput = input[1];
		}

		// Applies the current velocity to the position of the car.
		private void applyVelocity() {
			Vector3 direction = new Vector3(0, 1, 0);
			transform.rotation = rotation;
			direction = rotation * direction;
			lastPosition += direction * (velocity * Time.deltaTime);
			transform.position = lastPosition;
		}

		// Applies some friction to velocity
		private void applyFriction() {
			if (verticalInput != 0) return;
			if (velocity > 0) {
				velocity -= velFriction * Time.deltaTime;
				if (velocity < 0)
					velocity = 0;
			} else if (velocity < 0) {
				velocity += velFriction * Time.deltaTime;
				if (velocity > 0)
					velocity = 0;
			}
		}
	
		private void OnCollisionEnter2D() {
			hitWall?.Invoke();
		}

		/// <summary>
		/// Stops all current movement of the car.
		/// </summary>
		public void stop() {
			velocity = 0;
			rotation = Quaternion.AngleAxis(0, new Vector3(0, 0, 1));
		}
		
		public void moveToStart() {
			transform.position = startPosition;
			transform.rotation = startRotation;
			lastPosition = startPosition;
		}

	}
}