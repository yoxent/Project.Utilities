using UnityEngine;
using UnityEngine.InputSystem;

namespace ProjectUtilities.Parallax.Core
{
    public enum ParallaxMode
    {
        PointerBased,
        AutoScroll,
        CameraBased
    }

    /// <summary>
    /// Drives a set of ParallaxLayer components based on pointer movement, time, or camera position.
    /// Designed for 2D parallax backgrounds (menus or side-scrollers).
    /// </summary>
    public class ParallaxController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ParallaxConfig _config;
        [SerializeField] private ParallaxLayer[] _layers;

        [Header("Mode")]
        [SerializeField] private ParallaxMode _mode = ParallaxMode.PointerBased;

        private Vector2 _currentOffset;
        private Vector2 _velocity;
        private Camera _camera;
        private bool _isEnabled = true;
        private Vector3 _referenceCameraPosition;

        private void Awake()
        {
            _camera = Camera.main;
        }

        private void Start()
        {
            if (_camera != null)
            {
                _referenceCameraPosition = _camera.transform.position;
            }
        }

        private void Update()
        {
            if (!_isEnabled || _config == null || _layers == null || _layers.Length == 0)
            {
                return;
            }

            if (_mode == ParallaxMode.AutoScroll)
            {
                foreach (var layer in _layers)
                {
                    if (!layer) continue;
                    layer.UpdateAutoScroll(Time.deltaTime, _config.BaseIntensity);
                }
                return;
            }

            if (_mode == ParallaxMode.CameraBased)
            {
                if (_camera == null)
                {
                    _camera = Camera.main;
                }

                if (_camera != null)
                {
                    var cameraPos = _camera.transform.position;
                    var offset = new Vector2(
                        cameraPos.x - _referenceCameraPosition.x,
                        cameraPos.y - _referenceCameraPosition.y);

                    foreach (var layer in _layers)
                    {
                        if (!layer) continue;
                        layer.ApplyCameraOffset(offset);
                    }
                }
                return;
            }

            // Pointer-based parallax: compute a normalized offset and apply to all layers.
            var targetOffset = GetPointerOffset();

            _currentOffset = Vector2.SmoothDamp(
                _currentOffset,
                targetOffset,
                ref _velocity,
                _config.SmoothTime);

            foreach (var layer in _layers)
            {
                if (!layer)
                {
                    continue;
                }

                layer.ApplyOffset(_currentOffset, _config.BaseIntensity, _config.MaxOffset);
            }
        }

        private Vector2 GetPointerOffset()
        {
            if (_camera == null)
            {
                _camera = Camera.main;
                if (_camera == null)
                {
                    return Vector2.zero;
                }
            }

            var mouse = Mouse.current;
            if (mouse == null)
            {
                return Vector2.zero;
            }

            var mousePos = mouse.position.ReadValue();
            var viewport = _camera.ScreenToViewportPoint(mousePos);
            var centered = new Vector2(viewport.x - 0.5f, viewport.y - 0.5f);
            return centered * 2f; // range roughly [-1,1]
        }

        public void SetIntensity(float intensity)
        {
            if (_config == null)
            {
                return;
            }

            // This preserves ScriptableObject but allows runtime tuning via multiplier.
            _currentOffset *= intensity / Mathf.Max(_config.BaseIntensity, 0.0001f);
        }

        public void EnableParallax(bool enabled)
        {
            _isEnabled = enabled;
            if (!enabled)
            {
                _currentOffset = Vector2.zero;
                _velocity = Vector2.zero;
            }
        }

        /// <summary>
        /// Sets the reference position used for camera-based parallax. Call when the camera's "origin" changes (e.g. level load).
        /// </summary>
        public void SetCameraReferencePosition(Vector3 worldPosition)
        {
            _referenceCameraPosition = worldPosition;
        }
    }
}

