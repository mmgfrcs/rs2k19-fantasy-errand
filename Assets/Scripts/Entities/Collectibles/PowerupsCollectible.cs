using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantasyErrand.Entities
{
    public class PowerupsCollectible : CollectibleBase
    {
        public PowerUpsType powerUpsType;

        protected override void Update()
        {
            base.Update();
            transform.Rotate(0, 90 * Time.deltaTime, 0);
        }
    }
}

