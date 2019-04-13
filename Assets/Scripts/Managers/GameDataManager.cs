using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FantasyErrand.Utilities;
using System.IO;
using FantomLib;
using System;
using Firebase.Analytics;
using FantasyErrand.WebSockets.Models;

namespace FantasyErrand
{
    public class GameDataManager : MonoBehaviour
    {
        [SerializeField]
        int targetFrameRate = 60;

        internal static string SaveFileName { get; private set; } = "saves";
        internal static string ImagePath { get; private set; } = "img64";

        internal GameData Data { get; private set; }
        internal bool ResearchMode { get; private set; }
        internal bool BasicGathering { get; private set; }
        internal bool ExpressionGathering { get; private set; }
        internal string PlayerName { get; private set; }
        internal string ServerAddress { get; private set; }
        internal int Age { get; private set; }
        internal Texture2D NeutralPicture { get; private set; }
        internal Texture2D HappyPicture { get; private set; }
        internal ResearchData NeutralData { get; private set; }
        internal ResearchData HappyData { get; private set; }
        string NeutralLocation { get; set; }
        string HappyLocation { get; set; }

        bool NeutralModified { get; set; }
        bool HappyModified { get; set; }
        internal static GameDataManager instance;

        private void Start()
        {
            if (instance == null) instance = this;
            else
            {
                Destroy(this);
                return;
            }

            DontDestroyOnLoad(this);
            Application.targetFrameRate = targetFrameRate;

            if (Application.platform == RuntimePlatform.WindowsEditor)
                print($"Game Data Manager - Save path is in {Path.Combine(Application.persistentDataPath, SaveFileName)}");

            GameManager.OnGameEnd += GameManager_OnGameEnd;

            //Load all PlayerPrefs

            LoadAllData();
        }

        private void GameManager_OnGameEnd(GameEndEventArgs args)
        {
            Data.SetRecord(new GameRecord(Mathf.FloorToInt(args.Score), Mathf.FloorToInt(args.Distance), Mathf.FloorToInt(args.Currency), args.Multiplier));
            Data.CumulativeRuns++;
            Data.CumulativeScore += args.Score;
            Data.CumulativeCoins += args.Currency;
            Data.CumulativeDistance += args.Distance;
            SaveGameDataToFile();
        }

        public void SaveAllDataToFile()
        {
            SaveResearchDataToFile();
            SaveGameDataToFile();
        }

        public void SaveGameDataToFile()
        {
            Data.PlayerName = PlayerName;
            print($"GameDataManager: Saving Data: {Data.PlayerName}, {Data.CumulativeScore} in {Data.CumulativeRuns} runs\nJSON: {JsonUtility.ToJson(Data)}");
            File.WriteAllText(Path.Combine(Application.persistentDataPath, SaveFileName), JsonUtility.ToJson(Data));
        }

        public void SaveResearchDataToFile()
        {
            PlayerPrefs.SetInt("ResearchMode", ResearchMode ? 1 : 0);
            PlayerPrefs.SetInt("BasicGathering", BasicGathering ? 1 : 0);
            PlayerPrefs.SetInt("ExpressionGathering", ExpressionGathering ? 1 : 0);
            PlayerPrefs.SetString("Name", PlayerName);
            PlayerPrefs.SetString("ServerAddr", ServerAddress);
            PlayerPrefs.SetInt("Age", Age);

            if (!Directory.Exists(Application.persistentDataPath + "/img64")) Directory.CreateDirectory(Application.persistentDataPath + "/img64");

            if (NeutralPicture != null && NeutralModified)
            {
                string path = Path.Combine(Application.persistentDataPath, ImagePath, "neutral");
                print($"Saved Neutral Picture in {path}");
                File.WriteAllBytes(path, NeutralPicture.EncodeToPNG());
                
                PlayerPrefs.SetString("NeutralPicture", path);
            }
            if (HappyPicture != null && HappyModified)
            {
                string path = Path.Combine(Application.persistentDataPath, ImagePath, "happy");
                print($"Saved Happy Picture in {path}");
                File.WriteAllBytes(path, HappyPicture.EncodeToPNG());
                PlayerPrefs.SetString("HappyPicture", path);
            }
            NeutralModified = false;
            HappyModified = false;
            PlayerPrefs.Save();
            print($"Game Data Manager - Saved data: {PlayerName}, Age {Age}\nResearch: {ResearchMode}, Basic Gathering: {BasicGathering}, Expression Gathering: {ExpressionGathering} Server IP: {ServerAddress}\nNeutral Picture: {NeutralLocation}, Happy Picture: {HappyLocation}");
        }

        public void SaveBasicOptions(bool? research = null, bool? basicGather = null, bool? expGather = null, string serverAddr = "")
        {
            if (research.HasValue) ResearchMode = research.Value;
            if (basicGather.HasValue) BasicGathering = basicGather.Value;
            if (expGather.HasValue) ExpressionGathering = expGather.Value;
            if (serverAddr != "") ServerAddress = serverAddr;
        }

        public void SaveParticipantData(string name = "", int? age = null, Texture2D neutral = null, Texture2D happy = null, ResearchData neutralData = default(ResearchData), ResearchData happyData = default(ResearchData))
        {
            if (name != "") PlayerName = name;
            if (age.HasValue) Age = age.Value;
            if (neutral != null)
            {
                if (NeutralPicture != neutral) NeutralModified = true;
                NeutralPicture = neutral;
                if (neutralData.name != default(ResearchData).name) NeutralData = neutralData;
            }
            if (happy != null)
            {
                if (HappyPicture != happy) HappyModified = true;
                HappyPicture = happy;
                if (happyData.name != default(ResearchData).name) HappyData = happyData;
            }
            
        }

        public void LoadAllData()
        {
            LoadResearchData();
            LoadGameData();
        }

        public void LoadGameData()
        {
            if(File.Exists(Path.Combine(Application.persistentDataPath, SaveFileName))) Data = JsonUtility.FromJson<GameData>(File.ReadAllText(Path.Combine(Application.persistentDataPath, SaveFileName)));
            else
            {
                Data = new GameData("Researcher");
                Data.SetRecord(new GameRecord(0, 0, 0, 10));
            }
        }

        public void LoadResearchData()
        {
            ResearchMode = PlayerPrefs.GetInt("ResearchMode", 1) == 1 ? true : false;
            BasicGathering = PlayerPrefs.GetInt("BasicGathering", 1) == 1 ? true : false;
            ExpressionGathering = PlayerPrefs.GetInt("ExpressionGathering", 0) == 1 ? true : false;
            PlayerName = PlayerPrefs.GetString("Name", "Researcher");
            ServerAddress = PlayerPrefs.GetString("ServerAddr", "192.168.0.0");
            Age = PlayerPrefs.GetInt("Age", 0);
            var neutralLocation = PlayerPrefs.GetString("NeutralPicture", string.Empty);
            if (neutralLocation != string.Empty)
            {
                byte[] neutralPic = File.ReadAllBytes(neutralLocation);
                NeutralLocation = neutralLocation;
                NeutralPicture = new Texture2D(1, 1);
                NeutralPicture.LoadImage(neutralPic);
                //TODO: Do something
            }
            var happyLocation = PlayerPrefs.GetString("HappyPicture", string.Empty);
            if (happyLocation != string.Empty)
            {
                byte[] happyPic = File.ReadAllBytes(happyLocation);
                HappyLocation = happyLocation;
                HappyPicture = new Texture2D(1, 1);
                HappyPicture.LoadImage(happyPic);

                //TODO: Do something
            }
            print($"Game Data Manager - Loaded data: {PlayerName}, Age {Age}\nResearch: {ResearchMode}, Basic Gathering: {BasicGathering}, Expression Gathering: {ExpressionGathering} Server IP: {ServerAddress}\nNeutral Picture: {NeutralLocation}, Happy Picture: {HappyLocation}");
        }
    }
}
