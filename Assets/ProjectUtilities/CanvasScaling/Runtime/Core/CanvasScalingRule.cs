using UnityEngine;
using UnityEngine.UI;

namespace ProjectUtilities.CanvasScaling.Core
{
    /// <summary>
    /// One scaling rule: reference resolution and match weight for Scale With Screen Size.
    /// </summary>
    [System.Serializable]
    public class CanvasScalingRule
    {
        [SerializeField] private Vector2 _referenceResolution = new Vector2(1920f, 1080f);
        [SerializeField] [Range(0f, 1f)] private float _matchWidthOrHeight = 0.5f;
        [SerializeField] private CanvasScaler.ScaleMode _scaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

        public Vector2 ReferenceResolution => _referenceResolution;
        public float MatchWidthOrHeight => _matchWidthOrHeight;
        public CanvasScaler.ScaleMode ScaleMode => _scaleMode;
    }
}
