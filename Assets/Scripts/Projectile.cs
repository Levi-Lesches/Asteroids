using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
	public float lifespan = 2f;

	private float timeOfBirth, timeOfDeath;

	// Start is called before the first frame update
	void Start() {
		timeOfBirth = Time.time;
		timeOfDeath = timeOfBirth + lifespan;
	}

	// Update is called once per frame
	void Update() {
		if (Time.time >= timeOfDeath) {
			Destroy(gameObject);
		}
	}
}
