using UnityEngine;
using UnityEngine.Pool;

namespace ProjectUtilities.Pooling.Core
{
    /// <summary>
    /// Generic object pool for Components implementing IPoolable.
    /// Wraps Unity's <see cref="UnityEngine.Pool.ObjectPool{T}"/> and preserves
    /// the existing API/semantics (autoExpand + maxSize + Active/Inactive counts).
    /// </summary>
    public class ObjectPool<T> where T : Component, IPoolable
    {
        private readonly T _prefab;
        private readonly Transform _parent;
        private readonly bool _autoExpand;
        private readonly int _maxSize;

        // Underlying Unity pool
        private readonly UnityEngine.Pool.ObjectPool<T> _pool;

        // Track active instances for compatibility with previous API.
        private int _activeCount;

        public ObjectPool(T prefab, int initialSize, int maxSize, bool autoExpand, Transform parent)
        {
            _prefab = prefab;
            _parent = parent;
            _autoExpand = autoExpand;
            _maxSize = maxSize;

            // Map "no max" (0) to a very large maxSize for Unity's pool.
            var unityMaxSize = maxSize > 0 ? maxSize : int.MaxValue;

            _pool = new UnityEngine.Pool.ObjectPool<T>(
                createFunc: CreateInstance,
                actionOnGet: OnGet,
                actionOnRelease: OnRelease,
                actionOnDestroy: OnDestroy,
                collectionCheck: false,
                defaultCapacity: initialSize > 0 ? initialSize : 0,
                maxSize: unityMaxSize);

            // Prewarm to match previous behaviour (optional initial instances).
            Prewarm(initialSize);
        }

        /// <summary>
        /// Pre-create and store inactive instances in the pool.
        /// </summary>
        public void Prewarm(int count)
        {
            if (count <= 0)
            {
                return;
            }

            for (int i = 0; i < count; i++)
            {
                var instance = CreateInstance();
                // Release directly into the pool; OnRelease will ensure correct inactive state.
                _pool.Release(instance);
            }
        }

        /// <summary>
        /// Rent an instance from the pool.
        /// Respects autoExpand/maxSize semantics from the previous implementation.
        /// </summary>
        public T Rent()
        {
            if (!_autoExpand && _maxSize > 0)
            {
                // If nothing inactive and we've hit max active, fail instead of expanding.
                if (_pool.CountInactive == 0 && _activeCount >= _maxSize)
                {
                    return null;
                }
            }

            return _pool.Get();
        }

        /// <summary>
        /// Return an instance to the pool.
        /// </summary>
        public void Return(T instance)
        {
            if (instance == null)
            {
                return;
            }

            // collectionCheck is disabled on the underlying pool, so this will not throw
            // if the instance did not originate from this pool, but behaviour is undefined.
            _pool.Release(instance);
        }

        public int ActiveCount => _activeCount;
        public int InactiveCount => _pool.CountInactive;

        private T CreateInstance()
        {
            var instance = Object.Instantiate(_prefab, _parent);
            instance.gameObject.SetActive(false);
            return instance;
        }

        private void OnGet(T instance)
        {
            _activeCount++;
            instance.gameObject.SetActive(true);
            instance.OnSpawned();
        }

        private void OnRelease(T instance)
        {
            if (_activeCount > 0)
            {
                _activeCount--;
            }

            instance.OnDespawned();
            instance.gameObject.SetActive(false);
        }

        private static void OnDestroy(T instance)
        {
            if (instance != null)
            {
                Object.Destroy(instance.gameObject);
            }
        }
    }
}

