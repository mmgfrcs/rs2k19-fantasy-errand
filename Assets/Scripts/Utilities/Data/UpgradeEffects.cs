using Newtonsoft.Json;

namespace FantasyErrand.Utilities
{
    [System.Serializable]
    public class UpgradeEffects
    {

        public float[] MagnetDuration { get; set; }

        public float[] MagnetRange { get; set; }

        public float[] PhaseDuration { get; set; }

        public float[] BoostDuration { get; set; }

        public float[] GoldenCoinDuration { get; set; }

        public float[] MultiplierIncrease { get; set; }

        public CoinValueUpgradeEffect[] CoinValueUpgrades { get; set; }
        public LivesUpgradeEffect[] LivesUpgrades { get; set; }

        internal static string DumpToJson()
        {
            UpgradeEffects effects = new UpgradeEffects();
            effects.MagnetDuration = new float[] { 1f, 2f };
            effects.MagnetRange = new float[] { 1f };
            effects.PhaseDuration = new float[] { 1f };
            effects.BoostDuration = new float[] { 1f };
            effects.GoldenCoinDuration = new float[] { 1f };
            effects.MultiplierIncrease = new float[] { 1f };
            effects.CoinValueUpgrades = new CoinValueUpgradeEffect[] { new CoinValueUpgradeEffect() { GoldDistance = 1f, SilverDistance = 1f, PlatinumDistance = float.PositiveInfinity } };
            effects.LivesUpgrades = new LivesUpgradeEffect[] { new LivesUpgradeEffect() { ContinueCoinCost = 1f, ContinueCostMultiplier = 1f, HurdleLives = 1 }, new LivesUpgradeEffect() { ContinueCoinCost = 2f, ContinueCostMultiplier = 1f, HurdleLives = 1 } };
            return JsonConvert.SerializeObject(effects, Formatting.Indented);
        }
    }
}

