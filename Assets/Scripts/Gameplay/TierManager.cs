using System.Collections.Generic;
using Tapverse.Core;
using Tapverse.Gameplay.Definitions;
using UnityEngine;

namespace Tapverse.Gameplay
{
    public class TierManager : GameServiceBehaviour<TierManager>
    {
        private TierCatalog _catalog;
        private SaveManager _saveManager;
        private int _currentTierIndex;
        private readonly List<TierDefinition> _tiers = new List<TierDefinition>();

        public TierDefinition CurrentTier => _currentTierIndex < _tiers.Count ? _tiers[_currentTierIndex] : null;

        public void Initialize(TierCatalog catalog)
        {
            _catalog = catalog;
            _saveManager = ServiceLocator.Resolve<SaveManager>();

            _tiers.Clear();
            _tiers.AddRange(_catalog.Tiers);
            _tiers.Sort((a, b) => a.UnlockThreshold.CompareTo(b.UnlockThreshold));

            _currentTierIndex = Mathf.Clamp(_saveManager.Data.CurrentTierIndex, 0, _tiers.Count - 1);

            GameEvents.LifetimeCurrencyChanged += HandleLifetimeCurrencyChanged;
            NotifyTierChanged();
        }

        private void HandleLifetimeCurrencyChanged(BigDouble lifetime)
        {
            for (int i = _currentTierIndex + 1; i < _tiers.Count; i++)
            {
                if (lifetime.ToDouble() >= _tiers[i].UnlockThreshold)
                {
                    _currentTierIndex = i;
                    _saveManager.Data.CurrentTierIndex = _currentTierIndex;
                    NotifyTierChanged();
                    ServiceLocator.Resolve<FeedbackManager>().HandleTierTransition(_tiers[i]);
                    ServiceLocator.Resolve<AnalyticsRouter>().TrackTierUnlock(_tiers[i].TierIndex);
                }
            }
        }

        private void NotifyTierChanged()
        {
            GameEvents.RaiseTierChanged(_currentTierIndex);
        }

        public void ResetForPrestige()
        {
            _currentTierIndex = 0;
            _saveManager.Data.CurrentTierIndex = 0;
            NotifyTierChanged();
        }

        private void OnDestroy()
        {
            GameEvents.LifetimeCurrencyChanged -= HandleLifetimeCurrencyChanged;
        }
    }
}
