using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour {

	private float speed = 4.0f;

	private CharacterController controller;
	// Use this for initialization
	void Start () {
		controller = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () {
		controller.Move((Vector3.forward*speed) * Time.deltaTime);
	}
}
