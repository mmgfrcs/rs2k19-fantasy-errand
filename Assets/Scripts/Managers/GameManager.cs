using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

using FantasyErrand.Entities;
using FantasyErrand.Entities.Interfaces;

namespace FantasyErrand
{
    public delegate void BaseGameEventDelegate();
    public delegate void GameEndDelegate(GameEndEventArgs args);

    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        Player player;
        [SerializeField]
        GameUIManager UIManager;
        [SerializeField]
        float startSpeed;
        [SerializeField]
        AnimationCurve speedGraph;
        [SerializeField]
        int startingMultiplier = 10;

        public float Score { get; private set; }
        public float Distance { get; private set; }
        public float Currency { get; private set; }
        public int Multiplier { get; private set; }
        public bool IsGameRunning { get; private set; }
        public bool IsRollingStart { get; private set; }

        public static event BaseGameEventDelegate OnGameRollingStart;
        public static event BaseGameEventDelegate OnGameStart;
        public static event GameEndDelegate OnGameEnd;

        public static GameManager instance;

        TextMeshProUGUI scoreText;
        UnityEngine.UI.Image fader;
        float startTime;

        public void Start()
        {
            if (instance == null) instance = this;
            else { Destroy(this); return; }
            //Setup game
            Multiplier = startingMultiplier;
            scoreText = UIManager.GetUI<TextMeshProUGUI>(GameUIManager.UIType.ScoreText);
            fader = UIManager.GetUI<UnityEngine.UI.Image>(GameUIManager.UIType.Fader);
            StartGame();
        }

        internal void StartGame()
        {
            Score = 0;
            Distance = 0;
            Currency = 0;
            player.speed = startSpeed;
            Camera.main.GetComponent<Animator>().Play("StartPan", -1, 0f);
            player.OnCollision += Player_OnCollision;
            player.transform.position = Vector3.zero;
            player.transform.rotation = Quaternion.identity;
            player.enabled = true;

            StartCoroutine(RollingStart());
        }

        private void Player_OnCollision(Collision obj)
        {
            if (obj.collider.gameObject.layer == LayerMask.NameToLayer("Obstacles"))
            {
                OnGameEnd?.Invoke(new GameEndEventArgs() { Score = Score, Distance = Distance, Currency = Currency, Multiplier = Multiplier });
                Camera.main.GetComponent<Animator>().enabled = false;
                Camera.main.transform.DOPunchPosition(Vector3.up * 0.1f, 0.5f, 30);
                player.enabled = false;
                IsGameRunning = false;
                StartCoroutine(EndGame());
            }
            else {
                ICollectible collectible = obj.gameObject.GetComponent<ICollectible>();
                if (collectible != null)
                {
                    if (collectible.Type == CollectibleType.Monetary)
                        Currency += collectible.Value;
                    if (collectible.Type != CollectibleType.None)
                        collectible.CollectibleEffect();
                }
            }
        }

        IEnumerator RollingStart()
        {
            fader.DOFade(0f, 1f).onComplete = () => fader.gameObject.SetActive(false);
            IsRollingStart = true;
            IsGameRunning = false;
            player.IsControlActive = false;
            OnGameRollingStart?.Invoke();
            yield return new WaitForSeconds(5f);
            IsRollingStart = false;
            IsGameRunning = true;
            player.IsControlActive = true;
            startTime = Time.time;
            OnGameStart?.Invoke();
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Obstacles"), false);
        }

        IEnumerator EndGame()
        {
            yield return new WaitForSeconds(1.5f);
            Debug.Log("Game Over");
            UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
        }

        public void Update()
        {
            if (IsRollingStart || IsGameRunning)
            {
                Score += player.speed * Time.deltaTime * Multiplier;
                Distance += player.speed * Time.deltaTime;
                if (IsGameRunning) player.speed = speedGraph.Evaluate(Distance);
            }
            else if (!IsGameRunning && Input.GetMouseButtonDown(0)) StartGame();
            
            if(scoreText != null) scoreText.text = Score.ToString("n0");
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
