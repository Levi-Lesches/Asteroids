using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {
	public GameObject asteroid;
	public GameObject ship;

	public float invincibleDelay = 2f;
	public float maxAsteroidSpeed = 1f;

	private int waveNumber = 0;
	private int asteroidsLeft = 0;

	// Start is called before the first frame update
	void Start() {
		SpawnWave();
	}

	// Update is called once per frame
	void Update() {
		
	}

	Vector2 GetRandomVector(float xthreshold, float ythreshold) {
		float randomX = Random.Range(-xthreshold, xthreshold);
		float randomY = Random.Range(-ythreshold, ythreshold);
		return new Vector2(randomX, randomY);
	}

	Quaternion GetRandomRotation() {
		float randomZ = Random.Range(0f, 360f);
		return Quaternion.Euler(0f, 0f, randomZ);
	}

	IEnumerator MakePlayerInvincible() {
		// Destructible script = ship.GetComponent<Destructible>();
		Destructible.isInvincible = true;
		yield return new WaitForSeconds(invincibleDelay);
		Destructible.isInvincible = false;
	}

	void SpawnWave() {
		StartCoroutine(MakePlayerInvincible());
		waveNumber++;
		int numAsteroids = waveNumber + 5;
		asteroidsLeft = numAsteroids;
		for (int _ = 0; _ < numAsteroids; _++) {
			GameObject obj = Instantiate(asteroid, GetRandomVector(8, 4), GetRandomRotation());
			Destructible script = obj.GetComponent<Destructible>();
			Rigidbody2D rigidbody = obj.GetComponent<Rigidbody2D>();
			script.controller = this;
			rigidbody.velocity = GetRandomVector(maxAsteroidSpeed, maxAsteroidSpeed);
		}
	}

	public void OnShipDestroyed() {
		Debug.Log("The ship has been destroyed. Restarting...");
		SceneManager.LoadScene("Main");
	}

	public void OnAsteroidDestroyed() {
		Debug.Log("Asteroid has been destroyed.");
		asteroidsLeft--;
		if (asteroidsLeft == 0) {
			// Debug.Log("You win!");
			SpawnWave();
		}
	}
}
