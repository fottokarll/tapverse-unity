using System.Collections.Generic;
using UnityEngine;

namespace TapVerse.Gameplay.Definitions
{
    [CreateAssetMenu(menuName = "TapVerse/Tier Catalog")]
    public class TierCatalog : ScriptableObject
    {
        public List<TierDefinition> Tiers = new List<TierDefinition>();
    }
}
