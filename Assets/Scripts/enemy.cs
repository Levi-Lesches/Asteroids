using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
	public GameController controller;
	public GameObject projectile;
	public Transform projectileSpawnPoint;
	public Transform aim;

	public float shipSpeed;
	public float turnDelay;
	public float turnOffset;
	public float projectileSpeed;
	public float randomError;
	public float shootDelay;

	private Rigidbody2D rigidbody;
	private float timeSinceLastShot;
	private Vector2 forward;

	void Start() {
		rigidbody = GetComponent<Rigidbody2D>();
		float x = transform.position.x;
		forward = (x > 0 ? Vector2.left : Vector2.right);
		rigidbody.velocity = forward * shipSpeed;
		int direction = Random.value > 0.5 ? 1 : -1;
		StartCoroutine(ChangeVelocity(direction));
	}

	IEnumerator ChangeVelocity(int direction) {
		yield return new WaitForSeconds(turnDelay);
		Vector2 newVelocity = new Vector2(rigidbody.velocity.x, turnOffset * direction);
		rigidbody.velocity = newVelocity;
		yield return new WaitForSeconds(turnDelay);
		rigidbody.velocity = forward * shipSpeed;
		StartCoroutine(ChangeVelocity(-direction));
	}

	void Fire() {
		Vector2 velocity = (aim.rotation * Vector2.up) * projectileSpeed;
		GameObject obj = Instantiate(  // Don't use parent here!
			projectile, 
			projectileSpawnPoint.position, 
			projectileSpawnPoint.rotation
		);
		obj.GetComponent<Rigidbody2D>().velocity = velocity;
	}

	Vector3 GetRandomVector() {
		return new Vector3(
			Random.Range(-randomError, randomError), 
			Random.Range(-randomError, randomError)
		);
	}

	void FireAtPlayer() {
		Vector3 playerPosition = controller.player.transform.position;
		Vector3 currentPosition = transform.position;
		Vector3 difference = playerPosition - currentPosition;
		aim.rotation = Quaternion.LookRotation(
			Vector3.forward, 
			difference + GetRandomVector()
		);
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
