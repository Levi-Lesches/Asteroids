using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {
	// Prefabs
	public GameObject asteroid;
	public GameObject shipPrefab;
	public GameObject enemyShip1Big;
	public GameObject enemyShipSmall;

	// state
	public List<GameObject> enemies = new List<GameObject>();
	public GameObject player;
	public HUDController hud;

	// Config
	public float invincibleDelay = 2f;
	public float maxAsteroidSpeed = 1f;
	public float asteroidOffset = 0.5f;
	public int[] asteroidPoints = {100, 50, 20};
	public int enemyShipPoints = 200;
	public int pointsPerLife = 10_000;
	public int pointsBeforeSmallShip = 6_000;
	public int asteroidsOnFirstWave = 4;
	public int numLevels = 3;

	// Public state
	public int lives = 3;
	public int points = 0;
	public bool isAimBotEnabled = true;
	public bool isMobile = false;

	// Private state
	private float timeSinceEnemyShip = 0f;
	private int enemySpawnDelay = 30f;
	private int waveNumber = 0;
	private int asteroidsLeft = 0;
	private bool isFiring = false;

	// Start is called before the first frame update
	void Start() {
		SpawnPlayer();
		SpawnWave();
		hud.controls.SetActive(isMobile);
	}

	// Update is called once per frame
	void Update() {
		if (isFiring) player.GetComponent<ShipController>().Fire();
	}

	void FixedUpdate() {
		timeSinceEnemyShip += Time.fixedDeltaTime;
		if (timeSinceEnemyShip >= enemySpawnDelay) SpawnEnemyShip();
	}

	Vector3 GetRandomVector(float xthreshold, float ythreshold) {
		float randomX = Random.Range(-xthreshold, xthreshold);
		float randomY = Random.Range(-ythreshold, ythreshold);
		return new Vector2(randomX, randomY);
	}

	Quaternion GetRandomRotation() {
		float randomZ = Random.Range(0f, 360f);
		return Quaternion.Euler(0f, 0f, randomZ);
	}

	IEnumerator MakePlayerInvincible() {
		Destructible.isInvincible = true;
		yield return new WaitForSeconds(invincibleDelay);
		Destructible.isInvincible = false;
	}

	void SpawnPlayer() {
		StartCoroutine(MakePlayerInvincible());
		player = Instantiate(shipPrefab);
		AimBot aimBot = player.GetComponent<AimBot>();
		player.GetComponent<ShipController>().controller = this;
		player.GetComponent<Destructible>().controller = this;
		aimBot.controller = this;
		aimBot.enabled = isAimBotEnabled;
	}

	void SpawnAsteroid(Vector3 position, int level) {
		if (level == 0) return;
		Vector3 velocity = GetRandomVector(maxAsteroidSpeed, maxAsteroidSpeed);
		GameObject obj = Instantiate(asteroid, position, GetRandomRotation());
		Destructible script = obj.GetComponent<Destructible>();
		float scale = Mathf.Pow(2, numLevels - level);
		obj.transform.localScale /= scale;
		obj.GetComponent<Rigidbody2D>().velocity = velocity * scale;
		script.controller = this;
		script.level = level;
		enemies.Add(obj);
		asteroidsLeft++;
	}

	void SpawnWave() {
		timeSinceEnemyShip = 0f;
		enemySpawnDelay = 30f;
		enemies = new List<GameObject>();
		StartCoroutine(MakePlayerInvincible());
		waveNumber++;
		for (int _ = 0; _ < waveNumber + asteroidsOnFirstWave; _++) {
			SpawnAsteroid(GetRandomVector(Border.width - 1, Border.height - 1), numLevels);
		}
	}

	void SpawnEnemyShip() {
		timeSinceEnemyShip = 0f;
		if (enemySpawnDelay > 10) enemySpawnDelay -= 10;
		GameObject prefab = points > pointsBeforeSmallShip 
			? enemyShipSmall : enemyShipBig;
		/* TODO: Spawn */
		asteroidsLeft++;
	}

	public void OnShipDestroyed() {
		lives--;
		if (lives == 0)
			SceneManager.LoadScene("Main");
		else 
			SpawnPlayer();
	}

	public void AddPoints(int newPoints) {
		int oldLives = points / pointsPerLife;
		points += newPoints;
		int newLives = points / pointsPerLife;
		lives += newLives - oldLives;
	}

	public void OnAsteroidDestroyed(Vector3 position, int level) {
		AddPoints(asteroidPoints [level]);
		asteroidsLeft--;
		SpawnAsteroid(position + GetRandomVector(asteroidOffset, asteroidOffset), level - 1);
		SpawnAsteroid(position + GetRandomVector(asteroidOffset, asteroidOffset), level - 1);
		if (asteroidsLeft == 0) {
			SpawnWave();
		}
	}

	public void onEnemyShipDestroyed() {
		AddPoints(enemyShipPoints);
		asteroidsLeft--;
	}

	public void ToggleAimBot() {
		isAimBotEnabled = !isAimBotEnabled;
		player.GetComponent<AimBot>().enabled = isAimBotEnabled;
	}

	public void ToggleFiring() {isFiring = !isFiring;}
}
