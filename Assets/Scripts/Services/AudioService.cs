using TapVerse.Core;
using UnityEngine;

namespace TapVerse.Services
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioService : GameServiceBehaviour<IAudioService>, IAudioService
    {
        private AudioSource _sfxSource;
        private AudioSource _musicSource;
        private AudioClip _tapClip;
        private AudioClip _critClip;

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
            _sfxSource = GetComponent<AudioSource>();
            _sfxSource.playOnAwake = false;
            _musicSource = gameObject.AddComponent<AudioSource>();
            _musicSource.loop = true;
        }

        public void Initialize(AudioClip tapClip, AudioClip critClip)
        {
            _tapClip = tapClip;
            _critClip = critClip;
        }

        public void PlaySfx(SfxType type)
        {
            switch (type)
            {
                case SfxType.Tap:
                    if (_tapClip != null)
                    {
                        _sfxSource.PlayOneShot(_tapClip);
                    }
                    break;
                case SfxType.Crit:
                    if (_critClip != null)
                    {
                        _sfxSource.PlayOneShot(_critClip);
                    }
                    break;
            }
        }

        public void PlayMusicLayer(AudioClip clip, float volume)
        {
            if (clip == null)
            {
                return;
            }

            if (_musicSource.clip == clip)
            {
                return;
            }

            _musicSource.clip = clip;
            _musicSource.volume = volume;
            _musicSource.Play();
        }
    }
}
