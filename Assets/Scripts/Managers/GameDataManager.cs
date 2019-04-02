using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FantasyErrand.Utilities;
using System.IO;
using FantomLib;

namespace FantasyErrand
{
    public class GameDataManager : MonoBehaviour
    {
        [SerializeField]
        int targetFrameRate = 60;

        internal static string SavePath { private get; set; } = "saves";
        internal GameData Data { get; private set; }
        internal bool ResearchMode { get; private set; }
        internal bool BasicGathering { get; private set; }
        internal bool ExpressionGathering { get; private set; }
        internal string PlayerName { get; set; }
        internal string ServerAddress { get; set; }
        internal int Age { get; set; }
        internal Texture2D NeutralPicture { get; set; }
        internal Texture2D HappyPicture { get; set; }

        internal GameDataManager instance;

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
                print($"Game Data Manager - Save path is in {Path.Combine(Application.persistentDataPath, SavePath)}");

            GameManager.OnGameEnd += GameManager_OnGameEnd;

            //Load all PlayerPrefs
            ResearchMode = PlayerPrefs.GetInt("ResearchMode", 1) == 1 ? true : false;
            BasicGathering = PlayerPrefs.GetInt("BasicGathering", 1) == 1 ? true : false;
            ExpressionGathering = PlayerPrefs.GetInt("ExpressionGathering", 0) == 1 ? true : false;
            PlayerName = PlayerPrefs.GetString("PlayerName", "Researcher");
            ServerAddress = PlayerPrefs.GetString("ServerAddr", "192.168.0.0");
            Age = PlayerPrefs.GetInt("Age", 0);
            var neutralLocation = PlayerPrefs.GetString("NeutralPicture", string.Empty);
            if (neutralLocation != string.Empty)
            {
                string neutralPic = File.ReadAllText(neutralLocation);
                //TODO: Do something
            }
            var happyLocation = PlayerPrefs.GetString("HappyPicture", string.Empty);
            if (happyLocation != string.Empty)
            {
                string happyPic = File.ReadAllText(happyLocation);
                //TODO: Do something
            }
            LoadGame();
        }

        private void GameManager_OnGameEnd(GameEndEventArgs args)
        {
            Data.SetRecord(new GameRecord(Mathf.FloorToInt(args.Score), Mathf.FloorToInt(args.Distance), Mathf.FloorToInt(args.Currency), args.Multiplier));
            Data.CumulativeRuns++;
            Data.CumulativeScore += args.Score;
            Data.CumulativeCoins += args.Currency;
            Data.CumulativeDistance += args.Distance;
            SaveGame();
        }

        public void SaveGame()
        {
            File.WriteAllText(Path.Combine(Application.persistentDataPath, SavePath), JsonUtility.ToJson(Data));
        }

        public void SaveBasicOptions(bool? research = null, bool? basicGather = null, bool? expGather = null, string serverAddr = "")
        {
            if (research.HasValue) ResearchMode = research.Value;
            if (basicGather.HasValue) BasicGathering = basicGather.Value;
            if (expGather.HasValue) ExpressionGathering = expGather.Value;
            if (serverAddr == "") ServerAddress = serverAddr;
        }

        public void SaveParticipantData(string name = "", int? age = null, Texture2D neutral = null, Texture2D happy = null)
        {
            if (name != "") PlayerName = name;
            if (age.HasValue) Age = age.Value;
            if (neutral != null) NeutralPicture = neutral;
            if (happy != null) HappyPicture = happy;
        }

        public void LoadGame()
        {
            if(File.Exists(Path.Combine(Application.persistentDataPath, SavePath))) {
                Data = JsonUtility.FromJson<GameData>(File.ReadAllText(Path.Combine(Application.persistentDataPath, SavePath)));
            }
            else
            {
                Data = new GameData("Runner");
                Data.SetRecord(new GameRecord(0, 0, 0, 10));
            }
        }
    }
}
