using System.Collections.Generic;
using UnityEngine;

namespace TapVerse.Gameplay.Definitions
{
    [CreateAssetMenu(menuName = "TapVerse/Upgrade Catalog")]
    public class UpgradeCatalog : ScriptableObject
    {
        public List<UpgradeDefinition> Upgrades = new List<UpgradeDefinition>();
    }
}
