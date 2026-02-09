using System.Collections.Generic;
using UnityEngine;
using ProjectUtilities.CanvasScaling.Adapters;

namespace ProjectUtilities.CanvasScaling.Core
{
    /// <summary>
    /// Applies the active scaling rule from a profile to registered CanvasScalerAdapters.
    /// Call from a bootstrap or persistent GameObject; adapters register themselves on enable.
    /// </summary>
    public class CanvasScalingManager : MonoBehaviour
    {
        [SerializeField] private CanvasScalingProfile _profile;

        private static CanvasScalingManager _instance;
        private readonly List<CanvasScalerAdapter> _adapters = new List<CanvasScalerAdapter>();
        private DeviceCategory _lastCategory = DeviceCategory.Default;
        private Vector2Int _lastResolution;

        /// <summary>Current profile. Assign in inspector or at runtime.</summary>
        public CanvasScalingProfile Profile
        {
            get => _profile;
            set => _profile = value;
        }

        /// <summary>Singleton-style access. May be null if no manager in scene.</summary>
        public static CanvasScalingManager Instance => _instance;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            if (_instance == this)
                _instance = null;

            _adapters.Clear();
        }

        private void Start()
        {
            RefreshAll();
        }

        private void Update()
        {
            var res = new Vector2Int(Screen.width, Screen.height);
            if (res != _lastResolution)
            {
                _lastResolution = res;
                RefreshAll();
            }
        }

        /// <summary>
        /// Registers an adapter. Called by CanvasScalerAdapter.OnEnable.
        /// </summary>
        public void Register(CanvasScalerAdapter adapter)
        {
            if (adapter != null && !_adapters.Contains(adapter))
                _adapters.Add(adapter);
        }

        /// <summary>
        /// Unregisters an adapter. Called by CanvasScalerAdapter.OnDisable.
        /// </summary>
        public void Unregister(CanvasScalerAdapter adapter)
        {
            _adapters.Remove(adapter);
        }

        /// <summary>
        /// Re-evaluates device category and applies the current rule to all registered adapters.
        /// </summary>
        public void RefreshAll()
        {
            if (_profile == null)
                return;

            _lastCategory = GetCurrentCategory();
            var rule = _profile.GetRule(_lastCategory);

            foreach (var adapter in _adapters)
            {
                if (adapter != null)
                    adapter.ApplyRule(rule);
            }
        }

        /// <summary>
        /// Current device category based on screen aspect ratio.
        /// Override or extend for DPI/screen size if needed.
        /// </summary>
        public virtual DeviceCategory GetCurrentCategory()
        {
            float w = Screen.width;
            float h = Screen.height;
            if (h <= 0f) return DeviceCategory.Default;
            float aspect = w / h;

            if (aspect >= 1.4f) return DeviceCategory.Desktop;
            if (aspect <= 0.75f) return DeviceCategory.Phone;
            return DeviceCategory.Tablet;
        }
    }
}
