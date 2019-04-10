﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FantomLib;
using DG.Tweening;

namespace FantasyErrand.Entities
{
    public class Player : MonoBehaviour
    {
        [Header("Non-Game")]
        public bool enableNonGameMode;

        [Header("Motion Configuration"), SerializeField]
        private bool controlActive;
        public float sidestepSpeedThreshold = 1f;
        public float jumpSpeedThreshold = 1f;
        public float slideSpeedThreshold = 1f;
        public float turnThreshold = 4f;

        EmotionManager emotionManager;

        internal float speed;

        public delegate void TurnEventDelegate(int direction);
        public event System.Action<Collision> OnCollision;
        public event TurnEventDelegate OnTurn;

        /// <summary>
        /// Can the player be controlled by motion controls?
        /// </summary>
        public bool IsControlActive { get { return controlActive; } internal set { controlActive = value; } }

        //Vector2 moveDir = new Vector2(0, 1); //Move Direction: X -> X actual, Y -> Z actual

        //Motion related variables
        //Sidestep/Jump/Slide
        Vector3 accelerometerValue;
        Vector3 initialScale;
        float velocityX = 0;
        float velocityY = 0;
        bool canJump = true;
        bool canSlide = true;
        bool canSidestep = true;

        float jumpCollDelay = 0;
        int lane = 0;

        // Use this for initialization
        void Start()
        {
            if (enableNonGameMode)
            {
                Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Obstacles"), LayerMask.NameToLayer("Player"));
                speed = 5;
            }
            else
            {
                emotionManager = FindObjectOfType<EmotionManager>();
                initialScale = transform.localScale;
            }
        }


        // Update is called once per frame
        void Update()
        {
            //StartCoroutine(ReportSensor());
            //Camera.main.transform.rotation = Input.gyro.attitude;
            //speed += rate * emotionManager.HappyRatio * Time.deltaTime;
            //speed -= rate * emotionManager.DisgustRatio * Time.deltaTime;

            transform.Translate(transform.forward * speed * Time.deltaTime);

            jumpCollDelay = Mathf.Max(0, jumpCollDelay - Time.deltaTime);

            if (IsControlActive && !enableNonGameMode)
            {
                if (Application.platform == RuntimePlatform.WindowsEditor)
                    ProcessKeyControls();
                else ProcessControls();
                RaycastHit hit;
                if(Physics.Raycast(new Vector3(transform.position.x, 0.1f, transform.position.z), Vector3.down, out hit, 0.2f)){
                    if(jumpCollDelay == 0)
                    {
                        
                        GetComponent<Rigidbody>().isKinematic = true;
                        canSlide = true;
                        canJump = true;
                    }
                    
                }
            }

        }

        /// <summary>
        /// Process player Keyboard controls (Editor and Windows Test build only!). Not for use together with <see cref="Player.ProcessControls"/>
        /// </summary>
        void ProcessKeyControls()
        {
            //Check for Y acceleration, then find the speed by adding it by the acceleration. Multiply by 0.97 to decay it
            velocityY += accelerometerValue.y * Time.deltaTime;
            velocityY *= 0.97f;

            if (Input.GetKeyDown(KeyCode.W) && canJump) //If Y velocity reaches the high threshold, provided it can jump...
            {
                //Jump and disable player's capability to jump and slide while on air
                GetComponent<Rigidbody>().isKinematic = false;
                GetComponent<Rigidbody>().AddForce(Vector3.up * 7, ForceMode.Impulse);
                canJump = false;
                canSlide = false;
                jumpCollDelay = 0.1f;
            }
            else if (Input.GetKeyDown(KeyCode.S) && canSlide) //If Y velocity reaches the low threshold, provided it can slide...
            {
                //Slide and disable player's capability to jump and slide while sliding
                transform.localScale = new Vector3(initialScale.x, initialScale.y / 2, initialScale.z);
                StartCoroutine(SlideTime());
            }
            
            //Check slide direction from velocity. Also check if the player object is on the edge of the lane
            if (Input.GetKeyDown(KeyCode.D) && lane < 1)
            {
                Debug.Log("Slide Right!");
                transform.DOMoveX(1.5f * ++lane, 0.8f);
            }
            else if (Input.GetKeyDown(KeyCode.A) && lane > -1)
            {
                Debug.Log("Slide Left!");
                transform.DOMoveX(1.5f * --lane, 0.8f);
            }
        }

        /// <summary>
        /// Process player motion controls. Not to be used together with <see cref="Player.ProcessKeyControls"/>
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
                    transform.Translate(Vector3.right * 2);
                    canSidestep = false;
                }
                else if (velocityX < -sidestepSpeedThreshold && transform.localPosition.x > -2)
                {
                    Debug.Log("Slide Left!");
                    transform.Translate(Vector3.left * 2);
                    canSidestep = false;
                }
            }
            else if (velocityX < 0.05f && velocityX > -0.05f) canSidestep = true;
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
            transform.localScale = initialScale;
            canSlide = true;
            canJump = true;
        }

        private void OnCollisionEnter(Collision collision)
        {
            OnCollision?.Invoke(collision);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawRay(new Vector3(transform.position.x, 0.1f, transform.position.z), Vector3.down * 0.2f);
        }
    }
}