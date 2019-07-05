using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FantasyErrand.Entities;
using FantasyErrand.Utilities;
using System;

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

        // Use this for initialization
        void Start()
        {

        }

        private void Player_OnGetPowerUps(PowerUpsType type)
        {
            switch(type)
            {
                case PowerUpsType.Magnet:
                    {
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
                        int level = GameDataManager.instance.Data.UpgradeLevels.BoostLevel;
                        float duration = GameDataManager.instance.UpgradeEffects.BoostDuration[level];

                        if(BoostEndTime <= TimeSpan.Zero)
                        {
                            BoostEndTime = TimeSpan.FromSeconds(duration + 3);
                            StartCoroutine(BoostEffect(duration));
                        }
                        else BoostEndTime = TimeSpan.FromSeconds(duration + 2);

                        break;
                    }
                case PowerUpsType.GoldenCoin:
                    {
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
            if (MagnetEndTime > TimeSpan.Zero) MagnetEndTime.Subtract(TimeSpan.FromSeconds(Time.deltaTime));
            if (PhaseEndTime > TimeSpan.Zero) PhaseEndTime.Subtract(TimeSpan.FromSeconds(Time.deltaTime));
            if (BoostEndTime > TimeSpan.Zero) BoostEndTime.Subtract(TimeSpan.FromSeconds(Time.deltaTime));
            if (GoldenCoinEndTime > TimeSpan.Zero) GoldenCoinEndTime.Subtract(TimeSpan.FromSeconds(Time.deltaTime));

            if(BoostEndTime <= TimeSpan.Zero && PhaseEndTime <= TimeSpan.Zero) 
                Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Obstacles"), false);
        }

        private IEnumerator MagnetEffect()
        {
            while(MagnetEndTime > TimeSpan.Zero)
            {
                int magnetLevel = GameDataManager.instance.Data.UpgradeLevels.MagnetLevel;
                float magnetRange = GameDataManager.instance.UpgradeEffects.MagnetRange[magnetLevel];

                RaycastHit[] hits = Physics.SphereCastAll(transform.position, magnetRange, transform.forward, magnetRange, LayerMask.GetMask("Default"), QueryTriggerInteraction.UseGlobal);
                foreach (RaycastHit hit in hits)
                {
                    GameObject currObj = hit.transform.gameObject;
                    CollectibleBase collect = currObj.GetComponent<CollectibleBase>();
                    if (collect != null && collect.CollectibleType == CollectibleType.Monetary)
                    {
                        Vector3 dir = collect.transform.position - player.transform.position;
                        collect.transform.Translate(dir.normalized * magnetPullSpeed);
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
        }
        
        IEnumerator ActivateTemporaryPhasePower()
        {
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Obstacles"));
            yield return new WaitForSeconds(3f);
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Obstacles"), false);
        }

        
    }
}

