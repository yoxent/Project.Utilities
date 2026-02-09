using UnityEngine;
using ProjectUtilities.Pooling.Core;

namespace ProjectUtilities.Pooling.Demo
{
    /// <summary>
    /// Simple pooled object used in the Pooling_Demo scene.
    /// </summary>
    public class PoolingDemoPooledObject : MonoBehaviour, IPoolable
    {
        public void OnSpawned()
        {
            gameObject.SetActive(true);
        }

        public void OnDespawned()
        {
            // Optional: reset state when returned to pool.
        }
    }
}
