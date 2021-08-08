using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Border : MonoBehaviour {
	public static float width, height;

	public enum Axis {Horizontal, Vertical};
	public Axis axis;
	public bool isPositive;
	private float offset = 0f;

	// Start is called before the first frame update
	void Start() {
		EdgeCollider2D collider = GetComponent<EdgeCollider2D>();
		Vector3 newPosition = transform.position;
		Camera camera = Camera.main;

		Vector2[] edges = new Vector2[] {};
		height = camera.orthographicSize;
		width = camera.orthographicSize * camera.aspect;
		int factor = isPositive ? 1 : -1;

		// Sizes and translates the edge colliders to fit the screen.
		switch (axis) {
			case Axis.Horizontal:
				// newPosition.x = camera.aspect * camera.orthographicSize * factor;
				edges = new Vector2[2] {
					new Vector2((width + offset) * factor, height), 
					new Vector2((width + offset) * factor, -height)
				};
				break;
			case Axis.Vertical:
				// newPosition.y = camera.orthographicSize * factor;
				edges = new Vector2[2] {
					new Vector2(-width, (height + offset) * factor), 
					new Vector2(width, (height + offset) * factor)
				};
				break;
		}
		collider.points = edges;
	}

	// Inverts a coordinate on one of either axis. 
	// 
	// For example, a ship going through the top border will have its y coordinate
	// inverted, so it appears to teleport back to the bottom.
	Vector3 getNewSpawnPosition(Vector2 position) {
		float factor = isPositive ? 0.25f : -0.25f;
		switch (axis) {
			case Axis.Vertical: return new Vector2(position.x, -position.y + factor);
			case Axis.Horizontal: return new Vector2(-position.x + factor, position.y);
		}
		return position;
	}

	// BUG: this allows the ship to go out then back in before "onExit" is triggered.
	// 	Fix it by moving the border further off-screen and using OnTriggerEnter2D. 
	void OnTriggerEnter2D(Collider2D other) {
		other.transform.position = getNewSpawnPosition(other.transform.position);
	}
}
