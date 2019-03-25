using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantasyErrand.Utilities
{
    public class CameraMainMenuRotator : MonoBehaviour
    {
        public Transform target;
        public float speed;
        // Use this for initialization
        void Start()
        {
            transform.LookAt(target);
        }

        // Update is called once per frame
        void Update()
        {
            transform.RotateAround(target.position, Vector3.up, speed * Time.deltaTime);
        }
    }
}
