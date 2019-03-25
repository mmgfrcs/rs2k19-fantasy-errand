using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace FantasyErrand
{
    public class MainMenuManager : MonoBehaviour
    {
        public CanvasGroup optionsPanel;

        bool isPanelOpen = false;

        public void OnPlay()
        {
            SceneManager.LoadSceneAsync("SampleScene");
        }

        public void OnOptions()
        {
            if(isPanelOpen)
            {
                optionsPanel.DOFade(0f, 1f).onComplete += () => {
                    optionsPanel.blocksRaycasts = false;
                };
            }
            else
            {
                optionsPanel.blocksRaycasts = true;
                optionsPanel.DOFade(1f, 1f);
            }
            isPanelOpen = !isPanelOpen;
        }

        // Use this for initialization
        void Start()
        {
            GameObject panel = new GameObject("Fader", typeof(Image));
            panel.transform.SetParent(optionsPanel.transform.parent);
            RectTransform panelTransform = panel.GetComponent<RectTransform>();
            panelTransform.anchorMin = Vector2.zero;
            panelTransform.anchorMax = Vector2.one;
            panelTransform.sizeDelta = Vector2.zero;
            Image img = panel.GetComponent<Image>();
            img.color = Color.black;
            img.DOColor(Color.clear, 2f).onComplete = () => {
                img.gameObject.SetActive(false);
            };
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

