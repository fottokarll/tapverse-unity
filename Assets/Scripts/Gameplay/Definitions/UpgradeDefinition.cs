using Tapverse.Core;
using UnityEngine;

namespace Tapverse.Gameplay.Definitions
{
    public enum UpgradeEffectType
    {
        TapMultiplier,
        GeneratorEfficiency,
        CritChance,
        CritPower,
        AutoTap
    }

    [CreateAssetMenu(menuName = "Tapverse/Upgrade Definition")]
    public class UpgradeDefinition : ScriptableObject
    {
        public string Id;
        public string DisplayName;
        [TextArea]
        public string Description;
        public BigDouble BaseCost = BigDouble.FromDouble(10);
        public float CostGrowth = 1.15f;
        public UpgradeEffectType EffectType;
        public double EffectValue;
        public int MaxLevel = 100;
    }
}
