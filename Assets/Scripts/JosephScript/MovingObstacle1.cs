using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObstacle1 : MonoBehaviour {

	// Use this for initialization
	public int speed;
	public GameObject player;
	public int minDistance;
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
	}
	
	// Update is called once per frame
	void Update () {
		if (Vector3.Distance (player.transform.position, transform.position) < minDistance) {
			transform.Translate (-Vector3.forward * speed * Time.deltaTime, Space.World);
			transform.Rotate (-180f*Time.deltaTime, 0f, 0f);
		}
	}
}
