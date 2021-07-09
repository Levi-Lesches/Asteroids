using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour {
	public static bool isInvincible = false;

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
		if (other.CompareTag("Projectile") || (!isInvincible && !other.CompareTag("Border"))) {
			if (gameObject.CompareTag("Asteroid")) {
				controller.OnAsteroidDestroyed();
			} else if (gameObject.CompareTag("Player")) {
				controller.OnShipDestroyed();
			}

			Destroy(gameObject);
		}
	}
}