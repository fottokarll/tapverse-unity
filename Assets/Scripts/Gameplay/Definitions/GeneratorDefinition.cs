using TapVerse.Core;
using UnityEngine;

namespace TapVerse.Gameplay.Definitions
{
    [CreateAssetMenu(menuName = "TapVerse/Generator Definition")]
    public class GeneratorDefinition : ScriptableObject
    {
        public string Id;
        public string DisplayName;
        [TextArea]
        public string Description;
        public BigDouble BaseCost = BigDouble.FromDouble(10);
        public float CostGrowth = 1.15f;
        public BigDouble BaseProduction = BigDouble.FromDouble(1);
    }
}
