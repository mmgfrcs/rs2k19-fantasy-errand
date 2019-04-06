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
using FantasyErrand.WebSockets.Models;

namespace FantasyErrand.WebSockets
{
    public class ResearchDataManager : MonoBehaviour
    {
        [Header("Options"), SerializeField]
        private string opcode = "684F2BA5B03585274A874D21BB6B16B802227A081577946A82E14EED9A4468DB";
        public bool dataTransmission = true;

        [Header("Scripts")]
        public EmotionManager emotionManager;
        public Detector detector;

        WebSocket webSocket;
        WebCamTexture texture;

        Queue<Action> mainThreadActionQueue = new Queue<Action>();
        private readonly object actionQueueLock = new object();

        bool collecting = false;
        bool opened = false;
        bool identified = false;
        string nonce = "";

        private void Start()
        {
            //TODO: Only record if FER is enabled
            if (Application.platform == RuntimePlatform.WindowsEditor)
                texture = new WebCamTexture(WebCamTexture.devices.FirstOrDefault(x => x.name.Contains("RGB")).name, 640, 480, 50);
            else if (Application.platform == RuntimePlatform.Android)
                texture = new WebCamTexture(WebCamTexture.devices.FirstOrDefault(x => x.isFrontFacing).name, 640, 480, 50);

            texture.Play();
            StartCoroutine(AffdexProcessFace());

            InitiateWebsocket();
        }

        IEnumerator CollectData()
        {
            int n = 1;
            while (webSocket.ReadyState == WebSocketState.Open)
            {
                yield return new WaitForSecondsRealtime(2f);
                yield return new WaitForEndOfFrame();
                Texture2D photo = new Texture2D(texture.width, texture.height);
                photo.SetPixels(texture.GetPixels());
                photo.Apply();

                string imgStr = Convert.ToBase64String(photo.EncodeToPNG());
                ResearchData data = new ResearchData() {
                    pictureNo = n++,
                    imageData = imgStr,
                    emotions = new Dictionary<string, float>(),
                    expressions = new Dictionary<string, float>()
                };

                if(emotionManager.ExpressionsList.Count > 0)
                {
                    foreach (var val in emotionManager.ExpressionsList[0])
                    {
                        data.expressions.Add(val.Key.ToString(), val.Value);
                    }
                }

                if(emotionManager.EmotionsList.Count > 0)
                {
                    foreach (var val in emotionManager.EmotionsList[0])
                    {
                        data.emotions.Add(val.Key.ToString(), val.Value);
                    }
                }

                print($"JSON: Sent {emotionManager.EmotionsList.Count} face");

                string json = JsonConvert.SerializeObject(data);
                
                webSocket.Send(json);
            }
            texture.Stop();
            enabled = false;
            collecting = false;
        }

        void InitiateWebsocket()
        {
            Debug.Log("Initiating Websocket");
            webSocket = new WebSocket("ws://localhost/ws");

            webSocket.WaitTime = TimeSpan.FromSeconds(10);

            webSocket.OnOpen += (a, b) =>
            {
                DataPacket packet = new DataPacket();
                packet.packetId = (int)PacketType.Identify;
                packet.packetData = string.Join(";", new string[] { Application.identifier, opcode });

                webSocket.Send(packet.ToByteArray(ByteOrder.Big));

                mainThreadActionQueue.Enqueue(() =>
                {
                    Debug.Log("Websocket connection established");
                    
                });
                
                collecting = true;
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
                    print("Websocket Closed");
                    if (!opened) StartCoroutine(Reconnect());
                });
                identified = false;
            };

            webSocket.OnMessage += (sender, e) =>
            {
                if(e.IsBinary)
                {
                    DataPacket packet = e.RawData.To<DataPacket>(ByteOrder.Big);
                    if(packet.packetId == (int)PacketType.Identify)
                    {
                        string[] data = packet.packetData.Split(';');
                        try
                        {
                            //StartCoroutine(CollectData());
                        }
                        catch(Exception)
                        {
                            webSocket.CloseAsync(CloseStatusCode.InvalidData);
                        }
                    }
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

        IEnumerator AffdexProcessFace()
        {
            while(texture.isPlaying)
            {
                yield return new WaitForEndOfFrame();
                Frame frame = new Frame(texture.GetPixels32(), texture.width, texture.height, Frame.Orientation.Upright, Time.realtimeSinceStartup);
                detector.ProcessFrame(frame);
                yield return new WaitForSecondsRealtime(1 / texture.requestedFPS);
            }
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
            if (webSocket.ReadyState != WebSocketState.Closed) webSocket.CloseAsync(CloseStatusCode.Away);
        }
    }
}
