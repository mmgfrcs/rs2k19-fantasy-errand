using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FantasyErrand.Utilities;
using System.IO;

namespace FantasyErrand
{
    
    public class GameDataManager : MonoBehaviour
    {
        internal GameData Data { get; private set; }

        private void Start()
        {
            GameManager.OnGameEnd += GameManager_OnGameEnd;
        }

        private void GameManager_OnGameEnd(GameEndEventArgs args)
        {
            var temp = new GameData();
            temp.PlayerName = Data.PlayerName;
            temp.SetRecord(new GameRecord(Mathf.FloorToInt(args.Score), Mathf.FloorToInt(args.Distance), Mathf.FloorToInt(args.Currency), args.Multiplier));
            Data = temp;
            SaveGame();
        }

        public void SaveGame()
        {
            File.WriteAllText(Path.Combine(Application.persistentDataPath, "saves"), JsonUtility.ToJson(Data));
        }

        public void LoadGame()
        {
            if(File.Exists(Path.Combine(Application.persistentDataPath, "saves"))) {
                Data = JsonUtility.FromJson<GameData>(File.ReadAllText(Path.Combine(Application.persistentDataPath, "saves")));
            }
        }
    }
}
