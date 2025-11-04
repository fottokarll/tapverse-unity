using UnityEngine;

namespace Tapverse.Gameplay.Definitions
{
    [CreateAssetMenu(menuName = "Tapverse/Tier Definition")]
    public class TierDefinition : ScriptableObject
    {
        public int TierIndex;
        public string DisplayName;
        [TextArea]
        public string Description;
        public string BackgroundPrefabAddress;
        public AudioClipLayer MusicLayer;
        public double UnlockThreshold;
    }

    [System.Serializable]
    public class AudioClipLayer
    {
        public AudioClip Clip;
        [Range(0f, 1f)] public float Volume = 1f;
    }
}
