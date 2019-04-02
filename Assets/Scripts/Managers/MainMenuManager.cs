using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using FantasyErrand.Entities.Interfaces;
using System.Linq;
using FantomLib;

namespace FantasyErrand
{
    public class MainMenuManager : MonoBehaviour
    {
        public Slider loadingSlider;
        public CanvasGroup optionsPanel;
        public Image neutralImage, happyImage;
        public GalleryPickController picker;
        public ToastController toast;

        bool isPanelOpen = false;
        GameObject fader;

        int pickMode = 0;

        public void OnTakeNeutralPicture()
        {
            pickMode = 0;
            picker.Show();
        }

        public void OnResult(string data, int x, int y)
        {
            print($"{data} - {x} - {y}");
        }

        public void OnImageSelected(ImageInfo info)
        {
            print("Image selected - " + info);
            if(info.width < info.height)
            {
                float aspect = (float)info.width / info.height;
                if (aspect <= 9f / 16f)
                {
                    print(info.fileUri);
                    print(info.uri);
                    print(info.path);
                    print(info.name);
                    print(info.mimeType);

                }
                else toast.Show($"Aspect Ratio Error, expected 9:16 or lower, found {(aspect * 16).ToString("n1")}:16");
            }
            else toast.Show("Image must be portrait in orientation");
        }

        public void OnError(string err)
        {
            print(err);
        }

        IEnumerator LoadScene()
        {
            loadingSlider.maxValue = 0.9f;
            var scene = SceneManager.LoadSceneAsync("SampleScene");
            scene.allowSceneActivation = false;
            do
            {
                loadingSlider.value = scene.progress;
                yield return null;
            }
            while (scene.progress < 0.9f);
            fader.SetActive(true);
            var tween = fader.GetComponent<Image>().DOFade(1f, 2f);
            yield return tween.WaitForCompletion();
            scene.allowSceneActivation = true;
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
            fader = new GameObject("Fader", typeof(Image));
            fader.transform.SetParent(optionsPanel.transform.parent);
            RectTransform panelTransform = fader.GetComponent<RectTransform>();
            panelTransform.anchorMin = Vector2.zero;
            panelTransform.anchorMax = Vector2.one;
            panelTransform.sizeDelta = Vector2.zero;
            Image img = fader.GetComponent<Image>();
            img.color = Color.black;
            img.DOFade(0f, 2f).onComplete = () => {
                img.gameObject.SetActive(false);
            };
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

