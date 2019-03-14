using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGenerator : MonoBehaviour {

	public Transform normalTile;
	public GameObject player;
	public Vector3 nextTile;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (player.GetComponent<moveorb> ().genTile == true) {
			nextTile =player.GetComponent<moveorb>().nextTile;
			nextTile.z=nextTile.z+3;
			Instantiate(normalTile,nextTile,normalTile.rotation);
		}

	}
}
