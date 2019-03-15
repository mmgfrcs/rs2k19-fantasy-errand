using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FantomLib;

namespace FantasyErrand.Entities
{
    public class Player : MonoBehaviour
    {
        public float speed;

        [Header("Motion Configuration"), SerializeField]
        private bool controlActive;
        public float sidestepSpeedThreshold = 1f;
        public float jumpSpeedThreshold = 1f;
        public float slideSpeedThreshold = 1f;
        public float turnThreshold = 4f;

        EmotionManager emotionManager;

        /// <summary>
        /// Can the player be controlled by motion controls?
        /// </summary>
        public bool IsControlActive { get { return controlActive; } internal set { controlActive = value; } }

        //Vector2 moveDir = new Vector2(0, 1); //Move Direction: X -> X actual, Y -> Z actual

        //Motion related variables
        //Sidestep/Jump/Slide
        Vector3 accelerometerValue;
        float velocityX = 0;
        float velocityY = 0;
        bool canJump = false;
        bool canSlide = true;
        bool canSidestep = true;

        //Turn
        float currentRate;
        bool canTurn = true;

        // Use this for initialization
        void Start()
        {
            emotionManager = FindObjectOfType<EmotionManager>();
            Input.gyro.updateInterval = 0.1f;
            Input.gyro.enabled = true;
            
            //if(!AndroidPlugin.IsSupportedSensor(SensorType.Accelerometer)) Quit
        }

        // Update is called once per frame
        void Update()
        {
            //StartCoroutine(ReportSensor());
            //Camera.main.transform.rotation = Input.gyro.attitude;
            //speed += rate * emotionManager.HappyRatio * Time.deltaTime;
            //speed -= rate * emotionManager.DisgustRatio * Time.deltaTime;

            transform.Translate(transform.forward * speed * Time.deltaTime);

            if (IsControlActive)
            {
                speed += Time.deltaTime;
                ProcessControls();
            }

        }

        /// <summary>
        /// Process player motion controls
        /// </summary>
        void ProcessControls()
        {
            //Check for Y acceleration, then find the speed by adding it by the acceleration. Multiply by 0.97 to decay it
            velocityY += accelerometerValue.y * Time.deltaTime;
            velocityY *= 0.97f;

            if (velocityY > jumpSpeedThreshold && canJump) //If Y velocity reaches the high threshold, provided it can jump...
            {
                //Jump and disable player's capability to jump and slide while on air
                GetComponent<Rigidbody>().AddForce(Vector3.up * 7, ForceMode.Impulse);
                canJump = false;
                canSlide = false;
            }
            else if (velocityY < -jumpSpeedThreshold && canSlide) //If Y velocity reaches the low threshold, provided it can slide...
            {
                //Slide and disable player's capability to jump and slide while sliding
                transform.localScale = new Vector3(1, 1, 1);
                StartCoroutine(SlideTime());
            }

            velocityX += accelerometerValue.x * Time.deltaTime;
            velocityX *= 0.97f;

            if (canSidestep)
            {
                //Check slide direction from velocity. Also check if the player object is on the edge of the lane
                if (velocityX > sidestepSpeedThreshold && transform.localPosition.x < 2)
                {
                    Debug.Log("Slide Right!");
                    transform.localPosition = new Vector3(transform.localPosition.x + 2, transform.localPosition.y, transform.localPosition.z);
                    canSidestep = false;
                }
                else if (velocityX < -sidestepSpeedThreshold && transform.localPosition.x > -2)
                {
                    Debug.Log("Slide Left!");
                    transform.localPosition = new Vector3(transform.localPosition.x - 2, transform.localPosition.y, transform.localPosition.z);
                    canSidestep = false;
                }
            }
            else if (velocityX < 0.05f && velocityX > -0.05f) canSidestep = true;

            currentRate = Mathf.Lerp(currentRate, -Input.gyro.rotationRateUnbiased.y, 0.6f);

            if (canTurn)
            {
                if (currentRate > turnThreshold)
                {
                    Debug.Log("Turn Right!");
                    transform.Rotate(Vector3.up * 90);// = new Vector3(transform.localPosition.x + 2, transform.localPosition.y, transform.localPosition.z);
                    canTurn = false;
                }
                else if (currentRate < -turnThreshold)
                {
                    Debug.Log("Turn Left!");
                    transform.Rotate(Vector3.up * -90);
                    //transform.localPosition = new Vector3(transform.localPosition.x - 2, transform.localPosition.y, transform.localPosition.z);
                    canTurn = false;
                }
            }
            else if (currentRate < 0.05f && currentRate > -0.05f) canTurn = true;
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
            canSlide = false;
            canJump = false;
            yield return new WaitForSeconds(1f);
            transform.localScale = new Vector3(1, 2, 1);
            canSlide = true;
            canJump = true;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.gameObject.tag != "Wall")
            {
                canSlide = true;
                canJump = true;
            }

        }
    }
}