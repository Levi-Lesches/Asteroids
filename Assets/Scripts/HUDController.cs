using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDController : MonoBehaviour {
	public GameObject controls;

	public float rotationAxis = 0f;
	public float thrust = 0f;

	public void OnRotateRelease() {rotationAxis = 0f;}
	public void OnLeft() {rotationAxis = -1f;}
	public void OnRight() {rotationAxis = 1f;}

	public void onThrust() {thrust = 1f;}
	public void onThrustRelease() {thrust = 0f;}
}
