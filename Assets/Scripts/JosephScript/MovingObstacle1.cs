using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObstacle1 : MonoBehaviour {

	// Use this for initialization
	public int speed;
	public bool isBoulder;
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate (-Vector3.forward * speed * Time.deltaTime, Space.World);
		if(isBoulder==true)
			transform.Rotate (-180f*Time.deltaTime, 0f, 0f);
	}
}
