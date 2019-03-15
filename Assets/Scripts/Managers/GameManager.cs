using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using FantasyErrand.Entities;

namespace FantasyErrand
{
    public delegate void BaseGameEventDelegate();
    public delegate void GameEndDelegate(GameEndEventArgs args);

    public class GameManager : MonoBehaviour
    {

        public Player player;
        public GameUIManager UIManager;

        public float Score { get; private set; }
        public float Distance { get; private set; }
        public float Currency { get; private set; }
        public int Multiplier { get; private set; } = 10;
        public bool IsGameRunning { get; private set; }
        public bool IsRollingStart { get; private set; }

        public static event BaseGameEventDelegate OnGameRollingStart;
        public static event BaseGameEventDelegate OnGameStart;
        public static event GameEndDelegate OnGameEnd;

        TextMeshProUGUI scoreText;

        public void Start()
        {
            //Setup game
            Application.targetFrameRate = 60;
            Score = 0;
            Distance = 0;
            Currency = 0;
            scoreText = UIManager.GetUI<TextMeshProUGUI>(GameUIManager.UIType.ScoreText);
            
            StartCoroutine(RollingStart());
        }

        IEnumerator RollingStart()
        {
            IsRollingStart = true;
            IsGameRunning = false;
            player.IsControlActive = false;
            OnGameRollingStart?.Invoke();
            yield return new WaitForSeconds(5f);
            IsRollingStart = false;
            IsGameRunning = true;
            player.IsControlActive = true;
            OnGameStart?.Invoke();
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Obstacles"), false);
        }

        public void Update()
        {
            if(IsRollingStart || IsGameRunning) Score += player.speed * Time.deltaTime * Multiplier;
            
            scoreText.text = Score.ToString("n0");
        }
    }

    public class GameEndEventArgs
    {
        public float Score { get; set; }
        public float Distance { get; set; }
        public float Currency { get; set; }
        public int Multiplier { get; set; }
    }
}
