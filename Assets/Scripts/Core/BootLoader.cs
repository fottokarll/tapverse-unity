using System.Collections;
using TapVerse.Core;
using TapVerse.Gameplay;
using TapVerse.Services;
using UnityEngine;

namespace TapVerse
{
    public class BootLoader : MonoBehaviour
    {
        private IEnumerator Start()
        {
            DontDestroyOnLoad(gameObject);
            BuildSystems();
            yield return null;
        }

        private void BuildSystems()
        {
            if (ServiceLocator.TryResolve(out GameManager _))
            {
                return;
            }

            var systems = new GameObject("SystemsRoot");
            DontDestroyOnLoad(systems);
            systems.AddComponent<SaveManager>();
            systems.AddComponent<CurrencyManager>();
            systems.AddComponent<GeneratorManager>();
            systems.AddComponent<UpgradeManager>();
            systems.AddComponent<TierManager>();
            systems.AddComponent<PrestigeManager>();
            systems.AddComponent<AnalyticsRouter>();
            systems.AddComponent<AudioService>();
            systems.AddComponent<HapticsService>();
            var feedback = systems.AddComponent<FeedbackManager>();
            var gameManager = systems.AddComponent<GameManager>();

            var poolObj = new GameObject("FloatingTextPool");
            poolObj.transform.SetParent(systems.transform);
            var pool = poolObj.AddComponent<ObjectPool>();
            pool.SetPrefab(CreateFloatingTextPrefab(poolObj.transform));

            var particleObj = new GameObject("TapParticles");
            particleObj.transform.SetParent(systems.transform);
            var particle = particleObj.AddComponent<ParticleSystem>();


            feedback.Configure(pool, particle);

            var upgradeCatalog = Resources.Load<TapVerse.Gameplay.Definitions.UpgradeCatalog>("Catalogs/UpgradeCatalog");
            var generatorCatalog = Resources.Load<TapVerse.Gameplay.Definitions.GeneratorCatalog>("Catalogs/GeneratorCatalog");
            var tierCatalog = Resources.Load<TapVerse.Gameplay.Definitions.TierCatalog>("Catalogs/TierCatalog");
            gameManager.Configure(upgradeCatalog, generatorCatalog, tierCatalog);
        }

        private GameObject CreateFloatingTextPrefab(Transform parent)
        {
            var go = new GameObject("FloatingTextPrefab");
            go.transform.SetParent(parent);
            var rect = go.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(100, 30);
            var text = go.AddComponent<UnityEngine.UI.Text>();
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.color = Color.white;
            go.AddComponent<TapVerse.UI.FloatingText>();
            go.SetActive(false);
            return go;
        }

    }
}
