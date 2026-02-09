using UnityEngine;

namespace ProjectUtilities.Pooling.Core
{
    /// <summary>
    /// Configuration for a single pooled prefab.
    /// </summary>
    [CreateAssetMenu(fileName = "PoolConfig", menuName = "ProjectUtilities/Pooling/PoolConfig")]
    public class PoolConfig : ScriptableObject
    {
        [SerializeField] private string _poolId;
        [SerializeField] private GameObject _prefab;
        [SerializeField] private int _initialSize = 8;
        [SerializeField] private int _maxSize = 0;
        [SerializeField] private bool _autoExpand = true;

        public string PoolId => _poolId;
        public GameObject Prefab => _prefab;
        public int InitialSize => _initialSize;
        public int MaxSize => _maxSize;
        public bool AutoExpand => _autoExpand;
    }
}

