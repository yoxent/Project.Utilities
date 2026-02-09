using System.Collections.Generic;
using ProjectUtilities.Core;
using UnityEngine;

namespace ProjectUtilities.Pooling.Core
{
    /// <summary>
    /// Central registry for object pools, keyed by string poolId.
    /// Initialize with one or more PoolGroupConfig assets.
    /// </summary>
    public class PoolManager
    {
        private readonly Dictionary<string, object> _pools = new Dictionary<string, object>();
        private readonly Transform _root;

        public PoolManager(Transform root)
        {
            _root = root;
        }

        public void Initialize(IEnumerable<PoolGroupConfig> groups)
        {
            foreach (var group in groups)
            {
                if (group == null)
                {
                    continue;
                }

                foreach (var config in group.Pools)
                {
                    RegisterPool(config);
                }
            }

            ServiceLocator.Instance.Register(this);
        }

        private void RegisterPool(PoolConfig config)
        {
            if (config == null || config.Prefab == null || string.IsNullOrEmpty(config.PoolId))
            {
                return;
            }

            if (_pools.ContainsKey(config.PoolId))
            {
                Debug.LogWarning($"Pool with id '{config.PoolId}' is already registered.");
                return;
            }

            var parent = new GameObject($"Pool_{config.PoolId}").transform;
            parent.SetParent(_root, false);

            var prefabComponent = config.Prefab.GetComponent<IPoolable>();
            if (prefabComponent == null)
            {
                Debug.LogError($"Prefab for pool '{config.PoolId}' does not implement IPoolable.");
                Object.Destroy(parent.gameObject);
                return;
            }

            var component = prefabComponent as Component;
            var genericType = typeof(ObjectPool<>).MakeGenericType(component.GetType());
            var pool = System.Activator.CreateInstance(
                genericType,
                component,
                config.InitialSize,
                config.MaxSize,
                config.AutoExpand,
                parent);

            _pools[config.PoolId] = pool;
        }

        public T Rent<T>(string poolId) where T : Component, IPoolable
        {
            if (!_pools.TryGetValue(poolId, out var poolObj))
            {
                Debug.LogError($"No pool registered with id '{poolId}'.");
                return null;
            }

            if (poolObj is ObjectPool<T> typedPool)
            {
                return typedPool.Rent();
            }

            Debug.LogError($"Pool '{poolId}' is not of expected type {typeof(T).Name}.");
            return null;
        }

        public void Return<T>(string poolId, T instance) where T : Component, IPoolable
        {
            if (instance == null)
            {
                return;
            }

            if (!_pools.TryGetValue(poolId, out var poolObj))
            {
                Debug.LogError($"No pool registered with id '{poolId}'.");
                return;
            }

            if (poolObj is ObjectPool<T> typedPool)
            {
                typedPool.Return(instance);
                return;
            }

            Debug.LogError($"Pool '{poolId}' is not of expected type {typeof(T).Name}.");
        }
    }
}

