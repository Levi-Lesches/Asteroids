using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDController : MonoBehaviour {
	public GameObject controls;

	public float rotationAxis = 0f;

	// Start is called before the first frame update
	void Start() {
		
	}

	// Update is called once per frame
	void Update() {
		
	}

	public void OnRelease() {
		Debug.Log("Pointer up");
		rotationAxis = 0f;
	}
	public void OnDown() {Debug.Log("Pointer down");}
	public void OnLeft() {rotationAxis = -1f;}
	public void OnRight() {
		Debug.Log("Pointer right");
		rotationAxis = 1f;
	}
}
