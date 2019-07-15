using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FantasyErrand.Entities
{
    public class MovingObstacle : ObstacleBase
    {

        [SerializeField]
        float moveSpeed;
        [SerializeField]
        private float minSpeed;
        public float detectionRange;
        private GameObject player;
        private bool startMove = true;
        public float MoveSpeed
        {
            get
            {
                return moveSpeed;
            }
        }

        public void DoMove()
        {
            transform.Translate(-Vector3.forward * minSpeed * Time.deltaTime, Space.World);
            if (minSpeed < moveSpeed)
                minSpeed++;
        }

        // Use this for initialization
        protected override void Start()
        {
            base.Start();
            player = GameObject.FindGameObjectWithTag("Player");
            GameManager.OnGameEnd += disableMove;
            GameManager.OnGameStart += enableMove;
        }

        void OnEnable()
        {
            startMove = true;
        }

        void disableMove(GameEndEventArgs args)
        {
            startMove = false;   
        }

        void enableMove(bool restarted)
        {
            startMove = true;
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
            if (Vector3.Distance(player.transform.position, transform.position) < detectionRange && startMove)
            {
                transform.Translate(-Vector3.forward * minSpeed * Time.deltaTime, Space.World);
                if (minSpeed < moveSpeed)
                    minSpeed++;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Player")
            {
                Vector3 curr = gameObject.transform.position;
                gameObject.transform.position = new Vector3(curr.x, curr.y, curr.z + 1);
            }
            
        }

    }

}
