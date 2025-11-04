using Tapverse.Audio;
using Tapverse.Core;
using Tapverse.Gameplay.Definitions;
using Tapverse.Services;
using UnityEngine;

namespace Tapverse.Gameplay
{
    public class FeedbackManager : GameServiceBehaviour<FeedbackManager>
    {
        [SerializeField] private ObjectPool floatingTextPool;
        [SerializeField] private ParticleSystem tapParticleSystem;
        private IAudioService _audioService;
        private IHapticsService _hapticsService;

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }

        public void Configure(ObjectPool pool, ParticleSystem particleSystem)
        {
            floatingTextPool = pool;
            tapParticleSystem = particleSystem;
        }

        public void Initialize()
        {
            _audioService = ServiceLocator.Resolve<IAudioService>();
            _hapticsService = ServiceLocator.Resolve<IHapticsService>();

            var tapClip = ProceduralAudio.CreateSinePing("TapSfx", 660f, 0.085f, 0.45f, 0.02f, 0.35f);
            var critClip = ProceduralAudio.CreateSinePing("CritSfx", 880f, 0.14f, 0.6f, 0.01f, 0.55f);
            _audioService?.Initialize(tapClip, critClip);
        }

        public void HandleTapFeedback(BigDouble amount)
        {
            if (tapParticleSystem != null)
            {
                tapParticleSystem.Play();
            }

            if (floatingTextPool != null)
            {
                var go = floatingTextPool.Rent();
                var floatingText = go.GetComponent<FloatingText>();
                floatingText?.Show($"+{amount.ToShortString()}");
            }

            _audioService?.PlaySfx(SfxType.Tap);
            _hapticsService?.Pulse();
        }

        public void HandleCrit()
        {
            Time.timeScale = 0.9f;
            _audioService?.PlaySfx(SfxType.Crit);
            _hapticsService?.CriticalPulse();
            Invoke(nameof(ResetTimescale), 0.2f);
        }

        public void HandleTierTransition(TierDefinition tier)
        {
            _audioService?.PlayMusicLayer(tier.MusicLayer?.Clip, tier.MusicLayer?.Volume ?? 1f);
        }

        public void ReturnFloatingText(GameObject instance)
        {
            floatingTextPool?.Return(instance);
        }

        private void ResetTimescale()
        {
            Time.timeScale = 1f;
        }
    }
}
