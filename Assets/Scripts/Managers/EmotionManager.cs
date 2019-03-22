﻿using System.Collections;
using System.Collections.Generic;
using Affdex;
using UnityEngine;
using UnityEngine.UI;

namespace FantasyErrand
{
    public class EmotionManager : ImageResultsListener
    {
        public string FaceStatus { get; private set; }
        public float LastAcquiredTimestamp { get; private set; }
        public List<Dictionary<Emotions, float>> EmotionsList { get; private set; } = new List<Dictionary<Emotions, float>>();
        public List<Dictionary<Expressions, float>> ExpressionsList { get; private set; } = new List<Dictionary<Expressions, float>>();

        public override void onFaceFound(float timestamp, int faceId)
        {
            FaceStatus = "Tracking";
            LastAcquiredTimestamp = timestamp;
        }

        public override void onFaceLost(float timestamp, int faceId)
        {
            FaceStatus = "Standby";
        }

        public override void onImageResults(Dictionary<int, Face> faces)
        {
            print($"Face results! Faces: {faces.Count}");
            List<Dictionary<Emotions, float>> emoListTemp = new List<Dictionary<Emotions, float>>();
            List<Dictionary<Expressions, float>> expListTemp = new List<Dictionary<Expressions, float>>();
            for (int i = 0; i < faces.Count; i++)
            {
                emoListTemp.Add(faces[i].Emotions);
                expListTemp.Add(faces[i].Expressions);
            }
            EmotionsList = emoListTemp;
            ExpressionsList = expListTemp;
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