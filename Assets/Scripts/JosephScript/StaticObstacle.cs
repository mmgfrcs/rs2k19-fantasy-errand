using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FantasyErrand.Entities.Interfaces;
public class StaticObstacle : MonoBehaviour,IObstacle {
	[SerializeField]
	private float spawnRate;
	[SerializeField]
	private bool isHurdling;

	public bool IsHurdling
	{
		get
		{
			return isHurdling;
		}
	}
	public float SpawnRate 
	{
		get
		{
			return spawnRate;
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
