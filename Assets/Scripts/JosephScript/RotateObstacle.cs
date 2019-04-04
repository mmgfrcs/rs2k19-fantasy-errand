using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FantasyErrand.Entities.Interfaces;
public class RotateObstacle : MonoBehaviour,IObstacleMovable,IObstacleRotatable {

	// Use this for initialization
	[SerializeField]
	private float spawnRate;
	[SerializeField]
	private float minSpeed;
	[SerializeField]
	private float moveSpeed;
	[SerializeField]
	private Vector3 rotateSpeed= new Vector3(-180f,0f,0f);
	[SerializeField]
	private bool isHurdling;
	public GameObject movableObject;


	public GameObject player;
	public int minDistance;
	void Start () 
	{
		player = GameObject.FindGameObjectWithTag ("Player");
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Vector3.Distance (player.transform.position, transform.position) < minDistance) {
			DoMove();
			DoRotate();
		}
	}
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
		//transform.Translate (-Vector3.forward * moveSpeed * Time.deltaTime, Space.World);
		movableObject.transform.Translate (-Vector3.forward * minSpeed * Time.deltaTime, Space.World);
		if (minSpeed < moveSpeed)
				minSpeed++;
	}
	
	public Vector3 RotateSpeed
	{
		get
		{
			return rotateSpeed;
		}
	}

	
	public void DoRotate()
	{
		//transform.Rotate (rotateSpeed * Time.deltaTime);
		movableObject.transform.Rotate (rotateSpeed * Time.deltaTime);
	}

	public bool IsHurdling{
		get
		{
			return isHurdling;
		}
	}
}
