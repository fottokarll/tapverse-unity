using System.Collections.Generic;
using TapVerse.Core;
using TapVerse.Gameplay.Definitions;
using UnityEngine;

namespace TapVerse.Gameplay
{
    public class UpgradeManager : GameServiceBehaviour<UpgradeManager>
    {
        private readonly Dictionary<string, int> _levels = new Dictionary<string, int>();
        private UpgradeCatalog _catalog;
        private CurrencyManager _currencyManager;
        private SaveManager _saveManager;
        private GeneratorManager _generatorManager;

        public double GeneratorEfficiencyMultiplier { get; private set; } = 1d;
        public bool AutoTapUnlocked { get; private set; } = false;

        public void Initialize(UpgradeCatalog catalog)
        {
            _catalog = catalog;
            _currencyManager = ServiceLocator.Resolve<CurrencyManager>();
            _saveManager = ServiceLocator.Resolve<SaveManager>();
            _generatorManager = ServiceLocator.Resolve<GeneratorManager>();

            _levels.Clear();
            foreach (var upgrade in catalog.Upgrades)
            {
                _levels[upgrade.Id] = 0;
            }

            var save = _saveManager.Data;
            if (save.Upgrades != null)
            {
                foreach (var saved in save.Upgrades)
                {
                    if (_levels.ContainsKey(saved.Id))
                    {
                        _levels[saved.Id] = saved.Level;
                    }
                }
            }

            ApplyEffects();
            GameEvents.RaiseUpgradesChanged();
        }

        public int GetLevel(string upgradeId)
        {
            return _levels.TryGetValue(upgradeId, out var level) ? level : 0;
        }

        public BigDouble GetCost(UpgradeDefinition definition)
        {
            int level = GetLevel(definition.Id);
            var growth = Mathf.Pow(definition.CostGrowth, level);
            return definition.BaseCost * growth;
        }

        public bool CanPurchase(UpgradeDefinition definition)
        {
            return GetLevel(definition.Id) < definition.MaxLevel;
        }

        public bool TryPurchase(UpgradeDefinition definition)
        {
            if (!CanPurchase(definition))
            {
                return false;
            }

            var cost = GetCost(definition);
            if (!_currencyManager.SpendCurrency(cost))
            {
                return false;
            }

            _levels[definition.Id]++;
            SaveState();
            ApplyEffects();
            GameEvents.RaiseUpgradesChanged();
            ServiceLocator.Resolve<AnalyticsRouter>().TrackUpgrade(definition.Id);
            return true;
        }

        private void ApplyEffects()
        {
            double tapMultiplier = 1d;
            double generatorEfficiency = 1d;
            double critChance = 0.03d;
            double critPower = 1.5d;
            bool autoTap = false;

            foreach (var upgrade in _catalog.Upgrades)
            {
                int level = GetLevel(upgrade.Id);
                if (level <= 0) continue;

                double totalEffect = upgrade.EffectValue * level;
                switch (upgrade.EffectType)
                {
                    case UpgradeEffectType.TapMultiplier:
                        tapMultiplier += totalEffect;
                        break;
                    case UpgradeEffectType.GeneratorEfficiency:
                        generatorEfficiency += totalEffect;
                        break;
                    case UpgradeEffectType.CritChance:
                        critChance += totalEffect;
                        break;
                    case UpgradeEffectType.CritPower:
                        critPower += totalEffect;
                        break;
                    case UpgradeEffectType.AutoTap:
                        autoTap = level > 0;
                        break;
                }
            }

            GeneratorEfficiencyMultiplier = generatorEfficiency;
            AutoTapUnlocked = autoTap;
            _currencyManager.ApplyMultipliers(tapMultiplier, critChance, critPower);
            _generatorManager?.ApplyEfficiencyMultiplier(generatorEfficiency);
        }

        private void SaveState()
        {
            var data = _saveManager.Data;
            data.Upgrades.Clear();
            foreach (var kvp in _levels)
            {
                data.Upgrades.Add(new UpgradeStateData
                {
                    Id = kvp.Key,
                    Level = kvp.Value
                });
            }
        }

        public void ResetForPrestige()
        {
            var keys = new List<string>(_levels.Keys);
            foreach (var key in keys)
            {
                _levels[key] = 0;
            }

            SaveState();
            ApplyEffects();
            GameEvents.RaiseUpgradesChanged();
        }

        public IEnumerable<UpgradeStateData> SerializeState()
        {
            foreach (var kvp in _levels)
            {
                yield return new UpgradeStateData
                {
                    Id = kvp.Key,
                    Level = kvp.Value
                };
            }
        }
    }
}
