using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileRemove : MonoBehaviour {

	// Use this for initialization
	public GameObject player;
	public Vector3 curpos;
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
		curpos = gameObject.transform.position;	
	}
	
	// Update is called once per frame
	void Update () {

		if(Vector3.Distance(player.transform.position,curpos)<=2){
			StartCoroutine(wait());
		}
	}

	IEnumerator wait(){
		yield return new WaitForSeconds(5f);
		Destroy (gameObject);
	}
}
