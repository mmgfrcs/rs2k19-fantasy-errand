using Affdex;
using DG.Tweening;
using FantasyErrand.WebSockets.Models;
using FantasyErrand.WebSockets.Utilities;
using FantomLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FantasyErrand
{
    public class MainMenuManager : MonoBehaviour
    {
        public Slider loadingSlider, faderSlider;
        public CanvasGroup optionsPanel;
        public GameObject researchModeFrame, basicInfoFrame, expressionFrame;
        public TextMeshProUGUI versionText;
        public Detector detector;
        public EmotionManager emotionManager;
        public SceneChanger changer;

        [Header("Option Fields")]
        public TMP_InputField[] serverAddress;
        public Toggle researchToggle, basicGatherToggle, expressionToggle, swipeToggle, dynamicToggle;
        public TMP_InputField nameField, ageField;
        public Image neutralImage, happyImage;
        public Button backButton;

        Texture2D neutral, happy;
        PresetExpressionData nData, hData;

        public Image fader;

        bool isPanelOpen = false;

        int pickMode = 0;
        int errors = 0;

        public static bool isSwipeModeOn=true;
        public static string difficultyLevel="hard";
        public void OnPlay()
        {
            changer.ChangeScene("SampleScene");
        }

        public void playTap()
        {
            SoundManager.Instance.PlaySound("Tap");
        }

        public void playBack()
        {
            SoundManager.Instance.PlaySound("Back");
        }

        void ChangeScene()
        {
            if (dynamicToggle.isOn) changer.ChangeScene("DynamicTesting");
            else changer.ChangeScene("StaticTesting");
        }

        public void OnPlayEasy()
        {
            difficultyLevel = "easy";
            ChangeScene();
            
        }
        public void OnPlayNormal()
        {
            difficultyLevel = "normal";
            ChangeScene();
        }
        public void OnPlayHard()
        {
            difficultyLevel = "hard";
            ChangeScene();
        }

        public void OnUpgrades()
        {
            changer.ChangeScene("Shop");
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
            faderSlider.value = 0;
            faderSlider.gameObject.SetActive(true);
            StartCoroutine(LoadImage(path, pickMode == 0 ? neutralImage : happyImage));
        }

        IEnumerator LoadImage(string path, Image image)
        {
            var properties = NativeCamera.GetImageProperties(path);

            faderSlider.value = 1 / 6f;
            yield return null;
            if (properties.width < properties.height)
            {
                fader.gameObject.SetActive(true);
                fader.color = new Color(0, 0, 0, 0.5f);

                //Read the file to memory
                byte[] bytes = File.ReadAllBytes(path);
                faderSlider.value += 1 / 6f;
                yield return null;

                //Turn it into Texture2D - a format that Unity understands
                Texture2D t2d = new Texture2D(properties.width, properties.height);
                t2d.LoadImage(bytes);
                if (properties.orientation != NativeCamera.ImageOrientation.Normal && properties.orientation != NativeCamera.ImageOrientation.Unknown)
                    t2d = t2d.RotateImage();
                faderSlider.value += 1 / 6f;
                yield return null;

                //Process Texture2D using Affdex and load the results
                detector.ProcessFrame(new Frame(t2d.GetPixels32(), t2d.width, t2d.height, Time.unscaledTime));
                yield return null;
                if (pickMode == 0)
                {
                    print($"Neutral Image loaded: {emotionManager.EmotionsList.Count} emotions and {emotionManager.ExpressionsList.Count} expressions captured.");
                    nData = new PresetExpressionData() {emotions = new Dictionary<string, float>(), expressions = new Dictionary<string, float>() };
                    nData.imageData = Compressor.Compress(t2d.EncodeToPNG());
                    if (emotionManager.EmotionsList.Count > 0)
                    {
                        foreach (var emo in emotionManager.EmotionsList[0])
                        {
                            nData.emotions.Add(emo.Key.ToString(), emo.Value);
                        }
                    }
                    if(emotionManager.ExpressionsList.Count > 0)
                    { 
                        foreach (var emo in emotionManager.ExpressionsList[0])
                        {
                            nData.expressions.Add(emo.Key.ToString(), emo.Value);
                        }
                    }
                }
                else
                {
                    hData = new PresetExpressionData() { emotions = new Dictionary<string, float>(), expressions = new Dictionary<string, float>() };
                    hData.imageData = Compressor.Compress(t2d.EncodeToPNG());
                    if (emotionManager.EmotionsList.Count > 0)
                    {
                        foreach (var emo in emotionManager.EmotionsList[0])
                        {
                            hData.emotions.Add(emo.Key.ToString(), emo.Value);
                        }
                    }
                    if (emotionManager.ExpressionsList.Count > 0)
                    {
                        foreach (var emo in emotionManager.ExpressionsList[0])
                        {
                            hData.expressions.Add(emo.Key.ToString(), emo.Value);
                        }
                    }
                }
                faderSlider.value += 1 / 6f;
                yield return null;

                //Assign Texture2D to prepare for saving later
                if (pickMode == 0) neutral = t2d;
                else happy = t2d;
                faderSlider.value += 1 / 6f;
                yield return null;

                //Show Texture2D as sprite to the user
                image.rectTransform.sizeDelta = new Vector2(t2d.width * 432f / t2d.height, 432);
                image.sprite = Sprite.Create(t2d, new Rect(0, 0, t2d.width, t2d.height), Vector2.zero);
                
                AndroidPlugin.ShowToast("Image Loaded");
                fader.DOFade(0f, 1f).onComplete = () =>
                    {
                        faderSlider.value = 0;
                        fader.gameObject.SetActive(false);
                    };
                faderSlider.value += 1 / 6f;

                yield return null;
                faderSlider.gameObject.SetActive(false);
            }
            else AndroidPlugin.ShowToast("Not a portrait image");
        }

        public void OnOptions()
        {
            if(isPanelOpen)
            {
                GameDataManager.instance.SaveParticipantData(nameField.text, ageField.text == "" ? 0 : int.Parse(ageField.text), neutral, happy, nData, hData);
                GameDataManager.instance.SaveBasicOptions(researchToggle.isOn, basicGatherToggle.isOn, 
                    expressionToggle.isOn, $"192.168.{serverAddress[0].text}.{serverAddress[1].text}");
                optionsPanel.DOFade(0f, 1f).onComplete += () => {
                    optionsPanel.blocksRaycasts = false;
                };
                GameDataManager.instance.SaveResearchDataToFile();
            }
            else
            {
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
                swipeToggle.isOn = MainMenuManager.isSwipeModeOn;
                neutral = GameDataManager.instance.NeutralPicture;
                if (neutral != null)
                {
                    neutralImage.sprite = Sprite.Create(neutral, new Rect(0, 0, neutral.width, neutral.height), Vector2.zero);
                    nData = GameDataManager.instance.NeutralData;
                }
                happy = GameDataManager.instance.HappyPicture;
                if (happy != null)
                {
                    happyImage.sprite = Sprite.Create(happy, new Rect(0, 0, happy.width, happy.height), Vector2.zero);
                    hData = GameDataManager.instance.HappyData;
                }

                print($"Main Menu - Loading Data: {GameDataManager.instance.PlayerName}, Age {GameDataManager.instance.Age}\nResearch Mode is {(GameDataManager.instance.ResearchMode ? "on" : "off")} at {GameDataManager.instance.ServerAddress}");
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

        public void OnSwipeToggle(bool state)
        {
            if (state)
                isSwipeModeOn = true;
            else
                isSwipeModeOn = false;
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

