using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;
using FantasyErrand.Entities;
using FantasyErrand.Entities.Interfaces;
using Firebase.Analytics;

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
        
        TextMeshProUGUI scoreText;
        UnityEngine.UI.Image fader;
        float startTime;

        public void Start()
        {
            //Setup game
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLevelStart, new Parameter("level", "Easy"));
            FirebaseAnalytics.SetCurrentScreen("EasyGame", "In-Game");

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
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Obstacles"));
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
            UIManager.OnRestartGame += () => {
                fader.gameObject.SetActive(true);
                fader.DOFade(1f, 2f).onComplete = () => SceneManager.LoadScene("SampleScene");
            };

            UIManager.OnBackToMainMenu += () =>
            {
                fader.gameObject.SetActive(true);
                fader.DOFade(1f, 2f).onComplete = () => SceneManager.LoadScene("Main");
            };

            UIManager.GetUI<TextMeshProUGUI>(GameUIManager.UIType.GameOverScore).text = Score.ToString("n0");
            UIManager.GetUI<TextMeshProUGUI>(GameUIManager.UIType.GameOverDistance).text = Distance.ToString("n0");
            UIManager.GetUI<TextMeshProUGUI>(GameUIManager.UIType.GameOverCoins).text = Currency.ToString("n0");
            UIManager.GetUI<TextMeshProUGUI>(GameUIManager.UIType.GameOverMultiplier).text = "x" + Multiplier.ToString("n0");

            UIManager.ActivateGameOver();
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLevelEnd,
                new Parameter("level", "Easy"),
                new Parameter("score", Score),
                new Parameter("coins", Currency),
                new Parameter("distance", Distance),
                new Parameter("multiplier", Multiplier));
        }

        public void Update()
        {
            if (IsRollingStart || IsGameRunning)
            {
                Score += player.speed * Time.deltaTime * Multiplier;
                Distance += player.speed * Time.deltaTime;
                if (IsGameRunning) player.speed = speedGraph.Evaluate(Distance);
            }
            
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
