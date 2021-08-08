using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour {
	public GameObject projectile;
	public Transform projectileSpawnPoint;
	public GameController controller;
	public SpriteRenderer[] sprites;

	public float flashDelay = 1/3;
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
	private float timeSinceLastFlash;
	private bool isVisible = true;

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

	public void StopFlashing() {
		isVisible = true;
		timeSinceLastFlash = 0;
		Flash(true);
	}

	void Flash(bool visibility) {
		foreach (SpriteRenderer sprite in sprites) {
			sprite.enabled = visibility;
		}
	}

	public void Update() {
		if (Destructible.isInvincible) {
			timeSinceLastFlash += Time.deltaTime;
			if (timeSinceLastFlash >= flashDelay) {
				timeSinceLastFlash = 0;
				isVisible = !isVisible;
				Flash(isVisible);
			}
		}
	}

	void FixedUpdate() {
		// Instantiate a new projectile and send it flying.
		timeSinceLastFired += Time.fixedDeltaTime;
		if (shotsFired > 0 && timeSinceLastFired >= shootGroupDelay) shotsFired = 0;
		if (!controller.isMobile && Input.GetButton("Fire1")) Fire();

		// Rotate the ship
		float input = controller.isMobile 
			? controller.hud.rotationAxis 
			: Input.GetAxisRaw("Horizontal");
		Vector3 rotation = Vector3.back * input * rotationSpeed * Time.fixedDeltaTime;
		transform.Rotate(rotation);

		// Push the ship forward
		input = controller.isMobile
			? controller.hud.thrust
			: Input.GetAxis("Vertical");
		Vector2 force = Vector2.up * input * speedBoost * Time.fixedDeltaTime;
		rigidbody.AddRelativeForce(force);
		rigidbody.velocity = Vector2.ClampMagnitude(rigidbody.velocity, maxSpeed);
	}
}
