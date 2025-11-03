using System.Collections;
using NUnit.Framework;
using TapVerse.Core;
using TapVerse.Gameplay;
using TapVerse.Gameplay.Definitions;
using TapVerse.Services;
using UnityEngine;
using UnityEngine.TestTools;

namespace TapVerse.Tests
{
    public class TapversePlayModeTests
    {
        private GameObject _root;
        private SaveManager _saveManager;
        private CurrencyManager _currencyManager;
        private UpgradeManager _upgradeManager;
        private GeneratorManager _generatorManager;
        private TierManager _tierManager;
        private PrestigeManager _prestigeManager;
        private AnalyticsRouter _analyticsRouter;
        private UpgradeCatalog _upgradeCatalog;
        private GeneratorCatalog _generatorCatalog;
        private TierCatalog _tierCatalog;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            ServiceLocator.Clear();
            _root = new GameObject("SystemsTestRoot");
            _saveManager = _root.AddComponent<SaveManager>();
            _currencyManager = _root.AddComponent<CurrencyManager>();
            _upgradeManager = _root.AddComponent<UpgradeManager>();
            _generatorManager = _root.AddComponent<GeneratorManager>();
            _tierManager = _root.AddComponent<TierManager>();
            _prestigeManager = _root.AddComponent<PrestigeManager>();
            _analyticsRouter = _root.AddComponent<AnalyticsRouter>();
            _root.AddComponent<FeedbackManager>();
            _root.AddComponent<AudioService>();
            _root.AddComponent<HapticsService>();

            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            Object.DestroyImmediate(_root);
            ServiceLocator.Clear();
            yield return null;
        }

        private UpgradeCatalog CreateUpgradeCatalog()
        {
            var catalog = ScriptableObject.CreateInstance<UpgradeCatalog>();
            var upgrade = ScriptableObject.CreateInstance<UpgradeDefinition>();
            upgrade.Id = "test_tap";
            upgrade.DisplayName = "Test Tap";
            upgrade.BaseCost = BigDouble.FromDouble(1);
            upgrade.CostGrowth = 1f;
            upgrade.EffectType = UpgradeEffectType.TapMultiplier;
            upgrade.EffectValue = 1d;
            upgrade.MaxLevel = 5;
            catalog.Upgrades.Add(upgrade);
            return catalog;
        }

        private GeneratorCatalog CreateGeneratorCatalog()
        {
            var catalog = ScriptableObject.CreateInstance<GeneratorCatalog>();
            var generator = ScriptableObject.CreateInstance<GeneratorDefinition>();
            generator.Id = "test_gen";
            generator.DisplayName = "Test Generator";
            generator.BaseCost = BigDouble.FromDouble(1);
            generator.CostGrowth = 1f;
            generator.BaseProduction = BigDouble.FromDouble(2);
            catalog.Generators.Add(generator);
            return catalog;
        }

        private TierCatalog CreateTierCatalog()
        {
            var catalog = ScriptableObject.CreateInstance<TierCatalog>();
            var tier = ScriptableObject.CreateInstance<TierDefinition>();
            tier.TierIndex = 0;
            tier.DisplayName = "Void";
            tier.UnlockThreshold = 0;
            catalog.Tiers.Add(tier);
            return catalog;
        }

        private IEnumerator InitializeSystems()
        {
            _upgradeCatalog = CreateUpgradeCatalog();
            _generatorCatalog = CreateGeneratorCatalog();
            _tierCatalog = CreateTierCatalog();

            _generatorManager.Initialize(_generatorCatalog);
            _upgradeManager.Initialize(_upgradeCatalog);
            _tierManager.Initialize(_tierCatalog);
            _prestigeManager.Initialize();
            _currencyManager.Initialize();

            yield return null;
        }

        [UnityTest]
        public IEnumerator TapAddsCurrency()
        {
            yield return InitializeSystems();
            var before = _currencyManager.Current;
            _currencyManager.RegisterTap();
            Assert.Greater(_currencyManager.Current, before);
        }

        [UnityTest]
        public IEnumerator BuyingUpgradeIncreasesTapValue()
        {
            yield return InitializeSystems();
            var baseValue = _currencyManager.EvaluateTapValue(false);
            _currencyManager.AddCurrency(BigDouble.FromDouble(10), false);
            _upgradeManager.TryPurchase(_upgradeCatalog.Upgrades[0]);
            var boosted = _currencyManager.EvaluateTapValue(false);
            Assert.Greater(boosted.ToDouble(), baseValue.ToDouble());
        }

        [UnityTest]
        public IEnumerator GeneratorProducesIdleIncome()
        {
            yield return InitializeSystems();
            var generatorDefinition = _generatorCatalog.Generators[0];
            _currencyManager.AddCurrency(BigDouble.FromDouble(10), false);
            _generatorManager.TryPurchase(generatorDefinition);
            var production = _generatorManager.EvaluateProductionPerSecond();
            Assert.Greater(production.ToDouble(), 0d);
        }

        [UnityTest]
        public IEnumerator PrestigePreviewReturnsShardsWhenThresholdMet()
        {
            yield return InitializeSystems();
            _currencyManager.AddCurrency(BigDouble.FromDouble(20000), false);
            var preview = _prestigeManager.GetPreview();
            Assert.Greater(preview.AdditionalShards.ToDouble(), 0d);
        }
    }
}
