using UnityEngine;

namespace TapVerse.Gameplay
{
    public interface IAnalytics
    {
        void Track(string eventName, object payload = null);
    }

    public class AnalyticsRouter : GameServiceBehaviour<AnalyticsRouter>
    {
        private IAnalytics _analytics;

        public void Initialize()
        {
            ServiceLocator.TryResolve(out _analytics);
            TrackSessionStart();
        }

        private void OnApplicationQuit()
        {
            Track("session_end");
        }

        public void TrackTap() => Track("tap");
        public void TrackUpgrade(string id) => Track("purchase_upgrade", new { id });
        public void TrackGenerator(string id) => Track("purchase_generator", new { id });
        public void TrackTierUnlock(int tier) => Track("tier_unlock", new { tier });
        public void TrackPrestigeOpened() => Track("prestige_opened");
        public void TrackPrestigeConfirmed(double shards) => Track("prestige_confirmed", new { shards });

        private void TrackSessionStart() => Track("session_start");

        private void Track(string eventName, object payload = null)
        {
            if (_analytics != null)
            {
                _analytics.Track(eventName, payload);
                return;
            }

            if (payload != null)
            {
                Debug.Log($"[Analytics] {eventName}: {JsonUtility.ToJson(payload)}");
            }
            else
            {
                Debug.Log($"[Analytics] {eventName}");
            }
        }
    }
}
