using System.Collections;
using Tapverse.Core;
using Tapverse.Gameplay;
using Tapverse.Services;
using Tapverse.Visuals;
using UnityEngine;

namespace Tapverse
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
            ConfigureTapParticles(particle);

            feedback.Configure(pool, particle);

            var upgradeCatalog = Resources.Load<Tapverse.Gameplay.Definitions.UpgradeCatalog>("Catalogs/UpgradeCatalog");
            var generatorCatalog = Resources.Load<Tapverse.Gameplay.Definitions.GeneratorCatalog>("Catalogs/GeneratorCatalog");
            var tierCatalog = Resources.Load<Tapverse.Gameplay.Definitions.TierCatalog>("Catalogs/TierCatalog");
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
            go.AddComponent<Tapverse.UI.FloatingText>();
            go.SetActive(false);
            return go;
        }

        private void ConfigureTapParticles(ParticleSystem particle)
        {
            var main = particle.main;
            main.loop = false;
            main.startLifetime = 0.35f;
            main.startSpeed = new ParticleSystem.MinMaxCurve(1.5f, 3f);
            main.startSize = new ParticleSystem.MinMaxCurve(0.05f, 0.2f);
            main.startColor = new ParticleSystem.MinMaxGradient(new Color(0.9f, 0.9f, 1f, 0.9f), new Color(0.4f, 0.6f, 1f, 0.4f));

            var emission = particle.emission;
            emission.rateOverTime = 0f;
            emission.burstCount = 1;
            emission.SetBurst(0, new ParticleSystem.Burst(0f, (short)8, (short)12));

            var shape = particle.shape;
            shape.shapeType = ParticleSystemShapeType.Circle;
            shape.radius = 0.1f;

            var texture = ProceduralSprites.GetSparkTexture();
            var renderer = particle.GetComponent<ParticleSystemRenderer>();
            renderer.material = new Material(Shader.Find("Sprites/Default"));
            renderer.material.mainTexture = texture;

            var animation = particle.textureSheetAnimation;
            animation.enabled = true;
            animation.mode = ParticleSystemAnimationMode.Sprites;
            for (var i = animation.spriteCount - 1; i >= 0; i--)
            {
                animation.RemoveSprite(i);
            }
            animation.AddSprite(ProceduralSprites.GetSparkSprite());

            particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
}
