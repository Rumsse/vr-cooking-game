using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Wastepunk.Core
{
    public class PoolManager : MonoBehaviour
    {
        public static PoolManager Instance { get; private set; }

        private readonly Dictionary<string, IObjectPool<GameObject>> pools = new Dictionary<string, IObjectPool<GameObject>>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void CreatePool(string poolId, GameObject prefab, int defaultCapacity = 10, int maxSize = 100)
        {
            if (pools.ContainsKey(poolId))
                return;

            var pool = new ObjectPool<GameObject>(
                createFunc: () => Instantiate(prefab),
                actionOnGet: obj => obj.SetActive(true),
                actionOnRelease: obj => obj.SetActive(false),
                actionOnDestroy: Destroy,
                collectionCheck: true,
                defaultCapacity: defaultCapacity,
                maxSize: maxSize
            );

            pools.Add(poolId, pool);
        }

        public GameObject Get(string poolId)
        {
            if (!pools.ContainsKey(poolId))
            {
                Debug.LogError("Pool not found: " + poolId);
                return null;
            }

            return pools[poolId].Get();
        }

        public void Release(string poolId, GameObject obj)
        {
            if (!pools.ContainsKey(poolId))
            {
                Debug.LogError("Pool not found: " + poolId);
                return;
            }

            pools[poolId].Release(obj);
        }
    }
}
