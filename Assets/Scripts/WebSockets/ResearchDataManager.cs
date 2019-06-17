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
        public Detector detector;

        WebSocket webSocket;
        DeviceCamera cam;

        Queue<Action> mainThreadActionQueue = new Queue<Action>();
        private readonly object actionQueueLock = new object();

        bool collecting = false;
        bool opened = false;
        bool identified = false;

        int collections = 0;

        private void Start()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                cam = DeviceCamera.FrontCamera;
                cam.SetFramerate(FrameratePreset.Smooth);
                cam.SetPhotoResolution(768, 1366);
                cam.SetPreviewResolution(768, 1366);
                
                NatCam.OnStart += NatCam_OnStart;
                NatCam.OnFrame += NatCam_OnFrame;
                NatCam.Verbose = Switch.On;
                NatCam.Play(cam);
                print("NatCam Playing: " + NatCam.IsPlaying);
            }
            else enabled = false;
        }


        private void NatCam_OnStart()
        {
            img.GetComponent<AspectRatioFitter>().aspectRatio = cam.PreviewResolution.x / cam.PreviewResolution.y;
            img.texture = NatCam.Preview;
            GameManager.OnGameEnd += GameManager_OnGameEnd;
            //InitiateWebsocket();
        }

        private void NatCam_OnFrame()
        {
            Texture2D frameTex = NatCam.Preview.ToTexture2D();

            Frame frame = new Frame(frameTex.GetPixels32(), frameTex.width, frameTex.height, Frame.Orientation.Upright, Time.realtimeSinceStartup);
            detector.ProcessFrame(frame);

            if (frameTex != null) Destroy(frameTex);
        }

        private void GameManager_OnGameEnd(GameEndEventArgs args)
        {
            if (args.IsEnded)
            {
                NatCam.Release();
                cam = null;
                enabled = false;

            }
        }

        void CollectParticipantData()
        {
            //First send the participant data
            ParticipantData partdata = new ParticipantData();
            partdata.versionCode = 2;
            partdata.name = GameDataManager.instance.PlayerName;
            partdata.age = GameDataManager.instance.Age;
            partdata.researchData = new List<ResearchData>();
            List<ResearchData> presetData = new List<ResearchData>();
            
            if (GameDataManager.instance.NeutralPicture != null) presetData.Add(GameDataManager.instance.NeutralData);
            if (GameDataManager.instance.HappyPicture != null) presetData.Add(GameDataManager.instance.HappyData);
            partdata.presetExpressions = presetData;

            DataPacket packet = new DataPacket();
            packet.packetId = (int)PacketType.NewParticipant;
            packet.packetData = JsonConvert.SerializeObject(partdata);
            webSocket.Send(JsonConvert.SerializeObject(packet));
        }

        private void OnCapture(Texture2D photo, Orientation orientation)
        {
            StartCoroutine(CollectGameData(photo));
            Texture2D.Destroy(photo);
        }

        IEnumerator CollectGameData(Texture2D photo)
        {
            print($"Preparing data collection {collections}");
            
            string imgStr = Convert.ToBase64String(Compressor.Compress(photo.EncodeToPNG()));
            
            yield return null;
            ResearchData data = new ResearchData()
            {
                name = GameDataManager.instance.PlayerName,
                isImage = true,
                imageData = imgStr,
                emotions = new Dictionary<string, float>(),
                expressions = new Dictionary<string, float>()
            };

            if (emotionManager.ExpressionsList.Count > 0)
            {
                foreach (var val in emotionManager.ExpressionsList[0])
                {
                    data.expressions.Add(val.Key.ToString(), val.Value);
                }
            }

            if (emotionManager.EmotionsList.Count > 0)
            {
                foreach (var val in emotionManager.EmotionsList[0])
                {
                    data.emotions.Add(val.Key.ToString(), val.Value);
                }
            }

            print($"JSON: Sent {emotionManager.EmotionsList.Count} face");

            string json = JsonConvert.SerializeObject(data);

            DataPacket packet = new DataPacket()
            {
                packetId = (int)PacketType.GameData,
                packetData = json
            };

            webSocket.Send(JsonConvert.SerializeObject(packet));
        }

        IEnumerator ProcessAndSendData()
        {
            CollectParticipantData();
            yield return new WaitForSeconds(0.25f);
            while (webSocket.ReadyState == WebSocketState.Open)
            {
                
                NatCam.CapturePhoto(OnCapture);
                yield return new WaitForSeconds(2f);
            }
            enabled = false;
            collecting = false;

        }

        void InitiateWebsocket()
        {
            Debug.Log("Initiating Websocket");
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
                    webSocket.Send(JsonConvert.SerializeObject(packet));
                    
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
                    print($"Websocket Closed, code {(CloseStatusCode)e.Code}: {e.Reason}");
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
                                collecting = true;
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
                lock (actionQueueLock)
                {
                    mainThreadActionQueue.Dequeue().Invoke();
                }
            }
        }

        private void OnApplicationQuit()
        {
            if (webSocket.ReadyState != WebSocketState.Closed && identified) webSocket.CloseAsync(CloseStatusCode.Away);
        }
    }
}
