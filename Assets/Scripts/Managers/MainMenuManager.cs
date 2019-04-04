using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using FantasyErrand.Entities.Interfaces;
using System.Linq;
using FantomLib;
using System;
using System.IO;

namespace FantasyErrand
{
    public class MainMenuManager : MonoBehaviour
    {
        public Slider loadingSlider;
        public CanvasGroup optionsPanel;
        public Image neutralImage, happyImage;
        public GameObject researchModeFrame, basicInfoFrame, expressionFrame;

        public Image fader;

        bool isPanelOpen = false;

        int pickMode = 0;
        
        public void OnTakeNeutralPicture()
        {
            pickMode = 0;
            NativeCamera.TakePicture(OnLoadImage);
        }

        public void OnTakeHappyPicture()
        {
            pickMode = 1;
            NativeCamera.TakePicture(OnLoadImage);
        }

        private void OnLoadImage(string path)
        {
            AndroidPlugin.ShowToast("Loading Image...");
            StartCoroutine(LoadImage(path, pickMode == 0 ? neutralImage : happyImage));
        }

        IEnumerator LoadImage(string path, Image image)
        {
            var properties = NativeCamera.GetImageProperties(path);
            yield return null;
            if (properties.width < properties.height)
            {
                fader.gameObject.SetActive(true);
                fader.color = new Color(0, 0, 0, 0.5f);

                byte[] bytes = File.ReadAllBytes(path);
                yield return null;

                Texture2D t2d = new Texture2D(properties.width, properties.height);
                t2d.LoadImage(bytes);
                yield return null;
                t2d.Apply();
                if (properties.orientation != NativeCamera.ImageOrientation.Normal && properties.orientation != NativeCamera.ImageOrientation.Unknown)
                    t2d = t2d.RotateImage();
                yield return null;

                image.rectTransform.sizeDelta = new Vector2(t2d.width * 432f / t2d.height, 432);
                image.sprite = Sprite.Create(t2d, new Rect(0, 0, t2d.width, t2d.height), Vector2.zero);
                AndroidPlugin.ShowToast("Image Loaded");
                fader.DOFade(0f, 1f).onComplete = () =>
                    {
                        fader.gameObject.SetActive(false);
                    };
                yield return null;
               
            }
            else AndroidPlugin.ShowToast("Not a portrait image");
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
            fader.gameObject.SetActive(true);
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
            fader.DOFade(0f, 2f).onComplete = () => {
                fader.gameObject.SetActive(false);
            };
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

