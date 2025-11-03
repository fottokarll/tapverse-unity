using UnityEngine;

namespace TapVerse.Core
{
    /// <summary>
    /// Base MonoBehaviour for services that self-register into the ServiceLocator.
    /// </summary>
    /// <typeparam name="T">Type interface or concrete to register.</typeparam>
    public abstract class GameServiceBehaviour<T> : MonoBehaviour
    {
        protected virtual void Awake()
        {
            ServiceLocator.Register((T)(object)this);
        }

        protected virtual void OnDestroy()
        {
            if (ReferenceEquals(ServiceLocator.Resolve<T>(), this))
            {
                ServiceLocator.Register<T>(default);
            }
        }
    }
}
