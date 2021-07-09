using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
	public GameObject player;
	public GameObject projectile;
	public Transform projectileSpawnPoint;

	public float projectileSpeed;
	public float randomError;
	public float shootDelay;

	private float timeSinceLastShot;

	void Fire() {
		GameObject spawned = Instantiate(  // Don't use parent here!
			projectile, 
			projectileSpawnPoint.position, 
			projectileSpawnPoint.rotation
		);
		Vector2 velocity = (transform.rotation * Vector2.up) * projectileSpeed;
		spawned.GetComponent<Rigidbody2D>().velocity = velocity;
	}

	void FireAtPlayer() {
		float playerAngle = Vector2.Angle(transform.parent.position, player.transform.position);
		float randomAngle = Random.Range(playerAngle - randomError, playerAngle + randomError);
		transform.RotateAround(transform.position, Vector3.up, randomAngle);
		Fire();
	}

	public void FixedUpdate() {
		timeSinceLastShot += Time.fixedDeltaTime;
		if (timeSinceLastShot >= shootDelay) {
			FireAtPlayer();
			timeSinceLastShot = 0;
		}
	}
}
