using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using FantasyErrand.Entities.Interfaces;

namespace FantasyErrand.Entities
{
    public class MovableCube : MonoBehaviour, IObstacleMovable
    {
        [SerializeField]
        public float moveSpeed = 2f;

        public float MoveSpeed
        {
            get
            {
                return moveSpeed;
            }
        }

        public float SpawnRate
        {
            get
            {
                return 15;
            }
        }

        public bool IsHurdling
        {
            get
            {
                return false;
            }
        }

        public void DoMove()
        {
            transform.Translate(new Vector3(0, 0, moveSpeed) * Time.deltaTime);
        }
    }
}
