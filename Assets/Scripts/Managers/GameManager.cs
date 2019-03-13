using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace FantasyErrand
{
    public delegate void GameEventDelegate();
    public class GameManager : MonoBehaviour
    {
        public float Score { get; private set; }
        public float Distance { get; private set; }
        public float Currency { get; private set; }

        public static event GameEventDelegate OnGameStart;
        public static event GameEventDelegate OnGameEnd;

        public void Start()
        {
            //Setup game
            Score = 0;
            Distance = 0;
            Currency = 0;
        }

        public void Update()
        {
            
        }
    }
}
