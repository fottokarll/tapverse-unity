using Tapverse.Core;
using UnityEngine;

namespace Tapverse.Services
{
    public class HapticsService : GameServiceBehaviour<IHapticsService>, IHapticsService
    {
        [SerializeField] private bool enableEditorFeedback = false;

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }

        public void Pulse()
        {
            if (enableEditorFeedback)
            {
                Handheld.Vibrate();
            }
        }

        public void CriticalPulse()
        {
            if (enableEditorFeedback)
            {
                Handheld.Vibrate();
            }
        }
    }
}
