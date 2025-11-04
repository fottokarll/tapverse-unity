using System.Collections.Generic;
using UnityEngine;

namespace Tapverse.Gameplay.Definitions
{
    [CreateAssetMenu(menuName = "Tapverse/Tier Catalog")]
    public class TierCatalog : ScriptableObject
    {
        public List<TierDefinition> Tiers = new List<TierDefinition>();
    }
}
