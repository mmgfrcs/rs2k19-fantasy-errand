using System.Collections;
using System.Collections.Generic;
using System.Text;
using Affdex;
using UnityEngine;
using UnityEngine.UI;

public class EmotionUI : MonoBehaviour {
	public RawImage feed;
    public Text faceStatusText, lastAcquireText;
	public GameObject sliderGroup;
    public Slider happySlider, sadSlider, angrySlider, disgustSlider, fearSlider;
	public Text expressions;
	EmotionManager em;
	CameraInput cam;
	// Use this for initialization
	void Start () {
		em = GetComponent<EmotionManager>();
		cam = GameObject.FindObjectOfType<CameraInput>();
		
	}
	
	// Update is called once per frame
	void Update () {
		feed.texture = cam.Texture;
		faceStatusText.text = em.FaceStatus;
		lastAcquireText.text = "Last Acquired on " + em.LastAcquiredTimestamp;
		if(em.FaceStatus == "Analyzing Face") {
			sliderGroup.SetActive(true);
			happySlider.value = em.HappyRatio;
			sadSlider.value = em.SadRatio;
			angrySlider.value = em.AngryRatio;
			disgustSlider.value = em.DisgustRatio;
			fearSlider.value = em.FearRatio;
			StringBuilder sb = new StringBuilder();
			foreach(var exp in em.Expressions){
				sb.AppendLine(exp.Key + ": " + exp.Value.ToString("n2"));
			}
			expressions.text = sb.ToString();
		}
		else {
			sliderGroup.SetActive(false);
			expressions.text = "";
		}
	}

	Texture2D toTexture2D(Texture rTex)
	{
		Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGB24, false);
		
		tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
		tex.UpdateExternalTexture(rTex.GetNativeTexturePtr());
		tex.Apply();
		return tex;
	}
}
