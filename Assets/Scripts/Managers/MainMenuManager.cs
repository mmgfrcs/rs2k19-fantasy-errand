using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Firebase.Analytics;
using FantasyErrand.Entities.Interfaces;
using System.Linq;
using FantomLib;
using System;
using System.IO;
using TMPro;

namespace FantasyErrand
{
    public class MainMenuManager : MonoBehaviour
    {
        public Slider loadingSlider;
        public CanvasGroup optionsPanel;
        public GameObject researchModeFrame, basicInfoFrame, expressionFrame;
        public TextMeshProUGUI versionText;

        [Header("Option Fields")]
        public TMP_InputField[] serverAddress;
        public Toggle researchToggle, basicGatherToggle, expressionToggle;
        public TMP_InputField nameField, ageField;
        public Image neutralImage, happyImage;
        public Button backButton;

        Texture2D neutral, happy;

        public Image fader;

        bool isPanelOpen = false;

        int pickMode = 0;
        int errors = 0;

        public void OnPlay()
        {
            StartCoroutine(LoadScene());
        }
        
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

                if (pickMode == 0) neutral = t2d;
                else happy = t2d;
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
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLevelStart, new Parameter("level", "Easy"));
            scene.allowSceneActivation = true;
        }

        public void OnOptions()
        {
            if(isPanelOpen)
            {
                FirebaseAnalytics.SetCurrentScreen("Main menu", "System");
                print($"GameData - Saving Data: {nameField.text}, Age {ageField.text}\nResearch Mode is {(researchToggle.isOn ? "on" : "off")} at 192.168.{serverAddress[0].text}.{serverAddress[1].text}");
                GameDataManager.instance.SaveParticipantData(nameField.text, ageField.text == "" ? 0 : int.Parse(ageField.text), neutral, happy);
                GameDataManager.instance.SaveBasicOptions(researchToggle.isOn, basicGatherToggle.isOn, 
                    expressionToggle.isOn, $"192.168.{serverAddress[0].text}.{serverAddress[1].text}");
                optionsPanel.DOFade(0f, 1f).onComplete += () => {
                    optionsPanel.blocksRaycasts = false;
                };
                GameDataManager.instance.SaveResearchDataToFile();
            }
            else
            {
                FirebaseAnalytics.SetCurrentScreen("Options", "System");
                optionsPanel.blocksRaycasts = true;
                optionsPanel.DOFade(1f, 1f);
                nameField.text = GameDataManager.instance.PlayerName;
                ageField.text = GameDataManager.instance.Age.ToString();
                string[] addr = GameDataManager.instance.ServerAddress.Split('.');
                serverAddress[0].text = addr[2];
                serverAddress[1].text = addr[3];
                researchToggle.isOn = GameDataManager.instance.ResearchMode;
                basicGatherToggle.isOn = GameDataManager.instance.BasicGathering;
                expressionToggle.isOn = GameDataManager.instance.ExpressionGathering;
                neutral = GameDataManager.instance.NeutralPicture;
                if (neutral != null) neutralImage.sprite = Sprite.Create(neutral, new Rect(0, 0, neutral.width, neutral.height), Vector2.zero);
                happy = GameDataManager.instance.HappyPicture;
                if (happy != null) happyImage.sprite = Sprite.Create(happy, new Rect(0, 0, happy.width, happy.height), Vector2.zero);
                print($"GameData - Loading Data: {GameDataManager.instance.PlayerName}, Age {GameDataManager.instance.Age}\nResearch Mode is {(GameDataManager.instance.ResearchMode ? "on" : "off")} at {GameDataManager.instance.ServerAddress}");
            }
            isPanelOpen = !isPanelOpen;
        }

        public void OnResearchModeToggle(bool state)
        {
            if (state) researchModeFrame.SetActive(true);
            else researchModeFrame.SetActive(false);
        }

        public void OnBasicGatheringToggle(bool state)
        {
            if (state) basicInfoFrame.SetActive(true);
            else basicInfoFrame.SetActive(false);
        }

        public void OnExpressionGatheringToggle(bool state)
        {
            if (state) expressionFrame.SetActive(true);
            else expressionFrame.SetActive(false);
        }

        public void CheckNameEmpty(string text)
        {
            if (text == "") errors++;
            else errors--;

            if (errors > 0) backButton.enabled = false;
            else backButton.enabled = true;
        }

        public void CheckAgeEmpty(string text)
        {
            if (text == "") ageField.text = "0";
        }

        // Use this for initialization
        void Start()
        {
            FirebaseAnalytics.SetCurrentScreen("Main menu", "System");
            optionsPanel.alpha = 0;
            optionsPanel.blocksRaycasts = false;
            fader.DOFade(0f, 2f).onComplete = () => {
                fader.gameObject.SetActive(false);
            };

            versionText.text = $"v{Application.version}{(Debug.isDebugBuild ? "\nDev Mode" : "")}";
        }

        // Update is called once per frame
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Escape) && Application.platform == RuntimePlatform.Android)
            {
                AndroidPlugin.ShowDialog("Exit", "Are you sure you wanted to exit?", gameObject.name, "OnExit", "Yes", "YES", "No", "NO");

            }
        }

        void OnExit(string val)
        {
            if (val == "YES") Application.Quit();
        }
    }
}

