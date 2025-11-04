using UnityEngine;
using Tapverse.Core;

namespace Tapverse.Gameplay
{
    public class CurrencyManager : GameServiceBehaviour<CurrencyManager>
    {
        [SerializeField] private double baseTapValue = 1d;
        [SerializeField] private double idleCapHours = 8d;

        public BigDouble Current => _current;
        public BigDouble Lifetime => _lifetime;
        public BigDouble CreationShards => _creationShards;
        public double TapMultiplier { get; private set; } = 1d;
        public double CritChance { get; private set; } = 0.03d;
        public double CritPower { get; private set; } = 1.5d;

        private BigDouble _current = BigDouble.Zero;
        private BigDouble _lifetime = BigDouble.Zero;
        private BigDouble _creationShards = BigDouble.Zero;

        private GeneratorManager _generatorManager;
        private SaveManager _saveManager;

        protected override void Awake()
        {
            base.Awake();
        }

        public void Initialize()
        {
            _generatorManager = ServiceLocator.Resolve<GeneratorManager>();
            _saveManager = ServiceLocator.Resolve<SaveManager>();

            var data = _saveManager.Data;
            _current = data.CurrentCurrency;
            _lifetime = data.LifetimeCurrency;
            _creationShards = data.CreationShards;
            TapMultiplier = data.TapMultiplier <= 0 ? 1d : data.TapMultiplier;
            CritChance = Mathf.Max(0.0f, data.CritChance);
            CritPower = Mathf.Max(1.0f, data.CritPower);

            ApplyOfflineProgress(data);

            GameEvents.RaiseCurrencyChanged(_current);
            GameEvents.RaiseLifetimeCurrencyChanged(_lifetime);
            GameEvents.RaiseCreationShardsChanged(_creationShards);
        }

        private void Update()
        {
            if (_generatorManager == null)
            {
                return;
            }

            var idleProduction = _generatorManager.EvaluateProductionPerSecond();
            if (!idleProduction.IsZero())
            {
                var income = idleProduction * Time.deltaTime;
                AddCurrency(income, false);
            }
        }

        private void ApplyOfflineProgress(GameSaveData data)
        {
            var elapsedSeconds = Mathf.Clamp((float)data.SecondsSinceLastSession, 0f, (float)(idleCapHours * 3600f));
            if (elapsedSeconds <= 0f)
            {
                return;
            }

            var idleProduction = _generatorManager != null ? _generatorManager.EvaluateProductionPerSecond() : BigDouble.Zero;
            if (!idleProduction.IsZero())
            {
                var offlineIncome = idleProduction * elapsedSeconds;
                AddCurrency(offlineIncome, false);
            }
        }

        public BigDouble EvaluateTapValue(bool isCritical)
        {
            var tapBase = BigDouble.FromDouble(baseTapValue * TapMultiplier);
            if (isCritical)
            {
                tapBase = tapBase * CritPower;
            }

            return tapBase;
        }

        public void RegisterTap()
        {
            bool crit = Random.value < CritChance;
            var gain = EvaluateTapValue(crit);
            AddCurrency(gain, true);
            GameEvents.RaiseTapRegistered();
            if (crit)
            {
                ServiceLocator.Resolve<FeedbackManager>().HandleCrit();
            }
            ServiceLocator.Resolve<AnalyticsRouter>().TrackTap();
        }

        public void AddCurrency(BigDouble amount, bool fromTap)
        {
            if (amount.IsZero())
            {
                return;
            }

            _current += amount;
            _lifetime += amount;

            GameEvents.RaiseCurrencyChanged(_current);
            GameEvents.RaiseLifetimeCurrencyChanged(_lifetime);

            if (fromTap)
            {
                ServiceLocator.Resolve<FeedbackManager>().HandleTapFeedback(amount);
            }
        }

        public bool SpendCurrency(BigDouble cost)
        {
            if (_current < cost)
            {
                return false;
            }

            _current -= cost;
            GameEvents.RaiseCurrencyChanged(_current);
            return true;
        }

        public void GrantCreationShards(BigDouble amount)
        {
            _creationShards += amount;
            GameEvents.RaiseCreationShardsChanged(_creationShards);
        }

        public void ApplyMultipliers(double tapMultiplier, double critChance, double critPower)
        {
            TapMultiplier = tapMultiplier;
            CritChance = critChance;
            CritPower = critPower;
        }

        public void ResetForPrestige()
        {
            _current = BigDouble.Zero;
            _lifetime = BigDouble.Zero;
            TapMultiplier = 1d;
            CritChance = 0.03d;
            CritPower = 1.5d;
            GameEvents.RaiseCurrencyChanged(_current);
            GameEvents.RaiseLifetimeCurrencyChanged(_lifetime);
        }

        public GameSaveData CaptureSave()
        {
            var data = _saveManager.Data;
            data.CurrentCurrency = _current;
            data.LifetimeCurrency = _lifetime;
            data.CreationShards = _creationShards;
            data.TapMultiplier = TapMultiplier;
            data.CritChance = CritChance;
            data.CritPower = CritPower;
            return data;
        }
    }
}
