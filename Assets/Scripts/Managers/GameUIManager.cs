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
        [SerializeField] private Image fader;
        [SerializeField] private Image[] powerUpsImage;
        [SerializeField] private Slider[] powerUpsSlider;
        [SerializeField] private Button pauseBtn;

        [Header("Game Over UI"), SerializeField] private TextMeshProUGUI gameOverScore;
        [SerializeField] private TextMeshProUGUI gameOverCoins, gameOverDistance, gameOverMultiplier;
        [SerializeField] private CanvasGroup gameOverPanel;

        public event BaseGameEventDelegate OnRetryGame;
        public event BaseGameEventDelegate OnRestartGame;
        public event BaseGameEventDelegate OnBackToMainMenu;

        public enum UIType
        {
            ScoreText, DebugText, CoinsText, PauseBtn, PowerupSliderArray, PowerupImageArray, Fader, GameOverScore, GameOverCoins, GameOverDistance, GameOverMultiplier, GameOverPanel
        }

        // Use this for initialization
        void Start()
        {
            if (gameOverPanel == null) gameOverPanel = GameObject.Find("GameOver").GetComponent<CanvasGroup>();
            gameOverPanel.alpha = 0;
            gameOverPanel.blocksRaycasts = false;
        }

        public T[] GetUIArray<T>(UIType type) where T: MonoBehaviour
        {
            switch (type)
            {
                case UIType.CoinsText:
                case UIType.DebugText:
                case UIType.Fader:
                case UIType.GameOverCoins:
                case UIType.GameOverDistance:
                case UIType.GameOverMultiplier:
                case UIType.GameOverPanel:
                case UIType.GameOverScore:
                case UIType.ScoreText:
                case UIType.PauseBtn:
                    {
                        throw new System.ArgumentException("Cannot get singular component through this function. Use GetUI() instead");
                    }
                case UIType.PowerupImageArray:
                    {
                        if (powerUpsImage is T[]) return powerUpsImage as T[];
                        else throw new System.ArgumentException("UI Type is not valid for the given generic type");
                    }
                case UIType.PowerupSliderArray:
                    {
                        if (powerUpsSlider is T[]) return powerUpsSlider as T[];
                        else throw new System.ArgumentException("UI Type is not valid for the given generic type");
                    }
                default:
                    {
                        return null;
                    }
            }
        }

        public T GetUI<T>(UIType type) where T: MonoBehaviour
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
                case UIType.PowerupImageArray:
                case UIType.PowerupSliderArray:
                    {
                        throw new System.ArgumentException("Cannot get array through this function. Use GetUIArray() instead");
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
