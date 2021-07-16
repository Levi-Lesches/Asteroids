using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AimBot : MonoBehaviour { 
	public GameController controller;
	public ShipController ship;

	public float foresight = 3f;
	public float trackInterval = 0.05f;
	public float rotationSpeed = 5f;

	private Quaternion fromRotation, toRotation;

	private float t = 1f;
	private bool isReady = false;

	Vector3 GetFuturePosition(GameObject obj, float time) {
		Vector2 velocity = obj.GetComponent<Rigidbody2D>().velocity;
		Vector3 displacement = velocity * time;
		return obj.transform.position + displacement;
	}

	GameObject? GetNearestTarget() {
		float? minDistance = null;
		GameObject? target = null;
		foreach (GameObject enemy in controller.enemies.Concat(controller.asteroids)) {
			if (enemy == null) continue;
			else if (enemy.CompareTag("Enemy")) return enemy;
			float distance = Vector3.Distance(transform.position, enemy.transform.position);
			if (minDistance == null || distance < minDistance) {
				minDistance = distance;
				target = enemy;
			}
		}
		return target;
	}

	Vector3 GetIntercept(GameObject target) {
		for (float time = 0f; time <= foresight; time += trackInterval) {
			Vector2 futurePosition = GetFuturePosition(target, time);
			float distance = Vector2.Distance(transform.position, futurePosition);
			float travelTime = distance / ship.projectileSpeed;
			if (travelTime <= time && (travelTime - time) <= trackInterval) {  // this is the intercept point
				return futurePosition;
			}
		}
		return Vector3.zero;  // cannot find an intercept point
	}

	Quaternion GetRotation(Vector3 target) {
		Vector3 difference = target - transform.position;
		Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90) * difference;
		return Quaternion.LookRotation(Vector3.forward, difference);
	}

	void Start() {
		fromRotation = transform.rotation;
		toRotation = transform.rotation;
	}

	Quaternion FindTarget() {
		GameObject? target = GetNearestTarget();
		if (target == null) {
			return transform.rotation;
		}
		Vector3 position = isReady ? GetIntercept(target) : target.transform.position;
		Debug.DrawLine(transform.position, position, Color.white, 0.5f);
		Quaternion rotation = GetRotation(position);
		float angle = rotation.eulerAngles.z;
		return rotation;
	}

	void FixedUpdate() {
		/* 
			Selects a target and rotates towards it, firing when ready. 

			The algorithm works in two parts. First, it chooses the closest target and 
			rotates to face it. Then, the algorithm predicts the trajectory of the 
			target, finds the optimal angle to shoot at, rotates there and fires.

			The [FindTarget] method handles finding a rotation to aim towards. This 
			function simply handles rotation to the given target.
		*/
		t += rotationSpeed * Time.fixedDeltaTime;
		transform.rotation = Quaternion.Slerp(fromRotation, toRotation, t);

		if (t >= 1) {  // shoot and reset
			ship.Fire();
			fromRotation = transform.rotation;
			toRotation = FindTarget();
			isReady = !isReady;
			t = 0;
		}
	}
}
