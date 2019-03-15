using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveorb : MonoBehaviour {
	public  Vector3 nextTile;
	public bool genTile;
	public KeyCode moveL;
	public KeyCode moveR;
	public float horizVel=0;
	public int laneNum = 2;
	public string controlLocked ="n";
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		Rigidbody rb = GetComponent<Rigidbody> ();
		rb.velocity = new Vector3 (horizVel, rb.velocity.y, 5);
		Vector3 temp = gameObject.transform.position;
		float x = temp.x;
		float y = temp.y;
		float z = temp.z;

		if (Input.GetKeyDown (moveL)) {
			horizVel=-10;
			StartCoroutine(stopSlide());
		}
		if (Input.GetKeyDown (moveR)) {
			horizVel=+10;
			StartCoroutine(stopSlide());
		}

	}

	void OnCollisionEnter(Collision other){
		if (other.gameObject.tag == "lethal") {
			Destroy(gameObject);
		}
		if (other.gameObject.tag == "collectible") {
			Destroy(other.gameObject);
		}
	}


	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "startTile") {
			nextTile= other.transform.position;
			genTile=true;
		}
	}

	void OnTriggerExit(Collider other){
		if (other.gameObject.tag == "startTile") {
			genTile=false;
		}
	}


	IEnumerator stopSlide(){
		yield return new WaitForSeconds(.5f);
		horizVel=0;
		controlLocked="n";
	}
}
