using TapVerse.Core;
using UnityEngine;

namespace TapVerse.Gameplay
{
    public class PrestigeManager : GameServiceBehaviour<PrestigeManager>
    {
        private CurrencyManager _currencyManager;
        private UpgradeManager _upgradeManager;
        private GeneratorManager _generatorManager;
        private TierManager _tierManager;
        private SaveManager _saveManager;

        private const double PrestigeThreshold = 1e4;

        public void Initialize()
        {
            _currencyManager = ServiceLocator.Resolve<CurrencyManager>();
            _upgradeManager = ServiceLocator.Resolve<UpgradeManager>();
            _generatorManager = ServiceLocator.Resolve<GeneratorManager>();
            _tierManager = ServiceLocator.Resolve<TierManager>();
            _saveManager = ServiceLocator.Resolve<SaveManager>();
        }

        public bool CanPrestige()
        {
            return _currencyManager.Lifetime.ToDouble() >= PrestigeThreshold;
        }

        public PrestigePreviewData GetPreview()
        {
            var shards = PreviewShardsInternal();
            return new PrestigePreviewData
            {
                CurrentShards = _currencyManager.CreationShards,
                AdditionalShards = shards,
                LifetimeCurrency = _currencyManager.Lifetime
            };
        }

        private BigDouble PreviewShardsInternal()
        {
            double lifetime = _currencyManager.Lifetime.ToDouble();
            if (lifetime <= PrestigeThreshold)
            {
                return BigDouble.Zero;
            }

            double shards = Mathf.Floor(Mathf.Pow((float)(lifetime / PrestigeThreshold), 0.5f));
            return BigDouble.FromDouble(Mathf.Max(1, shards));
        }

        public void ConfirmPrestige()
        {
            var shards = PreviewShardsInternal();
            if (shards.IsZero())
            {
                return;
            }

            _currencyManager.GrantCreationShards(shards);
            _currencyManager.ResetForPrestige();
            _generatorManager.ResetForPrestige();
            _upgradeManager.ResetForPrestige();
            _tierManager.ResetForPrestige();
            _saveManager.ResetForPrestige(shards);
            GameEvents.RaisePrestigeAvailable();
            ServiceLocator.Resolve<AnalyticsRouter>().TrackPrestigeConfirmed(shards.ToDouble());
        }
    }

    public struct PrestigePreviewData
    {
        public BigDouble CurrentShards;
        public BigDouble AdditionalShards;
        public BigDouble LifetimeCurrency;
    }
}
