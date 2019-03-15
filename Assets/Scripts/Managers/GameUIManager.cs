using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FantasyErrand
{
    public class GameUIManager : MonoBehaviour
    {
        List<GameUIElement> elements;

        public enum UIType
        {
            ScoreText, InfoText, DebugText, PauseBtn, PowerupSlider, PowerupImage
        }

        // Use this for initialization
        void Start()
        {
            GetAllUIElements();
        }

        public T GetUI<T>(UIType type) where T : MonoBehaviour
        {
            Component text = elements.Find(x => x.type == type).obj;
            if (text is T) return text as T;
            else throw new System.ArgumentException("UI Type is not valid for the given generic type");
        }

        private void GetAllUIElements()
        {
            elements = new List<GameUIElement>();
            var sceneElements = FindObjectsOfType<MonoBehaviour>();
            foreach (var graphicElement in sceneElements)
            {
                if (graphicElement is TextMeshProUGUI && graphicElement.gameObject.name.Contains("Score"))
                    elements.Add(new GameUIElement(UIType.ScoreText, graphicElement));
                else if (graphicElement is TextMeshProUGUI && graphicElement.gameObject.name.Contains("Info"))
                    elements.Add(new GameUIElement(UIType.InfoText, graphicElement));
                else if (graphicElement is TextMeshProUGUI && graphicElement.gameObject.name.Contains("Debug"))
                    elements.Add(new GameUIElement(UIType.DebugText, graphicElement));
                else if (graphicElement is Image && graphicElement.gameObject.name.Contains("Powerup"))
                    elements.Add(new GameUIElement(UIType.PowerupImage, graphicElement));
                else if (graphicElement is Slider && graphicElement.gameObject.name.Contains("Powerup"))
                    elements.Add(new GameUIElement(UIType.PowerupSlider, graphicElement));
            }
            print($"UI Manager: {sceneElements.Length} Scripts detected, { elements.Count } UI elements detected");
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
