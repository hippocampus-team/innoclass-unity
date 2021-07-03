using UnityEngine;

namespace Game.Car {
/// <summary>
/// Class representing a sensor reading the distance to the nearest obstacle in a specified direction.
/// </summary>
public class Sensor : MonoBehaviour {
	[SerializeField] private LayerMask layerToSense;
	[SerializeField] private SpriteRenderer cross;
	private new Transform transform;
	private Transform crossTransform;

	private const float maxDist = 6f;
	private const float minDist = 0.01f;

	/// <summary>
	/// The current sensor readings in percent of maximum distance.
	/// </summary>
	public float output { get; private set; }

	private void Awake() {
		transform = GetComponent<Transform>();
		crossTransform = cross.transform;
	}

	private void Start() {
		cross.gameObject.SetActive(true);
	}

	private void FixedUpdate() {
		Vector3 position = transform.position;

		Vector2 direction = crossTransform.position - position;
		direction.Normalize();

		RaycastHit2D hit = Physics2D.Raycast(position, direction, maxDist, layerToSense);

		if (hit.collider == null)
			hit.distance = maxDist;
		else if (hit.distance < minDist)
			hit.distance = minDist;

		output = hit.distance / maxDist;
		cross.transform.position = (Vector2) transform.position + direction * hit.distance;
	}

	public void hide() { cross.gameObject.SetActive(false); }
	public void show() { cross.gameObject.SetActive(true); }

}
}