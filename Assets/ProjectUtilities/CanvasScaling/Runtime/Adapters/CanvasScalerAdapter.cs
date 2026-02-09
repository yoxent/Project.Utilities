using UnityEngine;
using UnityEngine.UI;
using ProjectUtilities.CanvasScaling.Core;

namespace ProjectUtilities.CanvasScaling.Adapters
{
    /// <summary>
    /// Attach to a GameObject with Canvas (and CanvasScaler). Registers with CanvasScalingManager
    /// and applies the active profile rule when the manager refreshes.
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    public class CanvasScalerAdapter : MonoBehaviour
    {
        private Canvas _canvas;
        private CanvasScaler _scaler;

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _scaler = GetComponent<CanvasScaler>();
            if (_scaler == null)
                _scaler = gameObject.AddComponent<CanvasScaler>();
        }

        private void OnEnable()
        {
            if (CanvasScalingManager.Instance != null)
                CanvasScalingManager.Instance.Register(this);
        }

        private void OnDisable()
        {
            if (CanvasScalingManager.Instance != null)
                CanvasScalingManager.Instance.Unregister(this);
        }

        /// <summary>
        /// Applies the given rule to this canvas's CanvasScaler. Called by CanvasScalingManager.
        /// </summary>
        public void ApplyRule(CanvasScalingRule rule)
        {
            if (rule == null || _scaler == null)
                return;

            _scaler.uiScaleMode = rule.ScaleMode;
            _scaler.referenceResolution = rule.ReferenceResolution;
            _scaler.matchWidthOrHeight = rule.MatchWidthOrHeight;
        }
    }
}
