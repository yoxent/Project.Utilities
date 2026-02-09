using UnityEngine;
using UnityEngine.InputSystem;
using ProjectUtilities.Pooling.Core;

namespace ProjectUtilities.Pooling.Demo
{
    /// <summary>
    /// Simple controller for demonstrating the pooling system.
    /// Press Space to spawn pooled objects and Backspace to despawn all.
    /// </summary>
    public class PoolingDemoController : MonoBehaviour
    {
        [SerializeField] private PoolGroupConfig _poolGroup;
        [SerializeField] private string _poolId = "DemoCube";
        [SerializeField] private float _spawnRadius = 3f;

        private PoolManager _poolManager;

        private void Awake()
        {
            var root = new GameObject("PoolRoot").transform;
            _poolManager = new PoolManager(root);
            _poolManager.Initialize(new[] { _poolGroup });
        }

        private void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return;
            }

            if (keyboard.spaceKey.wasPressedThisFrame)
            {
                var instance = _poolManager.Rent<PoolingDemoPooledObject>(_poolId);
                if (instance != null)
                {
                    instance.transform.position = Random.insideUnitSphere * _spawnRadius;
                }
            }

            if (keyboard.backspaceKey.wasPressedThisFrame)
            {
                var instances = FindObjectsByType<PoolingDemoPooledObject>(FindObjectsSortMode.None);
                foreach (var inst in instances)
                {
                    _poolManager.Return(_poolId, inst);
                }
            }
        }
    }
}
