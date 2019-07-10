using DG.Tweening;
using FantasyErrand.Entities;
using FantasyErrand.Utilities;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FantasyErrand
{
    public delegate void GameStartDelegate(bool restarted);
    public delegate void BaseGameEventDelegate();
    public delegate void GameEndDelegate(GameEndEventArgs args);
    

    public class GameManager : MonoBehaviour
    {
        [SerializeField] Player player;
        [SerializeField] GameUIManager UIManager;
        [SerializeField] EmotionManager emotionManager;
        [SerializeField] LevelManagerBase easyLevelManager, hardLevelManager, specialLevelManager;
        [SerializeField] float startSpeed;
        [SerializeField] int startingMultiplier = 10;

        AnimationCurve speedGraph;
        [SerializeField] AnimationCurve easySpeedGraph;
        [SerializeField] AnimationCurve normalSpeedGraph;
        [SerializeField] AnimationCurve hardSpeedGraph;
        public float Score { get; private set; }
        public float Distance { get; private set; }
        public float Currency { get; private set; }
        public int Multiplier { get; private set; }
        public bool IsGameRunning { get; private set; }
        public bool IsRollingStart { get; private set; }

        public float SpeedDistance { get { return Distance - restartDist; } }

        private float multiplierSpeed=1;
        private Rigidbody rb;
        
        public static event BaseGameEventDelegate OnGameRollingStart;
        public static event GameStartDelegate OnGameStart;
        public static event GameEndDelegate OnGameEnd;
        
        TextMeshProUGUI scoreText, debugText, coinsText;
        UnityEngine.UI.Image fader;
        float startTime;
        float restartDist = 0;
        internal LevelManagerBase levelManager;
        bool isPaused = false;
        int retryTimes = 0;

        internal float DynamicSpeedModifier=0;
        public SceneChanger changer;
        public void Start()
        {
            if (MainMenuManager.mainMenuDifficulty.Equals(Difficulty.Easy))
                speedGraph = easySpeedGraph;
            else if (MainMenuManager.mainMenuDifficulty.Equals(Difficulty.Special))
                speedGraph = normalSpeedGraph;
            else if (MainMenuManager.mainMenuDifficulty.Equals(Difficulty.Hard))
                speedGraph = hardSpeedGraph;

            if (MainMenuManager.mainMenuDifficulty == Difficulty.Easy) levelManager = easyLevelManager;
            else if (MainMenuManager.mainMenuDifficulty == Difficulty.Special) levelManager = specialLevelManager;
            else if (MainMenuManager.mainMenuDifficulty == Difficulty.Hard) levelManager = hardLevelManager;

            //Setup game
            rb = player.GetComponent<Rigidbody>();

            Player.OnCoinAdded += AddCurrency;
            PowerUpsManager.BoostEffectChanged += BoostEffect;
            UIManager.OnRetryGame += RetryGame;

            UIManager.OnRestartGame += () => {
                SoundManager.Instance.EndPlayBackSound();
                changer.ChangeScene(SceneManager.GetActiveScene().name);
                EndGameTruly();
            };

            UIManager.OnBackToMainMenu += ExitGame;

            Multiplier = startingMultiplier;
            scoreText = UIManager.GetUI<TextMeshProUGUI>(GameUIManager.UIType.ScoreText);
            fader = UIManager.GetUI<UnityEngine.UI.Image>(GameUIManager.UIType.Fader);
            debugText = UIManager.GetUI<TextMeshProUGUI>(GameUIManager.UIType.DebugText);
            coinsText = UIManager.GetUI<TextMeshProUGUI>(GameUIManager.UIType.CoinsText);
            SoundManager.Instance.playBackSound();
            StartGame();
        }

        private void BoostEffect(bool active, float duration, float multiplier)
        {
            OnBoost(active ? multiplier : 1);
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
                SoundManager.Instance.PlaySound("Collision");
                rb.constraints = RigidbodyConstraints.FreezeAll;
                OnGameEnd?.Invoke(new GameEndEventArgs() { IsEnded = false });
                Camera.main.GetComponent<Animator>().enabled = false;
                Camera.main.transform.DOPunchPosition(Vector3.up * 0.1f, 0.5f, 30);
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
            OnGameStart?.Invoke(false);
           Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Obstacles"), false);
        }

        IEnumerator EndGame()
        {
            SoundManager.Instance.PlaySound("Bite");
            yield return new WaitForSeconds(1.5f);
            Debug.Log("Game Over");

            UIManager.GetUI<TextMeshProUGUI>(GameUIManager.UIType.GameOverScore).text = Score.ToString("n0");
            UIManager.GetUI<TextMeshProUGUI>(GameUIManager.UIType.GameOverDistance).text = Distance.ToString("n0");
            UIManager.GetUI<TextMeshProUGUI>(GameUIManager.UIType.GameOverCoins).text = Currency.ToString("n0");
            UIManager.GetUI<TextMeshProUGUI>(GameUIManager.UIType.GameOverMultiplier).text = "x" + Multiplier.ToString("n0");

            LivesUpgradeEffect livesUpgrade = GameDataManager.instance.UpgradeEffects.LivesUpgrades[GameDataManager.instance.Data.UpgradeLevels.LivesLevel];
            float cost = livesUpgrade.ContinueCoinCost + (livesUpgrade.ContinueCoinCost * (livesUpgrade.ContinueCostMultiplier - 1) * retryTimes);
            UIManager.ActivateGameOver(cost);

        }

        public void Update()
        {
            Affdex.Emotions[] emo = { Affdex.Emotions.Disgust };
            debugText.text = $"{GameDataManager.instance.PlayerName}\nTravelling {Distance.ToString("n0")}m at {GetCurrSpeed().ToString("n2")} m/s, {Currency.ToString("n0")} coins\nIgnore Obstacle: {Physics.GetIgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Obstacles"))}\nRates: T {levelManager.tileSpawnRates.baseTile.Evaluate(player.transform.position.z).ToString("n2")}, O {levelManager.tileSpawnRates.obstacleTile.Evaluate(player.transform.position.z).ToString("n2")}, C {levelManager.tileSpawnRates.coinsTile.Evaluate(player.transform.position.z).ToString("n2")}, P {levelManager.tileSpawnRates.powerupsTile.Evaluate(player.transform.position.z).ToString("n2")}\nCoin Mod: {levelManager.coinMod.ToString("n2")}, Obs. Mod: {levelManager.obstacleMod.ToString("n2")}\n PosEmo:{DynamicLevelManager.totalPosEmotions.ToString("n2")},NegEmo:{DynamicLevelManager.totalNegEmotions.ToString("n2")}\nEmomanager :{DynamicLevelManager.emoStatus},NegEmo:{(DynamicLevelManager.totalNegEmotions- levelManager.getNeutral(emo)).ToString("n2")},NegDif:{(DynamicLevelManager.totalNegEmotions-levelManager.getNeutral(emo)).ToString("n2")}";

            if (IsRollingStart || IsGameRunning)
            {
                Score += player.speed * Time.deltaTime * Multiplier;
                Distance += player.speed * Time.deltaTime;
                if (IsGameRunning) player.speed = multiplierSpeed * (speedGraph.Evaluate(Distance)+DynamicSpeedModifier);
            }

            if (scoreText != null) scoreText.text = Score.ToString("n0");
            if (coinsText != null) coinsText.text = Currency.ToString("n0");
        }
        public void OnBoost(float multiplier)
        {
            StartCoroutine(Boost(multiplier));
            //multiplierSpeed= multiplier;
        }

        public IEnumerator Boost(float multiplier)
        {
            var tween = DOTween.To(() => multiplierSpeed, x => multiplierSpeed = x, multiplier, 1f);
            yield return tween.WaitForCompletion();
        }

        public float GetCurrSpeed()
        {
            return multiplierSpeed * (speedGraph.Evaluate(SpeedDistance) + DynamicSpeedModifier);
        }

        public void AddCurrency(int value)
        {
            Currency += value;
            Score += value * Multiplier;
        }

        public void RetryGame()
        {
            LivesUpgradeEffect livesUpgrade = GameDataManager.instance.UpgradeEffects.LivesUpgrades[GameDataManager.instance.Data.UpgradeLevels.LivesLevel];
            float cost = livesUpgrade.ContinueCoinCost + (livesUpgrade.ContinueCoinCost * (livesUpgrade.ContinueCostMultiplier - 1) * retryTimes++);
            GameDataManager.instance.Data.Coins -= Mathf.RoundToInt(cost);

            restartDist = Distance;
            player.transform.rotation = Quaternion.identity;
            UIManager.DeactivateGameOver();
            OnGameStart?.Invoke(true);
            player.enabled = true;
            rb.constraints = RigidbodyConstraints.None;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            IsGameRunning = true;
        }

        /// <summary>
        /// Called when the game ends by restarting/going to the main menu
        /// </summary>
        public void EndGameTruly()
        {
            OnGameEnd?.Invoke(new GameEndEventArgs() { IsEnded = true, Score = Score, Distance = Distance, Currency = Currency, Multiplier = Multiplier });
            OnGameEnd = null;
            OnGameStart = null;
            OnGameRollingStart = null;
        }

        public void ExitGame()
        {
            SoundManager.Instance.EndPlayBackSound();
            changer.ChangeScene("Main");
            EndGameTruly();
        }

        public void PauseGame()
        {
            if (isPaused)
            {
                Time.timeScale = 1;
                isPaused = false;
            }
            else
            {
                Time.timeScale = 0;
                isPaused = true;
            }
        }
    }

    public class GameEndEventArgs
    {
        public bool IsEnded { get; set; }
        public float Score { get; set; }
        public float Distance { get; set; }
        public float Currency { get; set; }
        public int Multiplier { get; set; }
    }

    
}
