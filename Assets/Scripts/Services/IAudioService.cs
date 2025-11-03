using UnityEngine;

namespace TapVerse.Services
{
    public enum SfxType
    {
        Tap,
        Crit
    }

    public interface IAudioService
    {
        void Initialize(AudioClip tapClip, AudioClip critClip);
        void PlaySfx(SfxType type);
        void PlayMusicLayer(AudioClip clip, float volume);
    }
}
