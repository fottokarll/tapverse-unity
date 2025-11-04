using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Tapverse.Core;
using Tapverse.Gameplay.Definitions;

namespace Tapverse.Gameplay
{
    public enum GameState
    {
        Boot,
        Loading,
        Gameplay
    }

    public class GameManager : GameServiceBehaviour<GameManager>
    {
        [SerializeField] private UpgradeCatalog upgradeCatalog;
        [SerializeField] private GeneratorCatalog generatorCatalog;
        [SerializeField] private TierCatalog tierCatalog;

        public GameState State { get; private set; } = GameState.Boot;

        public UpgradeCatalog UpgradeCatalog => upgradeCatalog;
        public GeneratorCatalog GeneratorCatalog => generatorCatalog;
        public TierCatalog TierCatalog => tierCatalog;

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }

        public void Configure(UpgradeCatalog upgrades, GeneratorCatalog generators, TierCatalog tiers)
        {
            upgradeCatalog = upgrades;
            generatorCatalog = generators;
            tierCatalog = tiers;
        }

        private void Start()
        {
            StartCoroutine(LoadGameRoutine());
        }

        private IEnumerator LoadGameRoutine()
        {
            State = GameState.Loading;

            var saveManager = ServiceLocator.Resolve<SaveManager>();
            yield return saveManager.LoadAsync();

            UpgradeManager upgradeManager = ServiceLocator.Resolve<UpgradeManager>();
            GeneratorManager generatorManager = ServiceLocator.Resolve<GeneratorManager>();
            generatorManager.Initialize(generatorCatalog);

            upgradeManager.Initialize(upgradeCatalog);

            CurrencyManager currencyManager = ServiceLocator.Resolve<CurrencyManager>();
            currencyManager.Initialize();

            TierManager tierManager = ServiceLocator.Resolve<TierManager>();
            tierManager.Initialize(tierCatalog);

            PrestigeManager prestigeManager = ServiceLocator.Resolve<PrestigeManager>();
            prestigeManager.Initialize();

            ServiceLocator.Resolve<FeedbackManager>().Initialize();
            ServiceLocator.Resolve<AnalyticsRouter>().Initialize();

            State = GameState.Gameplay;
            yield return SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);
        }

        public void RestartRun()
        {
            StartCoroutine(RestartRoutine());
        }

        private IEnumerator RestartRoutine()
        {
            State = GameState.Loading;
            ServiceLocator.Resolve<SaveManager>().ResetRun();
            yield return SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);
            State = GameState.Gameplay;
        }
    }
}
