using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantasyErrand.Utilities
{
    [System.Serializable]
    public class LivesUpgradeEffect
    {
        /// <summary>
        /// The cost to continue, in coins
        /// </summary>
        public float ContinueCoinCost { get; set; }
        /// <summary>
        /// The multiplier applied to the cost for each subsequent continues
        /// </summary>
        public float ContinueCostMultiplier { get; set; }
        /// <summary>
        /// How many hurdles can the player pass before receiving a game over
        /// </summary>
        public int HurdleLives { get; set; }
    }
}

