using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FantasyErrand.Entities;
using FantasyErrand.Utilities;
using UnityEngine.UI;
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

        private bool boostPhase = false;
        private float boostPhaseDuration = 2f;

        [Header("Magnet Attribute")]
        private float magnetRange;
        public int magnetSpeed = 8;
        private float magnetDuration;
        private int magnetLevel;
        private bool resetMagnet=false;
        private bool magnetStarted=false;
        private float magnetTime;
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

        

        public Slider magnetSlider;
        public Slider boostSlider;
        public Slider phaseSlider;
        public Slider goldenCoinSlider;

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
            SetPowerUpsBar();
        }

        void SetPowerUpsBar()
        {
            //Magnet Power
            if (player.magnetTime == 0)
            {
                magnetSlider.value = 0;
            }

            else
            {
                magnetSlider.value = 1 - (player.magnetTime / magnetDuration);
            }
                
            ///Phasing power
            if (player.phaseTime <= 0)
            {
                phaseSlider.value = 0;
            }
            else if (boostPhase)
            {
                phaseSlider.value = 1 - (player.phaseTime / boostPhaseDuration);
                //if (player.phaseTime>=1.9)
                //{
                //    phaseSlider.value = 0;
                //    player.phaseTime = 0;
                //    boostPhase = false;
                //}
            }
            else
            {
                phaseSlider.value = 1 - (player.phaseTime / phaseDuration);
                boostPhase = false;
            }
            //Gold coin
            if (player.goldenCoinTime == 0)
                goldenCoinSlider.value = 0;
            else
                goldenCoinSlider.value = 1 - (player.goldenCoinTime / goldenCoinDuration);

            //Boost Power
            if (player.boostTime == 0)
                boostSlider.value = 0;
            else
                boostSlider.value = 1- (player.boostTime / boostDuration);


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
            boostPhase = true;
            player.StartBoostPowerUps(boostDuration,boostPhaseDuration);
            
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
            else
                player.StartPhasePowerUps(phaseDuration);
        }

        public void SetPhaseEffect()
        {
            phaseLevel = GameDataManager.instance.Data.UpgradeLevels.PhaseLevel;
            phaseDuration = GameDataManager.instance.UpgradeEffects.PhaseDuration[phaseLevel];
        }
        public void SetBoostEffect()
        {
            int boostLvl = GameDataManager.instance.Data.UpgradeLevels.BoostLevel;
            boostDuration = GameDataManager.instance.UpgradeEffects.BoostDuration[boostLvl];
        }
        public void SetGoldenCoinEffect()
        {
            int level = GameDataManager.instance.Data.UpgradeLevels.GoldenCoinLevel;
            goldenCoinDuration = GameDataManager.instance.UpgradeEffects.GoldenCoinDuration[level];
        }
        public void SetMagnetEffect()
        {
            magnetLevel = GameDataManager.instance.Data.UpgradeLevels.MagnetLevel;
            magnetDuration= GameDataManager.instance.UpgradeEffects.MagnetDuration[magnetLevel];
            magnetRange = GameDataManager.instance.UpgradeEffects.MagnetRange[magnetLevel];
        }
        
        IEnumerator ActivateTemporaryPhasePower()
        {
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Obstacles"));
            yield return new WaitForSeconds(3f);
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Obstacles"), false);
        }

        
    }
}

