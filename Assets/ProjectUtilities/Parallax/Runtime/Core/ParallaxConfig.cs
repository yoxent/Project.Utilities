using UnityEngine;

namespace ProjectUtilities.Parallax.Core
{
    /// <summary>
    /// Global settings for parallax behaviour. Fields are grouped by mode.
    /// </summary>
    [CreateAssetMenu(fileName = "ParallaxConfig", menuName = "ProjectUtilities/Parallax/Config")]
    public class ParallaxConfig : ScriptableObject
    {
        [Header("Shared")]
        [SerializeField] private float _baseIntensity = 1f;

        [Header("Pointer (Mouse)")]
        [Tooltip("Maximum offset magnitude for pointer-based parallax.")]
        [SerializeField] private float _maxOffset = 1f;
        [Tooltip("Smooth time for pointer offset smoothing.")]
        [SerializeField] private float _smoothTime = 0.1f;

        [Header("Auto Scroll")]
        // Uses Shared BaseIntensity only.

        [Header("Camera")]
        // Reserved for camera-based mode settings.

        public float BaseIntensity => _baseIntensity;
        public float MaxOffset => _maxOffset;
        public float SmoothTime => _smoothTime;
    }
}

