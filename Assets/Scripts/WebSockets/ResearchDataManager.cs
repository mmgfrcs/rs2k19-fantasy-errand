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

namespace FantasyErrand.WebSockets
{
    public class ResearchDataManager : MonoBehaviour
    {
        public UnityEngine.UI.Image image;
        public EmotionManager emotionManager;
        public Detector detector;

        WebSocket webSocket;
        WebCamTexture texture;

        bool collecting = false;

        private void Start()
        {
            StartCoroutine(InitiateWebsocket());
            Debug.Log("");
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

                image.sprite = Sprite.Create(photo, new Rect(0, 0, photo.width, photo.height), Vector2.zero);

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

        IEnumerator InitiateWebsocket()
        {
            Debug.Log("Initiating Websocket");
            webSocket = new WebSocket("ws://localhost/ws");

            webSocket.WaitTime = TimeSpan.FromSeconds(10);

            webSocket.OnOpen += (a, b) =>
            {
                Debug.Log("Websocket connection established");
                if(webSocket.Ping("Init"))
                {
                    Debug.Log("Ping sent");
                }

                
            };

            webSocket.OnError += (a, b) =>
            {
                Debug.LogErrorFormat("Websocket Error: {0} - {1} ", b.Exception, b.Message);
                return;
            };

            webSocket.OnClose += (sender, e) =>
            {
                print("Websocket Closed");
            };

            webSocket.ConnectAsync();
            yield return null;
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
            if(webSocket != null && webSocket.ReadyState == WebSocketState.Open)
            {
                if(!collecting)
                {
                    if (Application.platform == RuntimePlatform.WindowsEditor)
                        texture = new WebCamTexture(WebCamTexture.devices.FirstOrDefault(x => x.name.Contains("RGB")).name, 640, 480, 50);
                    else if (Application.platform == RuntimePlatform.Android)
                        texture = new WebCamTexture(WebCamTexture.devices.FirstOrDefault(x => x.isFrontFacing).name, 640, 480, 50);

                    texture.Play();
                    StartCoroutine(CollectData());
                    StartCoroutine(AffdexProcessFace());
                    collecting = true;
                }
            }
        }

        private void OnApplicationQuit()
        {
            if (webSocket.ReadyState != WebSocketState.Closed) webSocket.CloseAsync(CloseStatusCode.Away);
        }
    }

    [Serializable]
    public class ResearchData
    {
        public int pictureNo;
        public string imageData;

        public Dictionary<string, float> emotions;
        public Dictionary<string, float> expressions;
    }
}
