using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FantasyErrand.Entities;
using FantasyErrand.Utilities;

namespace FantasyErrand
{

    public enum PowerUpsType {
        magnet,phase,coin
    };
    public delegate void MagnetBroadcast(bool mangetActive, int magnetRange, int magnetSpeed);
    
    public class PowerUpsManager : MonoBehaviour
    {
        public static event MagnetBroadcast magnetBroadcast;
        public GameObject gameDataManager;
        public GameObject gameManager;
        [Header("Magnet Attribute")]
        private int magnetRange;
        public int magnetSpeed=8;
        private float magnetDuration;
        private int magnetLevel;
        [Header("Phase Attribute")]
        private int phaseLevel;
        private float phaseDuration;
        [Header("Boost Attribute")]
        private float boostDuration;
        // Use this for initialization
        void Start()
        {
            //<-------------Magnet Part----------->
            SetMagnet();
            MagnetCollectible.TurnMagnet += StartMagnetPowerUps;

            //<-------------Phasing Part----------->
            SetPhase();
            PhaseCollectible.TurnPhasing += StartPhasePowerUps;
            SetBoost();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void StartMagnetPowerUps()
        {
            print("Magnet Starts baby");
            StartCoroutine(MagnetPower());
        }

        public void StartPhasePowerUps()
        {
            StartCoroutine(PhasePower());
        }

        public void StartBoostPowerUps()
        {
            StartCoroutine(BoostPower());
        }



        public void SetPhase()
        {
            phaseLevel = gameDataManager.GetComponent<GameDataManager>().Data.UpgradeLevels.PhaseLevel;
            switch (phaseLevel)
            {
                case 0:
                    phaseDuration = 3;
                    break;
                case 1:
                    phaseDuration = 3.75f;
                    break;
                case 2:
                    phaseDuration = 4.5f;
                    break;

                case 3:
                    phaseDuration = 5.25f;
                    break;

                case 4:
                    phaseDuration = 6;
                    break;
                case 5:
                    phaseDuration = 7;
                    break;
                default:
                    phaseDuration = 3;
                    break;
            }


        }


        public void SetBoost()
        {
            int boostLvl = gameDataManager.GetComponent<GameDataManager>().Data.UpgradeLevels.BoostLevel;
            switch (boostLvl)
            {
                case 0:
                    boostDuration = 4;
                    break;

                case 1:
                    boostDuration = 5;
                    break;

                case 2:
                    boostDuration = 6;
                    break;

                case 3:
                    boostDuration = 7;
                    break;

                case 4:
                    boostDuration = 8;
                    break;

                case 5:
                    boostDuration = 10;
                    break;
            }

        }

        public void SetMagnet()
        {
            magnetLevel = gameDataManager.GetComponent<GameDataManager>().Data.UpgradeLevels.MagnetLevel;
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


        IEnumerator MagnetPower()
        {
            float currTime = 0;
            while (currTime < magnetDuration)
            {
                magnetBroadcast(true, magnetRange, magnetSpeed);
                yield return new WaitForSeconds(0.25f);
                currTime += 0.25f;
            }
            magnetBroadcast(false, magnetRange, magnetSpeed);
        }


        IEnumerator PhasePower()
        {
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Obstacles"));
            yield return new WaitForSeconds(phaseDuration);
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Obstacles"),false);
        }

        IEnumerator BoostPower()
        {
            gameManager.GetComponent<GameManager>().SetPlayerSpeed(5,true);
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Obstacles"));
            print("Boost activated");
            yield return new WaitForSeconds(boostDuration);
            gameManager.GetComponent<GameManager>().SetPlayerSpeed(0.2f,false);
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Obstacles"), false);
        }
    }
}

