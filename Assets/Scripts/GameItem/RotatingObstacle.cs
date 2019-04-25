using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FantasyErrand.Entities.Interfaces;

namespace FantasyErrand
{
    public class RotatingObstacle : MonoBehaviour, IObstacleRotatable
    {

        [SerializeField]
        private float spawnRate;
        [SerializeField]
        private bool isHurdling;
        [SerializeField]
        private float detectionRange;
        [SerializeField]
        private Vector3 rotateSpeed = new Vector3(-180f, 0f, 0f);
        [SerializeField]
        private float minSpeed;
        [SerializeField]
        private float moveSpeed;
        private GameObject player;
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

        public Vector3 RotateSpeed
        {
            get
            {
                return rotateSpeed;
            }
        }

        public void DoRotate()
        {
            transform.Rotate(rotateSpeed * Time.deltaTime);
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
                DoRotate();
            }
        }
    }
}