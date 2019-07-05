using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantasyErrand.Entities
{
    public class RotatingObstacle : ObstacleBase
    {
        [SerializeField]
        private float detectionRange;
        [SerializeField]
        private Vector3 rotateSpeed = new Vector3(-180f, 0f, 0f);
        [SerializeField]
        private float minSpeed;
        [SerializeField]
        private float moveSpeed;
        private GameObject player;

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


        // Use this for initialization
        protected override void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        // Update is called once per frame
        protected override void Update()
        {
            if (Vector3.Distance(player.transform.position, transform.position) < detectionRange)
            {
                DoRotate();
            }
        }
    }
}