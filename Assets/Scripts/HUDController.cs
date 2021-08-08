using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour {
	public GameObject controls;
	public Text scoreStat;
	public GameObject[] livesStat;

	public float rotationAxis = 0f;
	public float thrust = 0f;

	public void OnRotateRelease() {rotationAxis = 0f;}
	public void OnLeft() {rotationAxis = -1f;}
	public void OnRight() {rotationAxis = 1f;}

	public void onThrust() {thrust = 1f;}
	public void onThrustRelease() {thrust = 0f;}

	public void SetScore(int score) {
		scoreStat.text = "Score: " + score;
	}

	public void SetLives(int lives) {
		for (int life = 0; life < 3; life++) {
			livesStat [life].SetActive(lives > life);
		}
	}
}
