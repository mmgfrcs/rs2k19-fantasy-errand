using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FantasyErrand.Entities;

namespace FantasyErrand
{
    public class PowerUpsManager : MonoBehaviour
    {
        [Header("Magnet Attribute")]
        public bool magnetActivated;
        public int magnetRange;
        public int magnetSpeed;
        public float magnetDuration;
        public int magnetLevel;
        [Header("Phase Attribute")]
        public int phaseDuration;
        // Use this for initialization
        void Start()
        {
            setMagnet();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void startMagnetPowerUps()
        {
            StartCoroutine(magnetPower());
        }

        public void startPhasePowerUps()
        {
            StartCoroutine(phasePower());
        }


        public void setMagnet()
        {
            switch (magnetLevel)
            {
                case 0:
                    magnetRange = 50;
                    magnetDuration = 6;
                    break;
                case 1:
                    magnetRange = 50;
                    magnetDuration = 7.5f;
                    break;
                case 2:
                    magnetRange = 60;
                    magnetDuration = 9;
                    break;
                case 3:
                    magnetRange = 60;
                    magnetDuration = 10.5f;
                    break;
                case 4:
                    magnetRange = 70;
                    magnetDuration = 12;
                    break;
                case 5:
                    magnetRange = 70;
                    magnetDuration = 15f;
                    break;
            } 
        }


        IEnumerator magnetPower()
        {
            magnetActivated = true;
            yield return new WaitForSeconds(magnetDuration);
            magnetActivated = false;

        }


        IEnumerator phasePower()
        {
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Obstacles"));
            print("PHase working");
            yield return new WaitForSeconds(phaseDuration);
            print("PHase Stopped");
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Obstacles"),false);
        }
    }
}

