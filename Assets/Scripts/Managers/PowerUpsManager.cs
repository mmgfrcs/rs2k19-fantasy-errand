using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FantasyErrand.Entities;
using FantasyErrand.Utilities;

namespace FantasyErrand
{
    public enum PowerUpsType {
        Magnet,
        Phase,
        Boost,
        GoldenCoin
    };
    public delegate void MagnetBroadcast(bool mangetActive, int magnetRange, int magnetSpeed);
    
    
    public class PowerUpsManager : MonoBehaviour
    {
        [SerializeField]
        private Player player;
        public static event MagnetBroadcast magnetBroadcast;
        
        

        [Header("Magnet Attribute")]
        private int magnetRange;
        public int magnetSpeed = 8;
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

        private int gameManagerBroadcastCount = 0;
        // Use this for initialization
        void Start()
        {
            //<-------------Magnet Part----------->
            SetMagnetEffect();
            MagnetCollectible.TurnMagnet += StartMagnetPowerUps;

            //<-------------Phasing Part----------->
            SetPhaseEffect();
            PhaseCollectible.TurnPhasing += StartPhasePowerUps;

            //<-------------Boost Part----------->
            SetBoostEffect();
            BoostCollectible.TurnBoost += StartBoostPowerUps;

            SetGoldenCoinEffect();
            GoldenCoinCollectible.TurnGoldenCoin += StartGoldenCoinPowerUps;

            GameManager.OnGameStart += StartTemporaryPhasePower;
        }

        void Update()
        {

        }

        public void StartMagnetPowerUps()
        {
            player.StartMagnetPowerUps(magnetDuration,magnetRange);
        }

        public void StartPhasePowerUps()
        {
            player.StartPhasePowerUps(phaseDuration);
        }

        public void StartBoostPowerUps()
        {
            player.StartBoostPowerUps(boostDuration);
        }

        public void StartGoldenCoinPowerUps()
        {
            player.StartGoldenCoinPowerUps(goldenCoinDuration);
        }

        public void StartTemporaryPhasePower()
        {
            if (gameManagerBroadcastCount == 0)
            {
                gameManagerBroadcastCount++;
                return;
            }
            else StartCoroutine(ActivateTemporaryPhasePower());
        }

        public void SetPhaseEffect()
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
        public void SetBoostEffect()
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
        public void SetGoldenCoinEffect()
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
        public void SetMagnetEffect()
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
        
        IEnumerator ActivateTemporaryPhasePower()
        {
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Obstacles"));
            yield return new WaitForSeconds(3f);
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Obstacles"), false);
        }

        
    }
}

