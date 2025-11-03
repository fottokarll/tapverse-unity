using System;
using TapVerse.Core;

namespace TapVerse
{
    public static class GameEvents
    {
        public static event Action<BigDouble> CurrencyChanged;
        public static event Action<BigDouble> LifetimeCurrencyChanged;
        public static event Action<BigDouble> CreationShardsChanged;
        public static event Action<int> TierChanged;
        public static event Action TapRegistered;
        public static event Action UpgradesChanged;
        public static event Action GeneratorsChanged;
        public static event Action PrestigeAvailable;

        public static void RaiseCurrencyChanged(BigDouble value) => CurrencyChanged?.Invoke(value);
        public static void RaiseLifetimeCurrencyChanged(BigDouble value) => LifetimeCurrencyChanged?.Invoke(value);
        public static void RaiseCreationShardsChanged(BigDouble value) => CreationShardsChanged?.Invoke(value);
        public static void RaiseTierChanged(int tier) => TierChanged?.Invoke(tier);
        public static void RaiseTapRegistered() => TapRegistered?.Invoke();
        public static void RaiseUpgradesChanged() => UpgradesChanged?.Invoke();
        public static void RaiseGeneratorsChanged() => GeneratorsChanged?.Invoke();
        public static void RaisePrestigeAvailable() => PrestigeAvailable?.Invoke();
    }
}
