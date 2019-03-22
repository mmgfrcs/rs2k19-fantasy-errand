using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using UnityEngine;

namespace FantasyErrand.WebSockets
{
    public class ResearchDataManager : MonoBehaviour
    {
        private void Start()
        {
            StartCoroutine(InitiateWebsocket());
            Debug.Log("");
        }

        IEnumerator InitiateWebsocket()
        {

            yield return new WaitForSeconds(5f);
            Debug.Log("Initiating Websocket");
            WebSocket webSocket = new WebSocket("wss://localhost:5001/ws");

            print(webSocket.Protocol);
            webSocket.WaitTime = TimeSpan.FromSeconds(10);

            webSocket.OnOpen += (a, b) =>
            {
                //webSocket.Send("Init");
                Debug.Log("Websocket connection established");
                webSocket.CloseAsync(CloseStatusCode.Normal);
            };


            webSocket.OnError += (a, b) =>
            {
                Debug.LogErrorFormat("Websocket Error: {0} - {1} ", b.Exception, b.Message);
                return;
            };

            webSocket.ConnectAsync();
            while (webSocket.ReadyState != WebSocketState.Open)
            {
                print("Websocket " + webSocket.ReadyState + " using " + webSocket.Protocol);
                yield return null;
            }

        }
    }
}
