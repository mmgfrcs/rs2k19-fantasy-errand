using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FantasyErrand.Entities.Interfaces;
public class MovingObstacle : MonoBehaviour,IObstacleMovable {

	[SerializeField]
	float moveSpeed;
	public int minDistance;
	public GameObject player;
	[SerializeField]
	private float minSpeed;
	[SerializeField]
	private float spawnRate;
	public GameObject movableObject;
	public float SpawnRate
	{
		get
		{
			return spawnRate;
		}
	}
	public float MoveSpeed
	{
		get
		{
			return moveSpeed;
		}
	}

	public void DoMove()
	{
		movableObject.transform.Translate (-Vector3.forward * minSpeed * Time.deltaTime, Space.World);
		if (minSpeed < moveSpeed)
			minSpeed++;
	}

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
	}
	
	// Update is called once per frame
	void Update () {
		if (Vector3.Distance (transform.position, player.transform.position) < minDistance) {
			DoMove ();
		}
	}
}
