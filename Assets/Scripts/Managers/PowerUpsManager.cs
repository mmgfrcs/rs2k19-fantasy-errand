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
    public delegate void GoldenCoinBroadcast(bool goldenCoinActive);
    public delegate void SpeedBroadcast(float multiplier);
    
    public class PowerUpsManager : MonoBehaviour
    {
        public static event MagnetBroadcast magnetBroadcast;
        public static event GoldenCoinBroadcast goldenCoinBroadcast;
        public static event SpeedBroadcast speedBroadcast;

        [Header("Magnet Attribute")]
        private int magnetRange;
        public int magnetSpeed=8;
        private float magnetDuration;
        private int magnetLevel;
        private bool resetMagnet=false;
        private bool magnetStarted=false;

        [Header("Phase Attribute")]
        private int phaseLevel;
        private float phaseDuration;
        private bool resetPhase=false;
        private bool phaseStarted=false;

        [Header("Boost Attribute")]
        private float boostDuration;
        private bool resetBoost=false;
        private bool boostStarted = false;

        [Header("Golden Coin Attribute")]
        private float goldenCoinDuration;
        private bool resetGoldenCoin = false;
        private bool goldenCoinStarted = false;
        // Use this for initialization
        void Start()
        {
            //<-------------Magnet Part----------->
            SetMagnet();
            MagnetCollectible.TurnMagnet += StartMagnetPowerUps;

            //<-------------Phasing Part----------->
            SetPhase();
            PhaseCollectible.TurnPhasing += StartPhasePowerUps;

            //<-------------Boost Part----------->
            SetBoost();
            BoostCollectible.TurnBoost += StartBoostPowerUps;

            setGoldenCoin();
            GoldenCoinCollectible.TurnGoldenCoin +=StartGoldenCoinPowerUps;
        }

        void Update()
        {

        }

        public void StartMagnetPowerUps()
        {
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

        public void StartGoldenCoinPowerUps()
        {
            StartCoroutine(GoldenCoinPower());
        }

        public void SetPhase()
        {
            phaseLevel = GameDataManager.instance.Data.UpgradeLevels.PhaseLevel;
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
            int boostLvl = GameDataManager.instance.Data.UpgradeLevels.BoostLevel;
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

        public void setGoldenCoin()
        {
            int level = GameDataManager.instance.Data.UpgradeLevels.GoldenCoinLevel;
            switch (level)
            {
                case 0:
                    goldenCoinDuration = 4;
                    break;
                case 1:
                    goldenCoinDuration = 6;
                    break;
                case 2:
                    goldenCoinDuration = 8;
                    break;
                case 3:
                    goldenCoinDuration = 10;
                    break;
                case 4:
                    goldenCoinDuration = 12;
                    break;
                case 5:
                    goldenCoinDuration = 14;
                    break;
            }


        }
        public void SetMagnet()
        {
            magnetLevel = GameDataManager.instance.Data.UpgradeLevels.MagnetLevel;
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
            if (!magnetStarted)
            {
                magnetStarted = true;
                float duration = magnetDuration;
                float timeStamp = Time.time;

                while (Time.time < timeStamp + duration)
                {
                    if (resetMagnet)
                    {
                        resetMagnet = false;
                        timeStamp = Time.time;
                    }
                    magnetBroadcast?.Invoke(true, magnetRange, magnetSpeed);
                    yield return new WaitForSeconds(0.25f);
                }
                magnetStarted = false;
                magnetBroadcast?.Invoke(false, magnetRange, magnetSpeed);
            }
            else
            {
                resetMagnet = true;
            }
        }


        IEnumerator PhasePower()
        {
            if (!phaseStarted)
            {
                phaseStarted = true;
                float duration = phaseDuration;
                float timeStamp = Time.time;
                Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Obstacles"));
                while (Time.time < timeStamp + duration)
                {
                    if (resetPhase)
                    {
                        resetPhase = false;
                        timeStamp = Time.time;
                    }
                    yield return null;
                }
                phaseStarted = false;
                Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Obstacles"), false);
            }
            else
            {
                resetPhase = true;
            }
        }

        IEnumerator BoostPower()
        {
            if (!boostStarted)
            {
                boostStarted = true;
                float duration = boostDuration;
                float timeStamp = Time.time;
                speedBroadcast?.Invoke(5);
                Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Obstacles"));
                while (Time.time < timeStamp + duration)
                {
                    if (resetBoost)
                    {
                        resetBoost = false;
                        timeStamp = Time.time;
                    }
                    yield return null;
                }
                boostStarted = false;
                speedBroadcast?.Invoke(1);
                Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Obstacles"), false);
            }
            else
            {
                resetBoost = true;
            }
            
        }

        IEnumerator GoldenCoinPower()
        {
            if (!goldenCoinStarted)
            {
                goldenCoinStarted = true;
                float duration = goldenCoinDuration;
                float timeStamp = Time.time;
                goldenCoinBroadcast?.Invoke(true);
                while (Time.time < timeStamp + duration)
                {
                    if (resetGoldenCoin)
                    {
                        resetGoldenCoin = false;
                        timeStamp = Time.time;
                    }
                    yield return null;
                }
                goldenCoinBroadcast?.Invoke(false);
                goldenCoinStarted = false;
            }
            else
            {
                resetGoldenCoin = true;
            }
        }
    }
}

