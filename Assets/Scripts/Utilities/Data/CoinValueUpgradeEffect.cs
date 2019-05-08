using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantasyErrand.Utilities
{
    [System.Serializable]
    public class CoinValueUpgradeEffect
    {
        /// <summary>
        /// The minimum distance required before silver coins appear. Set to <see cref="float.PositiveInfinity"/> when silver coins will not appear.
        /// </summary>
        public float SilverDistance { get; set; }
        /// <summary>
        /// The minimum distance required before gold coins appear. Set to <see cref="float.PositiveInfinity"/> when gold coins will not appear.
        /// </summary>
        public float GoldDistance { get; set; }
        /// <summary>
        /// The minimum distance required before platinum coins appear. Set to <see cref="float.PositiveInfinity"/> when platinum coins will not appear.
        /// </summary>
        public float PlatinumDistance { get; set; }
    }
}

