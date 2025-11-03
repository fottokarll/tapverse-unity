using System.Collections.Generic;
using System.IO;
using System.Text;
using TapVerse.Gameplay.Definitions;
using UnityEngine;

namespace TapVerse.Services
{
    public static class TuningService
    {
        private class TuningPayload
        {
            public List<UpgradeDefinition> Upgrades;
            public List<GeneratorDefinition> Generators;
            public List<TierDefinition> Tiers;
        }

        public static void Export(string path, UpgradeCatalog upgrades, GeneratorCatalog generators, TierCatalog tiers)
        {
            var payload = new TuningPayload
            {
                Upgrades = upgrades.Upgrades,
                Generators = generators.Generators,
                Tiers = tiers.Tiers
            };

            var json = JsonUtility.ToJson(payload, true);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, json, Encoding.UTF8);
        }

        public static void Import(string path, UpgradeCatalog upgrades, GeneratorCatalog generators, TierCatalog tiers)
        {
            if (!File.Exists(path))
            {
                Debug.LogWarning($"Tuning file not found: {path}");
                return;
            }

            var json = File.ReadAllText(path, Encoding.UTF8);
            var payload = JsonUtility.FromJson<TuningPayload>(json);
            if (payload == null)
            {
                Debug.LogError("Failed to parse tuning payload");
                return;
            }

            upgrades.Upgrades = payload.Upgrades;
            generators.Generators = payload.Generators;
            tiers.Tiers = payload.Tiers;
        }
    }
}
