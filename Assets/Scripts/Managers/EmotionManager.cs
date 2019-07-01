using Affdex;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using FantasyErrand.WebSockets;

namespace FantasyErrand
{
    public class EmotionManager : ImageResultsListener
    {
        public bool sendData = true;
        public ResearchDataManager target;

        public string FaceStatus { get; private set; } = "Standby";
        public int DetectedFaces { get; private set; } = 0;
        public float LastAcquiredTimestamp { get; private set; } = 0;
        public List<Dictionary<Emotions, float>> EmotionsList { get; private set; } = new List<Dictionary<Emotions, float>>();
        public List<Dictionary<Expressions, float>> ExpressionsList { get; private set; } = new List<Dictionary<Expressions, float>>();

        public delegate void FaceResultArgs(Dictionary<Emotions, float> emotions, Dictionary<Expressions, float> expressions);
        public static event FaceResultArgs OnFaceResults;
        public static event System.Action OnFaceFound, OnFaceLost;
        public override void onFaceFound(float timestamp, int faceId)
        {
            
            FaceStatus = "Tracking";
            print($"{SceneManager.GetActiveScene().name} EmotionManager - {FaceStatus}");
            LastAcquiredTimestamp = timestamp;
            OnFaceFound?.Invoke();
        }

        public override void onFaceLost(float timestamp, int faceId)
        {
            
            //if (SceneManager.GetActiveScene().name != "Main")
            //{
            //    EmotionsList = new List<Dictionary<Emotions, float>>();
            //    ExpressionsList = new List<Dictionary<Expressions, float>>();
            //}

            FaceStatus = "Standby";
            print($"{SceneManager.GetActiveScene().name} EmotionManager - {FaceStatus}");
            OnFaceLost?.Invoke();
        }

        public override void onImageResults(Dictionary<int, Face> faces)
        {
            if(faces.Count > 0)
            {
                //List<Dictionary<Emotions, float>> emoListTemp = new List<Dictionary<Emotions, float>>();
                //List<Dictionary<Expressions, float>> expListTemp = new List<Dictionary<Expressions, float>>();

                EmotionsList = new List<Dictionary<Emotions, float>>();
                ExpressionsList = new List<Dictionary<Expressions, float>>();

                //emoListTemp.Add(faces[i].Emotions);
                //expListTemp.Add(faces[i].Expressions);
                EmotionsList.Add(faces[0].Emotions);
                ExpressionsList.Add(faces[0].Expressions);
            
                //EmotionsList = emoListTemp;
                //ExpressionsList = expListTemp;
                DetectedFaces += faces.Count;
                OnFaceResults?.Invoke(EmotionsList[0], ExpressionsList[0]);
                //if (sendData && target != null) { print($"Data sent: {EmotionsList[0].Count} emotions, {ExpressionsList[0].Count} expressions"); target.EmotionsList = EmotionsList[0]; target.ExpressionsList = ExpressionsList[0]; }
            }
            
        }
        
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}