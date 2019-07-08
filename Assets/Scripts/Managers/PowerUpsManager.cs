using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FantasyErrand.Entities;
using FantasyErrand.Utilities;
using System;
using UnityEngine.UI;

namespace FantasyErrand
{
    public enum PowerUpsType {
        Magnet,
        Phase,
        Boost,
        GoldenCoin
    };

    public delegate void MagnetBroadcast(bool active, float duration, float range);
    public delegate void PhaseBroadcast(bool active, float duration);
    public delegate void BoostBroadcast(bool active, float duration, float multiplier);
    public delegate void GoldenCoinBroadcast(bool active, float duration);
    
    public class PowerUpsManager : MonoBehaviour
    {
        [SerializeField] private Player player;
        [SerializeField] private GameUIManager UIManager;
        [Header("Powerup Sprites"), SerializeField] private Sprite boostSprite;
        [SerializeField] private Sprite goldenCoinSprite, phaseSprite, magnetSprite;
        public static event MagnetBroadcast MagnetEffectChanged;
        public static event PhaseBroadcast PhaseEffectChanged;
        public static event BoostBroadcast BoostEffectChanged;
        public static event GoldenCoinBroadcast GoldenCoinEffectChanged;

        private bool boostPhase = false;
        private float boostPhaseDuration = 2f;

        public int magnetPullSpeed = 8;
        public float boostMultiplier = 2;

        public TimeSpan MagnetEndTime { get; private set; }
        public TimeSpan PhaseEndTime { get; private set; }
        public TimeSpan BoostEndTime { get; private set; }
        public TimeSpan GoldenCoinEndTime { get; private set; }

        List<PowerUpsType> powerUpsDisplayOrder = new List<PowerUpsType>() { PowerUpsType.Boost, PowerUpsType.GoldenCoin, PowerUpsType.Magnet, PowerUpsType.Phase };
        private bool restartPhase;

        // Use this for initialization
        void Start()
        {
            player.OnGetPowerUps += Player_OnGetPowerUps;
            GameManager.OnGameStart += GameManager_OnGameStart;
            GameManager.OnGameEnd += GameManager_OnGameEnd;
        }

        private void GameManager_OnGameStart(bool restarted)
        {
            if(restarted)
            {
                restartPhase = true;
                PhaseEndTime = TimeSpan.FromSeconds(3);
                StartCoroutine(PhaseEffect(3));
            }
        }

        private void Player_OnGetPowerUps(PowerUpsType type)
        {
            switch(type)
            {
                case PowerUpsType.Magnet:
                    {
                        powerUpsDisplayOrder.Remove(PowerUpsType.Magnet);
                        powerUpsDisplayOrder.Insert(0, PowerUpsType.Magnet);
                        int level = GameDataManager.instance.Data.UpgradeLevels.MagnetLevel;
                        float duration = GameDataManager.instance.UpgradeEffects.MagnetDuration[level];
                        float range = GameDataManager.instance.UpgradeEffects.MagnetRange[level];

                        if(MagnetEndTime <= TimeSpan.Zero)
                        {
                            MagnetEndTime = TimeSpan.FromSeconds(duration);
                            MagnetEffectChanged?.Invoke(true, duration, range);
                            StartCoroutine(MagnetEffect());
                        }
                        else MagnetEndTime = TimeSpan.FromSeconds(duration);
                        break;
                    }
                case PowerUpsType.Boost:
                    {
                        powerUpsDisplayOrder.Remove(PowerUpsType.Boost);
                        powerUpsDisplayOrder.Insert(0, PowerUpsType.Boost);
                        int level = GameDataManager.instance.Data.UpgradeLevels.BoostLevel;
                        float duration = GameDataManager.instance.UpgradeEffects.BoostDuration[level];

                        if(BoostEndTime <= TimeSpan.FromSeconds(2))
                        {
                            BoostEndTime = TimeSpan.FromSeconds(duration + 3);
                            StartCoroutine(BoostEffect(duration));
                        }
                        else BoostEndTime = TimeSpan.FromSeconds(duration + 2);

                        break;
                    }
                case PowerUpsType.GoldenCoin:
                    {
                        powerUpsDisplayOrder.Remove(PowerUpsType.GoldenCoin);
                        powerUpsDisplayOrder.Insert(0, PowerUpsType.GoldenCoin);
                        int level = GameDataManager.instance.Data.UpgradeLevels.GoldenCoinLevel;
                        float duration = GameDataManager.instance.UpgradeEffects.GoldenCoinDuration[level];

                        if (GoldenCoinEndTime <= TimeSpan.Zero)
                        {
                            GoldenCoinEndTime = TimeSpan.FromSeconds(duration);
                            StartCoroutine(GoldenCoinEffect(duration));
                        }
                        else GoldenCoinEndTime = TimeSpan.FromSeconds(duration);

                        break;
                    }
                case PowerUpsType.Phase:
                    {
                        restartPhase = false;
                        powerUpsDisplayOrder.Remove(PowerUpsType.Phase);
                        powerUpsDisplayOrder.Insert(0, PowerUpsType.Phase);
                        int level = GameDataManager.instance.Data.UpgradeLevels.PhaseLevel;
                        float duration = GameDataManager.instance.UpgradeEffects.PhaseDuration[level];

                        if (PhaseEndTime <= TimeSpan.Zero)
                        {
                            PhaseEndTime = TimeSpan.FromSeconds(duration);
                            StartCoroutine(PhaseEffect(duration));
                        }
                        else PhaseEndTime = TimeSpan.FromSeconds(duration);
                        break;
                    }
            }
        }

        private void OnDrawGizmosSelected()
        {
            int magnetLevel = GameDataManager.instance.Data.UpgradeLevels.MagnetLevel;
            float magnetRange = GameDataManager.instance.UpgradeEffects.MagnetRange[magnetLevel];
            Gizmos.DrawWireSphere(player.transform.position, magnetRange);
        }

        private void GameManager_OnGameEnd(GameEndEventArgs args)
        {

            if (MagnetEndTime > TimeSpan.Zero) MagnetEffectChanged?.Invoke(false, 0, 0);
            if (PhaseEndTime > TimeSpan.Zero) PhaseEffectChanged?.Invoke(false, 0);
            if (BoostEndTime > TimeSpan.Zero) BoostEffectChanged?.Invoke(false, 0, 0);
            if (GoldenCoinEndTime > TimeSpan.Zero) GoldenCoinEffectChanged?.Invoke(false, 0);

            MagnetEndTime = TimeSpan.Zero;
            PhaseEndTime = TimeSpan.Zero;
            BoostEndTime = TimeSpan.Zero;
            GoldenCoinEndTime = TimeSpan.Zero;            
        }

        void Update()
        {
            if (MagnetEndTime > TimeSpan.Zero) MagnetEndTime = MagnetEndTime.Subtract(TimeSpan.FromSeconds(Time.deltaTime));
            if (PhaseEndTime > TimeSpan.Zero) PhaseEndTime = PhaseEndTime.Subtract(TimeSpan.FromSeconds(Time.deltaTime));
            if (BoostEndTime > TimeSpan.Zero) BoostEndTime = BoostEndTime.Subtract(TimeSpan.FromSeconds(Time.deltaTime));
            if (GoldenCoinEndTime > TimeSpan.Zero) GoldenCoinEndTime = GoldenCoinEndTime.Subtract(TimeSpan.FromSeconds(Time.deltaTime));

            if(BoostEndTime <= TimeSpan.Zero && PhaseEndTime <= TimeSpan.Zero) 
                Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Obstacles"), false);

            Image[] pwImages = UIManager.GetUIArray<Image>(GameUIManager.UIType.PowerupImageArray);
            Slider[] pwSliders = UIManager.GetUIArray<Slider>(GameUIManager.UIType.PowerupSliderArray);

            for(int i = 0; i<Mathf.Min(pwImages.Length, pwSliders.Length, powerUpsDisplayOrder.Count); i++)
            {
                pwImages[i].gameObject.SetActive(true);
                if(powerUpsDisplayOrder[i] == PowerUpsType.Boost && BoostEndTime > TimeSpan.Zero)
                {
                    pwImages[i].gameObject.SetActive(true);
                    pwImages[i].sprite = boostSprite;
                    pwSliders[i].maxValue = GameDataManager.instance.UpgradeEffects.BoostDuration[GameDataManager.instance.Data.UpgradeLevels.BoostLevel] + 3;
                    pwSliders[i].value = (float)BoostEndTime.TotalSeconds;
                }
                else if (powerUpsDisplayOrder[i] == PowerUpsType.GoldenCoin && GoldenCoinEndTime > TimeSpan.Zero)
                {
                    pwImages[i].gameObject.SetActive(true);
                    pwImages[i].sprite = goldenCoinSprite;
                    pwSliders[i].maxValue = GameDataManager.instance.UpgradeEffects.GoldenCoinDuration[GameDataManager.instance.Data.UpgradeLevels.GoldenCoinLevel];
                    pwSliders[i].value = (float)GoldenCoinEndTime.TotalSeconds;
                }
                else if (powerUpsDisplayOrder[i] == PowerUpsType.Magnet && MagnetEndTime > TimeSpan.Zero)
                {
                    pwImages[i].gameObject.SetActive(true);
                    pwImages[i].sprite = magnetSprite;
                    pwSliders[i].maxValue = GameDataManager.instance.UpgradeEffects.MagnetDuration[GameDataManager.instance.Data.UpgradeLevels.MagnetLevel];
                    pwSliders[i].value = (float)MagnetEndTime.TotalSeconds;
                }
                else if (powerUpsDisplayOrder[i] == PowerUpsType.Phase && PhaseEndTime > TimeSpan.Zero)
                {
                    pwImages[i].gameObject.SetActive(true);
                    pwImages[i].sprite = phaseSprite;
                    pwSliders[i].maxValue = restartPhase ? 3 : GameDataManager.instance.UpgradeEffects.PhaseDuration[GameDataManager.instance.Data.UpgradeLevels.PhaseLevel];
                    pwSliders[i].value = (float)PhaseEndTime.TotalSeconds;
                }
                else pwImages[i].gameObject.SetActive(false);
            }
        }

        private IEnumerator MagnetEffect()
        {
            while(MagnetEndTime > TimeSpan.Zero)
            {
                int magnetLevel = GameDataManager.instance.Data.UpgradeLevels.MagnetLevel;
                float magnetRange = GameDataManager.instance.UpgradeEffects.MagnetRange[magnetLevel];

                RaycastHit[] hits = Physics.SphereCastAll(player.transform.position, magnetRange, player.transform.forward, magnetRange, LayerMask.GetMask("Default"), QueryTriggerInteraction.UseGlobal);
                foreach (RaycastHit hit in hits)
                {
                    GameObject currObj = hit.transform.gameObject;
                    CollectibleBase collect = currObj.GetComponent<CollectibleBase>();
                    if (collect != null && collect.CollectibleType == CollectibleType.Monetary)
                    {
                        Vector3 dir = collect.transform.position - player.transform.position;
                        collect.transform.position = Vector3.MoveTowards(collect.transform.position, player.transform.position, magnetPullSpeed * Time.deltaTime);
                    }
                }
                
                yield return null;
            }

            MagnetEffectChanged?.Invoke(false, 0, 0);
        }

        IEnumerator BoostEffect(float duration)
        {
            SoundManager.Instance.PlaySound("Boost");
            BoostEffectChanged?.Invoke(true, duration + 3, boostMultiplier);
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Obstacles"));
            yield return new WaitForSeconds(1f);
            while(BoostEndTime > TimeSpan.FromSeconds(2))
            {
                Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Obstacles"));
                yield return null;
            }
            BoostEffectChanged?.Invoke(false, duration + 3, boostMultiplier);
            yield return new WaitForSeconds(2f);
            //Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Obstacles"), false);
        }

        IEnumerator GoldenCoinEffect(float duration)
        {
            GoldenCoinEffectChanged?.Invoke(true, duration);
            while (GoldenCoinEndTime > TimeSpan.Zero)
            {
                yield return null;
            }
            GoldenCoinEffectChanged?.Invoke(false, duration);
        }

        IEnumerator PhaseEffect(float duration)
        {
            PhaseEffectChanged?.Invoke(true, duration);
            while (PhaseEndTime > TimeSpan.Zero)
            {
                Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Obstacles"));
                yield return null;
            }
            PhaseEffectChanged?.Invoke(false, duration);
            restartPhase = false;
        }
        
        IEnumerator ActivateTemporaryPhasePower()
        {
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Obstacles"));
            yield return new WaitForSeconds(3f);
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Obstacles"), false);
        }

        
    }
}

