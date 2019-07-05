using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FantasyErrand.Entities
{
    public class MovableCube : ObstacleBase
    {
        [SerializeField]
        public float moveSpeed = 2f;

        public void DoMove()
        {
            transform.Translate(new Vector3(0, 0, moveSpeed) * Time.deltaTime);
        }
    }
}
