using UnityEngine;

namespace Tapverse.Audio
{
    public static class ProceduralAudio
    {
        public static AudioClip CreateSinePing(string name, float frequencyHz, float durationSeconds, float amplitude = 0.5f, float attackFraction = 0.05f, float releaseFraction = 0.35f, int sampleRate = 44100)
        {
            if (durationSeconds <= 0f)
            {
                durationSeconds = 0.01f;
            }

            var totalSamples = Mathf.Max(1, Mathf.CeilToInt(durationSeconds * sampleRate));
            var clip = AudioClip.Create(name, totalSamples, 1, sampleRate, false);
            var samples = new float[totalSamples];

            var attackSamples = Mathf.Clamp(Mathf.FloorToInt(totalSamples * attackFraction), 1, totalSamples);
            var releaseSamples = Mathf.Clamp(Mathf.FloorToInt(totalSamples * releaseFraction), 1, totalSamples);

            for (var i = 0; i < totalSamples; i++)
            {
                var t = i / (float)sampleRate;
                var envelope = 1f;

                if (i < attackSamples)
                {
                    envelope = i / (float)attackSamples;
                }
                else if (i > totalSamples - releaseSamples)
                {
                    var releaseIndex = i - (totalSamples - releaseSamples);
                    envelope = 1f - releaseIndex / (float)releaseSamples;
                }

                var value = Mathf.Sin(2f * Mathf.PI * frequencyHz * t);
                samples[i] = value * amplitude * envelope;
            }

            clip.SetData(samples, 0);
            return clip;
        }
    }
}
