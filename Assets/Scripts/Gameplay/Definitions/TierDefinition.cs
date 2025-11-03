using UnityEngine;

namespace TapVerse.Gameplay.Definitions
{
    [CreateAssetMenu(menuName = "TapVerse/Tier Definition")]
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
