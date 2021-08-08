using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {
	// Prefabs
	public GameObject asteroid;
	public GameObject shipPrefab;
	public GameObject enemyShipBig;
	public GameObject enemyShipSmall;

	// Other GameObjects to track
	public List<GameObject> enemies = new List<GameObject>();
	public List<GameObject> asteroids = new List<GameObject>();
	public GameObject player;
	public HUDController hud;

	// Config -- consider moving this elsewhere
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
	private int enemySpawnDelay = 30;
	private int waveNumber = 0;
	private int asteroidsLeft = 0;
	private bool isFiring = false;
	private bool enemyShipPresent = false;

	// Start is called before the first frame update
	void Start() {
		SpawnPlayer();
		SpawnWave();
		hud.controls.SetActive(isMobile);
		hud.SetScore(points);
		hud.SetLives(lives);
	}

	// Update is called once per frame
	void Update() {
		if (isFiring) player.GetComponent<ShipController>().Fire();
		timeSinceEnemyShip += Time.deltaTime;
		if (timeSinceEnemyShip >= enemySpawnDelay && !enemyShipPresent) SpawnEnemyShip();
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
		player.GetComponent<ShipController>().StopFlashing();
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
		asteroids.Add(obj);
		asteroidsLeft++;
	}

	void SpawnWave() {
		timeSinceEnemyShip = 0f;
		enemySpawnDelay = 30;
		asteroids = new List<GameObject>();
		enemies = new List<GameObject>();
		StartCoroutine(MakePlayerInvincible());
		waveNumber++;
		for (int _ = 0; _ < waveNumber + asteroidsOnFirstWave; _++) {
			SpawnAsteroid(GetRandomVector(Border.width - 1, Border.height - 1), numLevels);
		}
	}

	void SpawnEnemyShip() {
		// Configure enemy ship
		GameObject prefab = points > pointsBeforeSmallShip 
			? enemyShipSmall : enemyShipBig;
		int direction = (Random.value > 0.5f) ? 1 : -1;
		Vector3 position = new Vector3(Border.width * 5/6 * direction, Border.height * 3/4 * direction, 0);
		GameObject obj = Instantiate(prefab, position, Quaternion.identity);
		obj.GetComponent<Destructible>().controller = this;
		obj.GetComponent<Enemy>().controller = this;

		// Update state
		enemies.Add(obj);
		enemyShipPresent = true;
		asteroidsLeft++;
	}

	public void OnShipDestroyed() {
		lives--;
		hud.SetLives(lives);
		if (lives == 0)
			SceneManager.LoadScene("Main");
		else 
			SpawnPlayer();
	}

	public void AddPoints(int newPoints) {
		int oldLives = points / pointsPerLife;
		points += newPoints;
		hud.SetScore(points);
		int newLives = points / pointsPerLife;
		lives += newLives - oldLives;
		if (lives > 3) lives = 3;
		hud.SetLives(lives);
	}

	public void OnAsteroidDestroyed(Vector3 position, int level) {
		AddPoints(asteroidPoints [level - 1]);
		SpawnAsteroid(position + GetRandomVector(asteroidOffset, asteroidOffset), level - 1);
		SpawnAsteroid(position + GetRandomVector(asteroidOffset, asteroidOffset), level - 1);
		if (--asteroidsLeft == 0) {
			SpawnWave();
		}
	}

	public void onEnemyShipDestroyed() {
		enemyShipPresent = false;
		AddPoints(enemyShipPoints);
		timeSinceEnemyShip = 0f;
		if (enemySpawnDelay > 10) enemySpawnDelay -= 10;
		if (--asteroidsLeft == 0) {
			SpawnWave();
		}
	}

	public void ToggleAimBot() {
		isAimBotEnabled = !isAimBotEnabled;
		player.GetComponent<AimBot>().enabled = isAimBotEnabled;
	}

	public void ToggleFiring() {isFiring = !isFiring;}
}
