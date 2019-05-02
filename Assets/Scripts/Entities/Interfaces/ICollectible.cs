using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FantasyErrand.Entities.Interfaces
{
    public enum CollectibleType
    {
        None, Monetary, Powerups
    }
    public interface ICollectible
    {
        CollectibleType Type { get; }
        int Value { get; }
        
        void CollectibleEffect();
    }
}
