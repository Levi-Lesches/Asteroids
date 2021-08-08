using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour {
	public static bool isInvincible = false;

	public int level = 3;
	public GameController controller;

	// Start is called before the first frame update
	void Start() {
		
	}

	// Update is called once per frame
	void Update() {
		
	}

	void OnTriggerEnter2D(Collider2D other) {
		// TODO: Add explosions

		// Enemies can phase through each other because Unity ignores 
		// collisions in the "Enemy" layer.
		if (gameObject.CompareTag("Projectile")) Destroy(gameObject);
		if (other.CompareTag("Projectile") || (!isInvincible && !other.CompareTag("Border"))) {
			if (gameObject.CompareTag("Asteroid")) {
				controller.OnAsteroidDestroyed(transform.position, level);
			} else if (gameObject.CompareTag("Player")) {
				controller.OnShipDestroyed();
			} else if (gameObject.CompareTag("Enemy")) {
				controller.onEnemyShipDestroyed();
			}

			Destroy(gameObject);
		}
	}
}
