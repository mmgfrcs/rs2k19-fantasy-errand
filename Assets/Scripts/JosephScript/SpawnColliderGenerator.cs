using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnColliderGenerator : MonoBehaviour {

	public Transform[] spawnPos;
	public GameObject Junction;
	public GameObject normalRoad;
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider hit){
		if (gameObject.tag == "Start") {
			gameObject.name="Last";
			for(int i=0;i<10;i++){
				GameObject newpath =(GameObject)Instantiate (Junction,spawnPos[0].transform.position,spawnPos[0].transform.rotation);
			}
		}

		if (hit.gameObject.tag == "Player") {
			Instantiate(Junction,spawnPos[0].transform.position,spawnPos[0].transform.rotation);
				
		}
	}
}
