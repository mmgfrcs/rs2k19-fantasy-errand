using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using FantasyErrand.Entities.Interfaces;
public class ObstacleGeneral : MonoBehaviour,IObstacleMovable,IObstacleRotatable {

	// Use this for initialization


	[SerializeField]
	float moveSpeed;
	public int minDistance;
	public GameObject player;
	[SerializeField]
	private float minSpeed;
	[SerializeField]
	private float spawnRate;
	[SerializeField]
	private bool isHurdling;
	[SerializeField]
	private Vector3 rotateSpeed= new Vector3(-180f,0f,0f);
	public GameObject movableObject;
	public bool isStaticObstacle;
	public bool isRotatableObstacle;
	public bool isMovableObstacle;

	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
	}
	
	// Update is called once per frame
	void Update () {
		if (isRotatableObstacle)
			DoRotate();
		if (isMovableObstacle)
			DoMove ();
	}

	public Vector3 RotateSpeed
	{
		get
		{
			return rotateSpeed;
		}
	}

	public float MoveSpeed
	{
		get
		{
			return moveSpeed;
		}
	}
	public bool IsHurdling
	{
		get{return isHurdling;}
	}
	
	public float SpawnRate
	{
		get
		{
			return spawnRate;
		}
	}


	public void DoRotate()
	{
		//transform.Rotate (rotateSpeed * Time.deltaTime);
		movableObject.transform.Rotate (rotateSpeed * Time.deltaTime);
	}

	public void DoMove()
	{
		movableObject.transform.Translate (-Vector3.forward * minSpeed * Time.deltaTime, Space.World);
		if (minSpeed < moveSpeed)
			minSpeed++;
	}
}
