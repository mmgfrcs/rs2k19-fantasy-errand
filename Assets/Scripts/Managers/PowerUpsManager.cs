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
        public Image magnetBar;
        private float magnetTime;
        [Header("Phase Attribute")]
        private int phaseLevel;
        private float phaseDuration;
        private bool resetPhase=false;
        private bool phaseStarted=false;
        public Image phaseBar;
        [Header("Boost Attribute")]
        private float boostDuration;
        private bool resetBoost=false;
        private bool boostStarted = false;
        public Image boostBar;
        [Header("Golden Coin Attribute")]
        private float goldenCoinDuration;
        private bool resetGoldenCoin = false;
        private bool goldenCoinStarted = false;
        public Image goldenCoinBar;



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
            if (player.magnetTime == 0)
                magnetBar.GetComponent<Image>().fillAmount = 0;
            else
                magnetBar.GetComponent<Image>().fillAmount = 1 - (player.magnetTime / magnetDuration);

            if (player.phaseTime <= 0.1)
            {
                phaseBar.GetComponent<Image>().fillAmount = 0;
            }
            else if (boostPhase)
            {
                phaseBar.GetComponent<Image>().fillAmount = 1 - (player.phaseTime / boostPhaseDuration);
                if (player.phaseTime>=1.9)
                {
                    phaseBar.GetComponent<Image>().fillAmount = 0;
                    player.phaseTime = 0;
                    boostPhase = false;
                }
            }
            else
            {
                phaseBar.GetComponent<Image>().fillAmount = 1 - (player.phaseTime / phaseDuration);
                boostPhase = false;
            }

            if (player.goldenCoinTime == 0)
                goldenCoinBar.GetComponent<Image>().fillAmount = 0;
            else
                goldenCoinBar.GetComponent<Image>().fillAmount = 1 - (player.goldenCoinTime / goldenCoinDuration);

            if (player.boostTime == 0)
                boostBar.GetComponent<Image>().fillAmount = 0;
            else
                boostBar.GetComponent<Image>().fillAmount = 1 - (player.boostTime / boostDuration);


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

