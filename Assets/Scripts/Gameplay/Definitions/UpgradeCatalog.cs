using System.Collections.Generic;
using UnityEngine;

namespace Tapverse.Gameplay.Definitions
{
    [CreateAssetMenu(menuName = "Tapverse/Upgrade Catalog")]
    public class UpgradeCatalog : ScriptableObject
    {
        public List<UpgradeDefinition> Upgrades = new List<UpgradeDefinition>();
    }
}
