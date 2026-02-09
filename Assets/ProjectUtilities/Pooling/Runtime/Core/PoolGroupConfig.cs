using System.Collections.Generic;
using UnityEngine;

namespace ProjectUtilities.Pooling.Core
{
    /// <summary>
    /// Groups multiple PoolConfig assets for prewarming and initialization.
    /// </summary>
    [CreateAssetMenu(fileName = "PoolGroupConfig", menuName = "ProjectUtilities/Pooling/PoolGroupConfig")]
    public class PoolGroupConfig : ScriptableObject
    {
        [SerializeField] private List<PoolConfig> _pools = new List<PoolConfig>();

        public IReadOnlyList<PoolConfig> Pools => _pools;
    }
}

