using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using UnityEngine;
using System.Collections.Generic;
using Affdex;
using Newtonsoft.Json;
using FantasyErrand.WebSockets.Utilities;
using FantasyErrand.WebSockets.Models;
using UnityEngine.UI;
using FantomLib;
using System.Runtime;
using NatCamU.Core;
using NatCamU.Extended;
using NatCamU.Professional;

namespace FantasyErrand.WebSockets
{
    public class ResearchDataManager : MonoBehaviour
    {
        public RawImage img;

        [Header("Options"), SerializeField]
        private string opcode = "684F2BA5B03585274A874D21BB6B16B802227A081577946A82E14EED9A4468DB";
        public bool dataTransmission = true;
        public int retryAttempts = 3;

        [Header("Scripts")]
        public EmotionManager emotionManager;
        public GameManager gameManager;
        

        internal Dictionary<Emotions, float> EmotionsList { get; set; } = new Dictionary<Emotions, float>();
        internal Dictionary<Expressions, float> ExpressionsList { get; set; } = new Dictionary<Expressions, float>();

        LevelManagerBase levelManager;
        WebSocket webSocket;
        //WebCamTexture webcam;
        Queue<Action> mainThreadActionQueue = new Queue<Action>();
        private readonly object actionQueueLock = new object();

        bool opened = false;
        bool identified = false;

        int collections = 0;

        private void Start()
        {
            //webcam = new WebCamTexture(WebCamTexture.devices.First(x => x.isFrontFacing).name, 640, 480, 15);
            //webcam.Play();
            //img.texture = webcam;
            //StartCoroutine(StartAffdex());
            if (GameDataManager.instance.ResearchMode)
            {

                InitiateWebsocket();
                levelManager = gameManager.levelManager;
                GameManager.OnGameEnd += GameManager_OnGameEnd;
                EmotionManager.OnFaceResults += EmotionManager_OnFaceResults;
                EmotionManager.OnFaceLost += EmotionManager_OnFaceLost;
            }
        }

        private void EmotionManager_OnFaceLost()
        {
            EmotionsList = new Dictionary<Emotions, float>();
            ExpressionsList = new Dictionary<Expressions, float>();
        }

        private void EmotionManager_OnFaceResults(Dictionary<Emotions, float> emotions, Dictionary<Expressions, float> expressions)
        {
            EmotionsList = emotions;
            ExpressionsList = expressions;
        }

        private void GameManager_OnGameEnd(GameEndEventArgs args)
        {
            if (args.IsEnded)
            {
                StopAllCoroutines();
                enabled = false;
            }
        }

        void CollectParticipantData()
        {
            //First send the participant data
            print("Sending participant data");
            ParticipantData partdata = new ParticipantData();
            partdata.versionCode = 3;
            if (GameDataManager.instance.BasicGathering)
            {
                partdata.name = GameDataManager.instance.PlayerName;
                partdata.age = GameDataManager.instance.Age;
            }
            else
            {
                partdata.name = "Anonymous";
                partdata.age = 1;
            }

            print("Participant name and age loaded");

            partdata.researchData = new List<ResearchData>();
            List<PresetExpressionData> presetData = new List<PresetExpressionData>();

            if (GameDataManager.instance.ExpressionGathering)
            {
                if (GameDataManager.instance.NeutralPicture != null) presetData.Add(GameDataManager.instance.NeutralData);
                if (GameDataManager.instance.HappyPicture != null) presetData.Add(GameDataManager.instance.HappyData);
            }

            partdata.presetExpressions = presetData;

            print("Participant preset expressions loaded");

            DataPacket packet = new DataPacket();
            packet.packetId = (int)PacketType.NewParticipant;
            packet.packetData = JsonConvert.SerializeObject(partdata);
            webSocket.SendAsync(JsonConvert.SerializeObject(packet), null);
            print("JSON: Participant data sent");
        }

        void CollectGameData()
        {
            print($"Preparing data collection {collections}, expressions: {ExpressionsList.Count}");

            ResearchData data = new ResearchData()
            {
                time = DateTime.UtcNow,
                emotions = new Dictionary<string, float>(),
                expressions = new Dictionary<string, float>(),
                score = gameManager.Score,
                distance = gameManager.Distance,
                coins = gameManager.Currency,
                baseTileRate = levelManager.GetTileRate(TileType.Tile),
                obstacleRate = levelManager.GetTileRate(TileType.Obstacle),
                coinsRate = levelManager.GetTileRate(TileType.Coin),
                powerupsRate = levelManager.GetTileRate(TileType.Powerups),
                difficulty = MainMenuManager.mainMenuDifficulty.ToString(),
                gameType = levelManager is StaticLevelManager ? "Static" : "Dynamic",
                playerSpeed = gameManager.GetCurrSpeed()
            };

            if (ExpressionsList.Count > 0)
            {
                foreach (var val in ExpressionsList)
                {
                    data.expressions.Add(val.Key.ToString(), val.Value);
                }
            }

            if (EmotionsList.Count > 0)
            {
                foreach (var val in EmotionsList)
                {
                    data.emotions.Add(val.Key.ToString(), val.Value);
                }
            }

            print($"JSON: Sent {EmotionsList.Count} face");

            string json = JsonConvert.SerializeObject(data);

            DataPacket packet = new DataPacket()
            {
                packetId = (int)PacketType.GameData,
                packetData = json
            };

            webSocket.SendAsync(JsonConvert.SerializeObject(packet), null);
            collections++;
        }

        IEnumerator ProcessAndSendData()
        {
            CollectParticipantData();
            yield return new WaitForSeconds(0.25f);
            while (webSocket.ReadyState == WebSocketState.Open)
            {
                if(gameManager.IsGameRunning) CollectGameData();
                yield return new WaitForSeconds(1f);
            }
            enabled = false;

        }

        void InitiateWebsocket()
        {
            Debug.Log($"Initiating Websocket at {GameDataManager.instance.ServerAddress}:5002");
            webSocket = new WebSocket($"ws://{GameDataManager.instance.ServerAddress}:5002/ws");

            webSocket.WaitTime = TimeSpan.FromSeconds(10);

            webSocket.OnOpen += (a, b) =>
            {
                mainThreadActionQueue.Enqueue(() =>
                {
                    DataPacket packet = new DataPacket();
                    packet.packetId = (int)PacketType.Identify;
                    packet.packetData = string.Join(";", new string[] { Application.identifier, opcode });
                    Debug.Log($"Websocket connection established. Sending Identify {packet.packetData}");
                    webSocket.SendAsync(JsonConvert.SerializeObject(packet), null);
                    
                });
            };

            webSocket.OnError += (a, b) =>
            {
                mainThreadActionQueue.Enqueue(() => Debug.LogErrorFormat("Websocket Error: {0} - {1} ", b.Exception, b.Message));
                identified = false;
                return;
            };

            webSocket.OnClose += (sender, e) =>
            {
                mainThreadActionQueue.Enqueue(() => {
                    if(e.WasClean) print($"Websocket Closed, code {(CloseStatusCode)e.Code}: {e.Reason}");
                    else Debug.LogWarning($"Websocket Closed, code {(CloseStatusCode)e.Code}: {e.Reason}");
                    if (!opened)
                    {
                        if (retryAttempts > 0)
                        {
                            retryAttempts--;
                            StartCoroutine(Reconnect());
                        }
                        else Debug.LogWarning("WebSockets: Out of Retry Attempts - Research data will not be submitted");
                    }
                });
                identified = false;
            };

            webSocket.OnMessage += (sender, e) =>
            {
                if(!e.IsBinary)
                {
                    DataPacket packet = JsonConvert.DeserializeObject<DataPacket>(e.Data);
                    if (packet.packetId == (int)PacketType.Identify && !identified)
                    {
                        string[] data = packet.packetData.Split(';');
                        try
                        {
                            if (data.Length == 2 && data[1] == opcode)
                            {
                                identified = true;
                                mainThreadActionQueue.Enqueue(() => StartCoroutine(ProcessAndSendData()));
                            }
                            else throw new ArgumentException($"Data received invalid: Length {data.Length}, Opcode: {data[1]}");
                        }
                        catch (ArgumentException)
                        {
                            webSocket.CloseAsync(CloseStatusCode.InvalidData);
                        }
                    }
                    else Debug.LogError($"(Server) Error in {(PacketType)packet.packetId}: {packet.packetData}");
                }
            };

            webSocket.ConnectAsync();
        }

        IEnumerator Reconnect()
        {
            print("Reconnecting in 5 seconds");
            yield return new WaitForSecondsRealtime(5f);
            InitiateWebsocket();
        }

        private void Update()
        {
            if(mainThreadActionQueue.Count > 0)
            {
                    mainThreadActionQueue.Dequeue().Invoke();
                
            }
        }

        private void OnApplicationQuit()
        {
            if (webSocket.ReadyState != WebSocketState.Closed && identified) webSocket.CloseAsync(CloseStatusCode.Away);
        }

    }
}
