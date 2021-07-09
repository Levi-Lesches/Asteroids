using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour {
	public GameObject projectile;
	public Transform projectileSpawnPoint;
	public float projectileSpeed = 10f;
	public float shootDelay = 0.25f;
	public float shootGroupDelay = 1f;
	public int maxShots = 4;

	public float rotationSpeed = 300f;
	public float speedBoost = 400f;
	public float maxSpeed = 10f;

	private float timeSinceLastFired = 0f; 
	private int shotsFired = 0;
	private new Rigidbody2D rigidbody;

	void Start() {
		rigidbody = GetComponent<Rigidbody2D>();
	}

	public void Fire() {
		if (timeSinceLastFired < (shotsFired == 0 ? shootGroupDelay : shootDelay))
			return;

		GameObject spawned = Instantiate(  // Don't use parent here!
			projectile, 
			projectileSpawnPoint.position, 
			projectileSpawnPoint.rotation
		);
		Vector2 velocity = (transform.rotation * Vector2.up) * projectileSpeed;
		spawned.GetComponent<Rigidbody2D>().velocity = velocity;
		timeSinceLastFired = 0f;
		shotsFired++;
		if (shotsFired == 4) {
			shotsFired = 0;
		}
	}

	void FixedUpdate() {
		// Instantiate a new projectile and send it flying.
		timeSinceLastFired += Time.fixedDeltaTime;
		if (shotsFired > 0 && timeSinceLastFired >= shootGroupDelay) shotsFired = 0;
		if (Input.GetButton("Fire1")) Fire();

		// Rotate the ship
		Vector3 rotation = Vector3.back * Input.GetAxis("Horizontal") * rotationSpeed * Time.fixedDeltaTime;
		transform.Rotate(rotation);

		// Push the ship forward
		Vector2 force = Vector2.up * Input.GetAxis("Vertical") * speedBoost * Time.fixedDeltaTime;
		rigidbody.AddRelativeForce(force);
		rigidbody.velocity = Vector2.ClampMagnitude(rigidbody.velocity, maxSpeed);
	}
}
