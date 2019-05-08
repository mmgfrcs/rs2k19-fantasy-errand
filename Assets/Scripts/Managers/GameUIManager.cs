using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FantasyErrand
{
    public class GameUIManager : MonoBehaviour
    {
        public CanvasGroup gameOverPanel;
        List<GameUIElement> elements;

        public event BaseGameEventDelegate OnRetryGame;
        public event BaseGameEventDelegate OnRestartGame;
        public event BaseGameEventDelegate OnBackToMainMenu;

        public enum UIType
        {
            ScoreText, InfoText, DebugText, PauseBtn, PowerupSlider, PowerupImage, Fader, GameOverScore, GameOverCoins, GameOverDistance, GameOverMultiplier
        }

        // Use this for initialization
        void Start()
        {
            if (gameOverPanel == null) gameOverPanel = GameObject.Find("GameOver").GetComponent<CanvasGroup>();
            gameOverPanel.alpha = 0;
            gameOverPanel.blocksRaycasts = false;
            GetAllUIElements();
        }

        public T GetUI<T>(UIType type) where T : MonoBehaviour
        {
            Component text = elements.Find(x => x.type == type).obj;
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

        private void GetAllUIElements()
        {
            elements = new List<GameUIElement>();
            var sceneElements = FindObjectsOfType<MonoBehaviour>();
            foreach (var graphicElement in sceneElements)
            {
                if (graphicElement is TextMeshProUGUI && graphicElement.gameObject.name.Contains("GameOverScore"))
                    elements.Add(new GameUIElement(UIType.GameOverScore, graphicElement));
                else if(graphicElement is TextMeshProUGUI && graphicElement.gameObject.name.Contains("GameOverCoins"))
                    elements.Add(new GameUIElement(UIType.GameOverCoins, graphicElement));
                else if(graphicElement is TextMeshProUGUI && graphicElement.gameObject.name.Contains("GameOverDistance"))
                    elements.Add(new GameUIElement(UIType.GameOverDistance, graphicElement));
                else if(graphicElement is TextMeshProUGUI && graphicElement.gameObject.name.Contains("GameOverMultiplier"))
                    elements.Add(new GameUIElement(UIType.GameOverMultiplier, graphicElement));
                else if(graphicElement is TextMeshProUGUI && graphicElement.gameObject.name == "ScoreText")
                    elements.Add(new GameUIElement(UIType.ScoreText, graphicElement));
                else if (graphicElement is TextMeshProUGUI && graphicElement.gameObject.name.Contains("Info"))
                    elements.Add(new GameUIElement(UIType.InfoText, graphicElement));
                else if (graphicElement is TextMeshProUGUI && graphicElement.gameObject.name.Contains("Debug"))
                    elements.Add(new GameUIElement(UIType.DebugText, graphicElement));
                else if (graphicElement is Image && graphicElement.gameObject.name.Contains("Fader"))
                    elements.Add(new GameUIElement(UIType.Fader, graphicElement));
                else if (graphicElement is Image && graphicElement.gameObject.name.Contains("Powerup"))
                    elements.Add(new GameUIElement(UIType.PowerupImage, graphicElement));
                else if (graphicElement is Slider && graphicElement.gameObject.name.Contains("Powerup"))
                    elements.Add(new GameUIElement(UIType.PowerupSlider, graphicElement));
            }
            print($"UI Manager: {sceneElements.Length} Scripts detected, { elements.Count } UI elements detected");
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

    [System.Serializable]
    public class GameUIElement
    {
        public GameUIManager.UIType type;
        public MonoBehaviour obj;

        public GameUIElement(GameUIManager.UIType type, MonoBehaviour obj)
        {
            this.type = type;
            this.obj = obj;
        }
    }
}
