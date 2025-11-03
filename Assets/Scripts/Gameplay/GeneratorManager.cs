using System.Collections.Generic;
using TapVerse.Core;
using TapVerse.Gameplay.Definitions;
using UnityEngine;

namespace TapVerse.Gameplay
{
    public class GeneratorManager : GameServiceBehaviour<GeneratorManager>
    {
        private readonly Dictionary<string, int> _counts = new Dictionary<string, int>();
        private readonly Dictionary<string, BigDouble> _productionPerGenerator = new Dictionary<string, BigDouble>();
        private GeneratorCatalog _catalog;
        private SaveManager _saveManager;
        private CurrencyManager _currencyManager;
        private double _efficiencyMultiplier = 1d;

        public void Initialize(GeneratorCatalog catalog)
        {
            _catalog = catalog;
            _saveManager = ServiceLocator.Resolve<SaveManager>();
            _currencyManager = ServiceLocator.Resolve<CurrencyManager>();

            _counts.Clear();
            _productionPerGenerator.Clear();
            foreach (var generator in _catalog.Generators)
            {
                _counts[generator.Id] = 0;
                _productionPerGenerator[generator.Id] = generator.BaseProduction;
            }

            var data = _saveManager.Data;
            if (data.Generators != null)
            {
                foreach (var saved in data.Generators)
                {
                    if (_counts.ContainsKey(saved.Id))
                    {
                        _counts[saved.Id] = saved.Count;
                    }
                }
            }

            GameEvents.RaiseGeneratorsChanged();
        }

        public int GetCount(string generatorId)
        {
            return _counts.TryGetValue(generatorId, out var value) ? value : 0;
        }

        public BigDouble GetCost(GeneratorDefinition definition)
        {
            int count = GetCount(definition.Id);
            var growth = Mathf.Pow(definition.CostGrowth, count);
            return definition.BaseCost * growth;
        }

        public bool TryPurchase(GeneratorDefinition definition)
        {
            var cost = GetCost(definition);
            if (!_currencyManager.SpendCurrency(cost))
            {
                return false;
            }

            _counts[definition.Id] = GetCount(definition.Id) + 1;
            SaveState();
            GameEvents.RaiseGeneratorsChanged();
            ServiceLocator.Resolve<AnalyticsRouter>().TrackGenerator(definition.Id);
            return true;
        }

        public BigDouble EvaluateProductionPerSecond()
        {
            BigDouble total = BigDouble.Zero;
            foreach (var generator in _catalog.Generators)
            {
                int count = GetCount(generator.Id);
                if (count <= 0) continue;
                var perGen = _productionPerGenerator[generator.Id];
                total += perGen * count * _efficiencyMultiplier;
            }

            return total;
        }

        public void ApplyEfficiencyMultiplier(double multiplier)
        {
            _efficiencyMultiplier = multiplier;
        }

        public void ResetForPrestige()
        {
            var keys = new List<string>(_counts.Keys);
            foreach (var key in keys)
            {
                _counts[key] = 0;
            }

            SaveState();
            GameEvents.RaiseGeneratorsChanged();
        }

        public IEnumerable<GeneratorStateData> SerializeState()
        {
            foreach (var kvp in _counts)
            {
                yield return new GeneratorStateData
                {
                    Id = kvp.Key,
                    Count = kvp.Value
                };
            }
        }

        private void SaveState()
        {
            var data = _saveManager.Data;
            data.Generators.Clear();
            foreach (var kvp in _counts)
            {
                data.Generators.Add(new GeneratorStateData
                {
                    Id = kvp.Key,
                    Count = kvp.Value
                });
            }
        }
    }
}
