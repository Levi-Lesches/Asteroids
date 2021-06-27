using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimBot : MonoBehaviour { 
	/// The amount of time in the future the bot will track an object.
	/// 
	/// See [GetAngleToFire] for details on the tracking algorithm.
	public float maxTime;  

	/// The time-intervals at which the bot will track an object. 
	/// 
	/// See [GetAngleToFire] for details on the tracking algorithm.
	public float interval;

	/// The maximum allowed error from the desired rotation, [targetAngle].
	public float maxRotationError;

	/// The script to control the ship.
	public ShipController ship;

	/// The angle at which to shoot at. 
	/// 
	/// The ship has to move to this angle the same way the player would: slowly.
	private float targetAngle;

	Vector3 GetFuturePosition(GameObject obj, float time);
	float GetDistanceTo(Vector2 position);
	float GetTravelTime(float distance);

	float GetAngleToFire(GameObject asteroid) {
		for (float time = 0; time <= maxTime; time += interval) {
			Vector2 futurePosition = GetFuturePosition(asteroid, time);
			float distance = GetDistanceTo(futurePosition);
			float travelTime = GetTravelTime(distance);
			if (Mathf.abs(travelTime - time) <= interval) {
				return Vector2.Angle(transform.position, futurePosition);
			}
		}
	}

	GameObject GetNearestTarget();

	void Start() { }

	void FixedUpdate() {
		/* 
			Selects a target and rotates towards it, firing when ready. 

			If there is no target, selects one. Otherwise, rotates by a small amount
			relative to the target. When the angles match (to a small error), fires 
			a projectile and resets the target, ready for the next frame. 
		*/
		if (targetAngle == null) {
			GameObject target = GetNearestTarget();
			targetAngle = GetAngleToFire(target);
		} else if (Mathf.abs(targetAngle - currentAngle) <= maxRotationError) {
			ship.Fire();
			targetAngle = null;
		} else {
			float rotation = ship.rotationSpeed * fixedDeltaTime;
			if (targetAngle > currentAngle) rotation *= -1;
			transform.Rotate(Vector3.back, rotation);
		}
	}
}
