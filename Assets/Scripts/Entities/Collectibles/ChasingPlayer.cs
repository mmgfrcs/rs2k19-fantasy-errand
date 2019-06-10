using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantasyErrand.Entities
{
    public class ChasingPlayer : MonoBehaviour
    {
        public GameObject player;
        Vector3 pos;
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            pos = player.transform.position;
            transform.position = new Vector3(pos.x, pos.y, pos.z-5);
        }
    }
}