using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Tapverse.Core;
using UnityEngine;

namespace Tapverse.Gameplay
{
    public class SaveManager : GameServiceBehaviour<SaveManager>
    {
        private const string SaveFileName = "tapverse_save.json";
        private const string SaveVersion = "0.1.0";

        public GameSaveData Data { get; private set; } = new GameSaveData();

        private string SavePath => Path.Combine(Application.persistentDataPath, SaveFileName);

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            InvokeRepeating(nameof(AutoSave), 10f, 10f);
            GameEvents.UpgradesChanged += AutoSave;
            GameEvents.GeneratorsChanged += AutoSave;
            GameEvents.PrestigeAvailable += AutoSave;
        }

        private void OnDestroy()
        {
            GameEvents.UpgradesChanged -= AutoSave;
            GameEvents.GeneratorsChanged -= AutoSave;
            GameEvents.PrestigeAvailable -= AutoSave;
        }

        public IEnumerator LoadAsync()
        {
            if (File.Exists(SavePath))
            {
                try
                {
                    var json = File.ReadAllText(SavePath, Encoding.UTF8);
                    Data = JsonUtility.FromJson<GameSaveData>(json) ?? new GameSaveData();
                }
                catch
                {
                    Data = new GameSaveData();
                }
            }
            else
            {
                Data = new GameSaveData();
            }

            Data.Version = SaveVersion;
            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            Data.SecondsSinceLastSession = Mathf.Max(0, now - Data.LastSaveTimestamp);
            yield return null;
        }

        public void AutoSave()
        {
            CaptureDataFromManagers();
            WriteToDisk();
        }

        private void CaptureDataFromManagers()
        {
            if (ServiceLocator.TryResolve(out CurrencyManager currency))
            {
                Data = currency.CaptureSave();
            }

            if (ServiceLocator.TryResolve(out GeneratorManager generators))
            {
                Data.Generators.Clear();
                foreach (var entry in generators.SerializeState())
                {
                    Data.Generators.Add(entry);
                }
            }

            if (ServiceLocator.TryResolve(out UpgradeManager upgrades))
            {
                Data.Upgrades.Clear();
                foreach (var entry in upgrades.SerializeState())
                {
                    Data.Upgrades.Add(entry);
                }
            }

            if (ServiceLocator.TryResolve(out TierManager tier))
            {
                Data.CurrentTierIndex = tier.CurrentTier != null ? tier.CurrentTier.TierIndex : 0;
            }

            Data.LastSaveTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        private void WriteToDisk()
        {
            try
            {
                var directory = Path.GetDirectoryName(SavePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var json = JsonUtility.ToJson(Data, true);
                File.WriteAllText(SavePath, json, Encoding.UTF8);
            }
            catch (IOException ex)
            {
                Debug.LogError($"Failed to save Tapverse data: {ex}");
            }
        }

        public void ResetRun()
        {
            Data = new GameSaveData
            {
                CreationShards = this.Data.CreationShards
            };
            AutoSave();
        }

        public void ResetForPrestige(BigDouble shards)
        {
            Data.CurrentCurrency = BigDouble.Zero;
            Data.LifetimeCurrency = BigDouble.Zero;
            Data.CreationShards += shards;
            Data.Upgrades.Clear();
            Data.Generators.Clear();
            Data.CurrentTierIndex = 0;
            AutoSave();
        }
    }

    [System.Serializable]
    public class GameSaveData
    {
        public string Version = "0.1.0";
        public BigDouble CurrentCurrency = BigDouble.Zero;
        public BigDouble LifetimeCurrency = BigDouble.Zero;
        public BigDouble CreationShards = BigDouble.Zero;
        public double TapMultiplier = 1d;
        public double CritChance = 0.03d;
        public double CritPower = 1.5d;
        public List<UpgradeStateData> Upgrades = new List<UpgradeStateData>();
        public List<GeneratorStateData> Generators = new List<GeneratorStateData>();
        public int CurrentTierIndex;
        public long LastSaveTimestamp;
        public double SecondsSinceLastSession;
        public PlayerOptions Options = new PlayerOptions();
        public bool TutorialCompleted;
    }

    [System.Serializable]
    public class UpgradeStateData
    {
        public string Id;
        public int Level;
    }

    [System.Serializable]
    public class GeneratorStateData
    {
        public string Id;
        public int Count;
    }

    [System.Serializable]
    public class PlayerOptions
    {
        public bool SfxEnabled = true;
        public bool HapticsEnabled = true;
        public bool ReducedMotion = false;
    }
}
