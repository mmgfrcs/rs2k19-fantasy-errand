﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FantomLib;

public class Player : MonoBehaviour {
    public float speed;
    public TextMeshProUGUI sensText;
    public TextMeshProUGUI scoreText;

    [Header("Motion Configuration")]
    public float sidestepSpeedThreshold = 1f;
    public float jumpSpeedThreshold = 1f;
    public float slideSpeedThreshold = 1f;
    public float turnThreshold = 4f;

    float score;
    EmotionManager emotionManager;
    bool canJump = false;
    Vector2 moveDir = new Vector2(0, 1); //Move Direction: X -> X actual, Y -> Z actual

    //Motion related variables
    //Sidestep/Jump
    Vector3 accelerometerValue;
    float velocityX = 0; //TODO: use same variable to track Y
    float velocityY = 0;
    bool recordSlide = true;
    bool recordSidestep = true;

    //Turn
    float currentRate;
    float startingAngle;
    bool recordTurn = true;

	// Use this for initialization
	void Start () {
        emotionManager = FindObjectOfType<EmotionManager>();
        Input.gyro.updateInterval = 0.1f;
        Input.gyro.enabled = true;
        
        startingAngle = GetDeviceYAngle();
        //if(!AndroidPlugin.IsSupportedSensor(SensorType.Accelerometer)) Quit
    }

    // Update is called once per frame
    void Update()
    {
        //StartCoroutine(ReportSensor());
        //Camera.main.transform.rotation = Input.gyro.attitude;
        //speed += rate * emotionManager.HappyRatio * Time.deltaTime;
        //speed -= rate * emotionManager.DisgustRatio * Time.deltaTime;

        transform.Translate(new Vector3(moveDir.x, 0, moveDir.y) * speed * Time.deltaTime);

        score += new Vector3(moveDir.x, 0, moveDir.y).magnitude * speed * Time.deltaTime;
        scoreText.text = score.ToString("n0");
        velocityY += accelerometerValue.y * Time.deltaTime;
        velocityY *= 0.97f;

        if (velocityY > jumpSpeedThreshold && canJump)
        {
            GetComponent<Rigidbody>().AddForce(Vector3.up * 7, ForceMode.Impulse);
            canJump = false;
            recordSlide = false;
        }
        else if (velocityY < -jumpSpeedThreshold && recordSlide)
        {
            transform.localScale = new Vector3(1, 1, 1);
            StartCoroutine(SlideTime());
        }

        velocityX += accelerometerValue.x * Time.deltaTime;
        velocityX *= 0.97f;

        sensText.text = $"Vel: ({velocityX}, {velocityY}) m/s\nPos {transform.position}, Scale {transform.localScale}\nJump: {canJump}, Slide: {recordSlide}, Turn: {recordTurn}, Sidestep: {recordSidestep}";

        if (recordSidestep)
        {
            //Check slide direction from velocity. Also check if the player object is on the edge of the lane
            if (velocityX > sidestepSpeedThreshold && transform.localPosition.x < 2)
            {
                Debug.Log("Slide Right!");
                transform.localPosition = new Vector3(transform.localPosition.x + 2, transform.localPosition.y, transform.localPosition.z);
                recordSidestep = false;
            }
            else if (velocityX < -sidestepSpeedThreshold && transform.localPosition.x > -2)
            {
                Debug.Log("Slide Left!");
                transform.localPosition = new Vector3(transform.localPosition.x - 2, transform.localPosition.y, transform.localPosition.z);
                recordSidestep = false;
            }
        }
        else if (velocityX < 0.05f && velocityX > -0.05f) recordSidestep = true;

        currentRate = Mathf.Lerp(currentRate, -Input.gyro.rotationRateUnbiased.y, 0.6f);

        if (recordTurn)
        {
            if (currentRate > turnThreshold)
            {
                Debug.Log("Turn Right!");
                transform.Rotate(Vector3.up * 90);// = new Vector3(transform.localPosition.x + 2, transform.localPosition.y, transform.localPosition.z);
                recordTurn = false;
            }
            else if (currentRate < -turnThreshold)
            {
                Debug.Log("Turn Left!");
                transform.Rotate(Vector3.up * -90);
                //transform.localPosition = new Vector3(transform.localPosition.x - 2, transform.localPosition.y, transform.localPosition.z);
                recordTurn = false;
            }
        }
        else if (currentRate < 0.05f && currentRate > -0.05f) recordTurn = true;
    }

    float GetDeviceYAngle()
    {
        Quaternion referenceRotation = Quaternion.identity;
        Quaternion deviceRotation = new Quaternion(0.5f, 0.5f, -0.5f, 0.5f) * Input.gyro.attitude * new Quaternion(0, 0, 1, 0);
        Quaternion eliminationOfXZ = Quaternion.Inverse(
            Quaternion.FromToRotation(referenceRotation * Vector3.up,
                                      deviceRotation * Vector3.up)
        );
        Quaternion rotationY = eliminationOfXZ * deviceRotation;
        return rotationY.eulerAngles.y;
    }

    public void OnAccelerometer(float x, float y, float z)
    {
        accelerometerValue = new Vector3(x, y, z);
    }

    IEnumerator SlideTime()
    {
        recordSlide = false;
        canJump = false;
        yield return new WaitForSeconds(1f);
        transform.localScale = new Vector3(1, 2, 1);
        recordSlide = true;
        canJump = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.gameObject.tag != "Wall")
        {
            recordSlide = true;
            canJump = true;
        }

    }
}
