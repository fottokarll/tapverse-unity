using System.Collections.Generic;
using UnityEngine;

namespace TapVerse.Core
{
    public class ObjectPool : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private int preloadCount = 8;

        private readonly Queue<GameObject> _pool = new Queue<GameObject>();

        private void Awake()
        {
            if (prefab == null)
            {
                return;
            }

            for (int i = 0; i < preloadCount; i++)
            {
                var instance = CreateInstance();
                Return(instance);
            }
        }

        private GameObject CreateInstance()
        {
            var instance = Instantiate(prefab, transform);
            instance.SetActive(false);
            return instance;
        }

        public void SetPrefab(GameObject newPrefab)
        {
            prefab = newPrefab;
        }

        public GameObject Rent()
        {
            if (prefab == null)
            {
                return null;
            }

            if (_pool.Count == 0)
            {
                _pool.Enqueue(CreateInstance());
            }

            var instance = _pool.Dequeue();
            instance.SetActive(true);
            return instance;
        }

        public void Return(GameObject instance)
        {
            instance.SetActive(false);
            instance.transform.SetParent(transform, false);
            _pool.Enqueue(instance);
        }
    }
}
