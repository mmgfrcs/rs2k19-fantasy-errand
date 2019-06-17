using Affdex;
using System.Collections.Generic;

namespace FantasyErrand
{
    public class EmotionManager : ImageResultsListener
    {
        public string FaceStatus { get; private set; } = "Standby";
        public float LastAcquiredTimestamp { get; private set; } = 0;
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