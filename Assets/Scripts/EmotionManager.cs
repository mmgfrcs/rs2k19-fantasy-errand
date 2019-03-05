using System.Collections;
using System.Collections.Generic;
using Affdex;
using UnityEngine;
using UnityEngine.UI;

public class EmotionManager : ImageResultsListener {

    public string FaceStatus {get; private set;}
    public float LastAcquiredTimestamp {get; private set;}
    public float HappyRatio {get; private set;}
    public float SadRatio {get; private set;}
    public float AngryRatio {get; private set;}
    public float DisgustRatio {get; private set;}
    public float FearRatio {get; private set;}
    public Dictionary<Expressions,float> Expressions {get; private set;}

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
        if(faces.Count > 0) {
            FaceStatus = "Analyzing Face";
            HappyRatio = faces[0].Emotions[Emotions.Joy] / 100f;
            SadRatio = faces[0].Emotions[Emotions.Sadness] / 100f;
            AngryRatio = faces[0].Emotions[Emotions.Anger] / 100f;
            DisgustRatio = faces[0].Emotions[Emotions.Disgust] / 100f;
            FearRatio = faces[0].Emotions[Emotions.Fear] / 100f;
            Expressions = faces[0].Expressions;
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
