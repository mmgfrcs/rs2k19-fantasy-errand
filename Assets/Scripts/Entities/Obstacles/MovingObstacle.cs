﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FantasyErrand.Entities.Interfaces;

namespace FantasyErrand
{
    public class MovingObstacle : MonoBehaviour, IObstacleMovable
    {

        [SerializeField]
        float moveSpeed;
        [SerializeField]
        private bool isHurdling;
        [SerializeField]
        private float spawnRate;
        [SerializeField]
        private float minSpeed;
        public float detectionRange;
        private GameObject player;
        public float MoveSpeed
        {
            get
            {
                return moveSpeed;
            }
        }

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

        public void DoMove()
        {
            transform.Translate(-Vector3.forward * minSpeed * Time.deltaTime, Space.World);
            if (minSpeed < moveSpeed)
                minSpeed++;
        }

        // Use this for initialization
        void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        // Update is called once per frame
        void Update()
        {
            if (Vector3.Distance(player.transform.position, transform.position) < detectionRange)
            {
                transform.Translate(-Vector3.forward * minSpeed * Time.deltaTime, Space.World);
                if (minSpeed < moveSpeed)
                    minSpeed++;
            }
        }
    }

}
