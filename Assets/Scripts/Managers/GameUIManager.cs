using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FantasyErrand
{
    public class GameUIManager : MonoBehaviour
    {
        [Header("General UI"), SerializeField] private TextMeshProUGUI debugText;
        [SerializeField] private TextMeshProUGUI scoreText, coinsText;
        [SerializeField] private Image fader, powerUpsImage;
        [SerializeField] private Slider powerUpsSlider;
        [SerializeField] private Button pauseBtn;

        [Header("Game Over UI"), SerializeField] private TextMeshProUGUI gameOverScore;
        [SerializeField] private TextMeshProUGUI gameOverCoins, gameOverDistance, gameOverMultiplier;
        [SerializeField] private CanvasGroup gameOverPanel;

        public event BaseGameEventDelegate OnRetryGame;
        public event BaseGameEventDelegate OnRestartGame;
        public event BaseGameEventDelegate OnBackToMainMenu;

        public enum UIType
        {
            ScoreText, DebugText, CoinsText, PauseBtn, PowerupSlider, PowerupImage, Fader, GameOverScore, GameOverCoins, GameOverDistance, GameOverMultiplier, GameOverPanel
        }

        // Use this for initialization
        void Start()
        {
            if (gameOverPanel == null) gameOverPanel = GameObject.Find("GameOver").GetComponent<CanvasGroup>();
            gameOverPanel.alpha = 0;
            gameOverPanel.blocksRaycasts = false;
        }

        public T GetUI<T>(UIType type) where T : MonoBehaviour
        {
            Component text;

            switch (type)
            {
                case UIType.CoinsText:
                    {
                        text = coinsText;
                        break;
                    }
                case UIType.DebugText:
                    {
                        text = debugText;
                        break;
                    }
                case UIType.Fader:
                    {
                        text = fader;
                        break;
                    }
                case UIType.GameOverCoins:
                    {
                        text = gameOverCoins;
                        break;
                    }
                case UIType.GameOverDistance:
                    {
                        text = gameOverDistance;
                        break;
                    }
                case UIType.GameOverMultiplier:
                    {
                        text = gameOverMultiplier;
                        break;
                    }
                case UIType.GameOverPanel:
                    {
                        text = gameOverPanel;
                        break;
                    }
                case UIType.GameOverScore:
                    {
                        text = gameOverScore;
                        break;
                    }
                case UIType.PauseBtn:
                    {
                        text = pauseBtn;
                        break;
                    }
                case UIType.PowerupImage:
                    {
                        text = powerUpsImage;
                        break;
                    }
                case UIType.PowerupSlider:
                    {
                        text = powerUpsSlider;
                        break;
                    }
                case UIType.ScoreText:
                    {
                        text = scoreText;
                        break;
                    }
                default:
                    {
                        text = null;
                        break;
                    }
            }

            if (text is T) return text as T;
            else throw new System.ArgumentException("UI Type is not valid for the given generic type");
        }

        public void ActivateGameOver()
        {
            gameOverPanel.blocksRaycasts = true;
            gameOverPanel.DOFade(1f, 1f);
        }

        public void DeactivateGameOver()
        {
            gameOverPanel.blocksRaycasts = false;
            gameOverPanel.alpha = 0;
        }

        public void OnRetry()
        {
            OnRetryGame?.Invoke();
        }
        
        public void OnRestart()
        {
            OnRestartGame?.Invoke();
        }

        public void OnMainMenu()
        {
            OnBackToMainMenu?.Invoke();
        }
    }
}
