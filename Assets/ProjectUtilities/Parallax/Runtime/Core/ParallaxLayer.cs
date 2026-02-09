using UnityEngine;

namespace ProjectUtilities.Parallax.Core
{
    /// <summary>
    /// Attach to a 2D background layer (sprite or UI element) to have it moved by a ParallaxController.
    /// Supports Pointer, Auto Scroll, and Camera-based parallax.
    /// </summary>
    public class ParallaxLayer : MonoBehaviour
    {
        [Header("Structure")]
        [Tooltip("Optional. Assign the child that contains the tile visual. When set, this GameObject is the empty parent; the duplicate is created as another child so draw order is correct. Leave empty to use this GameObject as the tile (legacy). Used in: Auto Scroll.")]
        [SerializeField] private Transform _tile;

        [Header("Shared")]
        [Tooltip("Depth strength: multiplier for layer movement (e.g. far = 0.5, near = 1.5). Used in: Pointer, Auto Scroll, Camera.")]
        [SerializeField] private float _depthFactor = 1f;

        [Header("Auto Scroll")]
        [Tooltip("Units per second in X/Y. Sign controls direction; use 0 on an axis for no scroll on that axis. Used in: Auto Scroll.")]
        [SerializeField] private Vector2 _scrollSpeed = new Vector2(0.2f, 0f);

        [Tooltip("If true, the layer will loop when it moves beyond the computed tile size. Used in: Auto Scroll.")]
        [SerializeField] private bool _loop = true;

        [Tooltip("When true and looping, a second tile is created so there are no gaps when scroll starts. Used in: Auto Scroll.")]
        [SerializeField] private bool _duplicateTileForSeamlessScroll = true;

        [Header("Pointer (Mouse)")]
        [Tooltip("Multiplier for pointer-based movement (1 = default). Increase for more visible parallax (e.g. 2 = double). Used in: Pointer.")]
        [SerializeField] [Min(0.01f)] private float _pointerStrength = 1f;

        [Header("Camera")]
        [Tooltip("How much this layer follows camera movement: 0 = fixed (no parallax), 1 = 1:1 with camera. Used in: Camera.")]
        [SerializeField] [Range(0f, 1f)] private float _scrollFactor = 0.5f;

        private Vector3 _basePosition;
        private Vector3 _tileBasePosition;
        private Vector2 _computedLoopDistance;
        private Vector2 _scrollOffset;
        private Transform _duplicateTile;

        // Camera-based mode: track accumulated parallax-driven scroll along X and last frame's parallax
        private float _cameraScrollOffsetX;
        private float _lastParallaxX;

        private static bool s_creatingSeamlessClone;

        public float DepthFactor => _depthFactor;
        public float ScrollFactor => _scrollFactor;

        private const float Epsilon = 0.0001f;

        private void Awake()
        {
            if (s_creatingSeamlessClone)
                return;

            _basePosition = transform.localPosition;
            _tileBasePosition = _tile != null ? _tile.localPosition : Vector3.zero;
            _computedLoopDistance = ComputeLoopDistance();
            // Duplicate is created lazily in UpdateAutoScroll or ApplyCameraOffset so pointer-based mode never creates it.
        }

        /// <summary>
        /// Creates the seamless duplicate tile when needed (Auto Scroll or Camera mode). Not used in Pointer mode.
        /// </summary>
        private void EnsureSeamlessDuplicate()
        {
            if (_duplicateTile != null)
                return;
            if (!_loop || (Mathf.Abs(_computedLoopDistance.x) < Epsilon && Mathf.Abs(_computedLoopDistance.y) < Epsilon))
                return;

            if (_tile != null)
            {
                var clone = Instantiate(_tile.gameObject, transform);
                clone.name = _tile.name + " (Seamless Clone)";
                _duplicateTile = clone.transform;
                _duplicateTile.localPosition = _tileBasePosition + GetLoopDistanceVector3();
            }
            else
            {
                s_creatingSeamlessClone = true;
                var clone = Instantiate(gameObject, transform.parent);
                s_creatingSeamlessClone = false;

                clone.name = gameObject.name + " (Seamless Clone)";
                var layerOnClone = clone.GetComponent<ParallaxLayer>();
                if (layerOnClone != null)
                {
#if UNITY_EDITOR
                    // In EditMode tests we cannot use Destroy; use DestroyImmediate instead.
                    if (!Application.isPlaying)
                    {
                        DestroyImmediate(layerOnClone);
                    }
                    else
#endif
                    {
                        Destroy(layerOnClone);
                    }
                }
                _duplicateTile = clone.transform;
                _duplicateTile.localPosition = _basePosition + GetLoopDistanceVector3();
            }
        }

        /// <summary>
        /// Pointer-based mode: applies a normalized offset from mouse position (X and Y), clamped to maxOffset.
        /// </summary>
        public void ApplyOffset(Vector2 normalizedOffset, float intensity, float maxOffset)
        {
            var offset = new Vector2(
                normalizedOffset.x * intensity * _depthFactor * _pointerStrength,
                normalizedOffset.y * intensity * _depthFactor * _pointerStrength);

            offset = Vector2.ClampMagnitude(offset, maxOffset * _pointerStrength);

            var target = _basePosition;
            target.x += offset.x;
            target.y += offset.y;

            transform.localPosition = target;
        }

        /// <summary>
        /// Camera-based mode: offsets this layer by (cameraOffset * ScrollFactor * DepthFactor) on X only.
        /// Y is ignored for camera parallax.
        /// When looping is enabled, behaves similarly to auto-scroll: A and B are moved independently along X and, when one
        /// tile has moved a full width off-screen in the scroll direction, only that tile is repositioned to the far side
        /// of the other tile. This yields an alternating ...ABAB... pattern with no gaps.
        /// </summary>
        public void ApplyCameraOffset(Vector2 cameraOffset)
        {
            float loopX = _computedLoopDistance.x;
            bool hasLoopX = loopX > Epsilon;

            var driveA = _tile != null ? _tile : transform;
            var basePosA = _tile != null ? _tileBasePosition : _basePosition;

            // Camera-based parallax only affects X
            float parallaxXCurrent = cameraOffset.x * _scrollFactor * _depthFactor;

            if (_loop && hasLoopX)
            {
                EnsureSeamlessDuplicate();
                if (_duplicateTile != null)
                {
                    // Incremental parallax since last frame
                    float deltaParallaxX = parallaxXCurrent - _lastParallaxX;
                    _lastParallaxX = parallaxXCurrent;

                    // Move both tiles by the incremental parallax
                    var posA = driveA.localPosition;
                    var posB = _duplicateTile.localPosition;

                    posA.x += deltaParallaxX;
                    posB.x += deltaParallaxX;

                    // Simple thresholds: when a tile's local X has moved one width off-screen in parallax space,
                    // move only that tile to the far side of the other.
                    if (Mathf.Abs(loopX) > Epsilon && Mathf.Abs(deltaParallaxX) > Epsilon)
                    {
                        if (deltaParallaxX < 0f)
                        {
                            // Camera effectively moved right-to-left for this layer.
                            if (posA.x <= -loopX - Epsilon)
                            {
                                posA.x = posB.x + loopX;
                            }
                            else if (posB.x <= -loopX - Epsilon)
                            {
                                posB.x = posA.x + loopX;
                            }
                        }
                        else if (deltaParallaxX > 0f)
                        {
                            // Camera effectively moved left-to-right for this layer.
                            if (posA.x >= loopX + Epsilon)
                            {
                                posA.x = posB.x - loopX;
                            }
                            else if (posB.x >= loopX + Epsilon)
                            {
                                posB.x = posA.x - loopX;
                            }
                        }
                    }

                    // Y is ignored in camera mode; keep original local Y/Z from basePosA/_basePosition.
                    posA.y = basePosA.y;
                    posB.y = basePosA.y;

                    driveA.localPosition = posA;
                    _duplicateTile.localPosition = posB;
                    return;
                }
            }

            // No looping / no valid width → simple 1‑tile parallax on X
            var target = basePosA;
            target.x += parallaxXCurrent;
            driveA.localPosition = target;
        }

        /// <summary>
        /// Auto-scroll mode: continuously scrolls and optionally wraps when exceeding LoopDistance.
        /// When using two tiles for seamless scroll, both are driven from a single wrapped offset so no gaps appear.
        /// </summary>
        public void UpdateAutoScroll(float deltaTime, float globalIntensity)
        {
            var delta = _scrollSpeed * (_depthFactor * globalIntensity) * deltaTime;

            if (_duplicateTileForSeamlessScroll && _loop &&
                (Mathf.Abs(_computedLoopDistance.x) > Epsilon || Mathf.Abs(_computedLoopDistance.y) > Epsilon) &&
                _duplicateTile == null)
            {
                EnsureSeamlessDuplicate();
            }

            if (_duplicateTile != null)
            {
                // Track scroll offset primarily for Y; we will handle X per-tile so we can move one tile at a time.
                _scrollOffset += delta;

                float loopX = _computedLoopDistance.x;
                float loopY = _computedLoopDistance.y;

                // Wrap Y offsets if we have a meaningful loop distance on Y
                if (loopY > 0.0001f)
                {
                    while (_scrollOffset.y <= -loopY) _scrollOffset.y += loopY;
                    while (_scrollOffset.y >=  loopY) _scrollOffset.y -= loopY;
                }

                float baseY   = _tile != null ? _tileBasePosition.y : _basePosition.y;
                float yOffset = _scrollOffset.y;

                var driveA = _tile != null ? _tile : transform; // A (original)
                var driveB = _duplicateTile;                    // B (duplicate)

                var posA = driveA.localPosition;
                var posB = driveB.localPosition;

                // Apply horizontal delta to both tiles
                posA.x += delta.x;
                posB.x += delta.x;

                // Only if we have a valid horizontal loop and non-zero horizontal scroll
                if (Mathf.Abs(loopX) > Epsilon && Mathf.Abs(_scrollSpeed.x) > Epsilon)
                {
                    if (_scrollSpeed.x < 0f)
                    {
                        // Moving right-to-left.
                        // When a tile's X is <= -tileWidth (fully off to the left), move ONLY that tile
                        // to the right of the other tile by exactly one width.
                        if (posA.x <= -loopX - Epsilon)
                        {
                            posA.x = posB.x + loopX;
                        }
                        else if (posB.x <= -loopX - Epsilon)
                        {
                            posB.x = posA.x + loopX;
                        }
                    }
                    else if (_scrollSpeed.x > 0f)
                    {
                        // Moving left-to-right.
                        // When a tile's X is >= +tileWidth (fully off to the right), move ONLY that tile
                        // to the left of the other tile by exactly one width.
                        if (posA.x >= loopX + Epsilon)
                        {
                            posA.x = posB.x - loopX;
                        }
                        else if (posB.x >= loopX + Epsilon)
                        {
                            posB.x = posA.x - loopX;
                        }
                    }
                }

                // Apply updated positions with shared Y offset
                posA.y = baseY + yOffset;
                posB.y = baseY + yOffset;

                driveA.localPosition = posA;
                driveB.localPosition = posB;
                return;
            }

            var drive = _tile != null ? _tile : transform;
            var basePos = _tile != null ? _tileBasePosition : _basePosition;
            var pos = drive.localPosition;
            pos += (Vector3)delta;

            if (_loop)
            {
                if (Mathf.Abs(_computedLoopDistance.x) > 0.0001f)
                {
                    var offsetX = pos.x - basePos.x;
                    var limitX = _computedLoopDistance.x;
                    if (offsetX <= -limitX) pos.x += limitX;
                    else if (offsetX >= limitX) pos.x -= limitX;
                }
                if (Mathf.Abs(_computedLoopDistance.y) > 0.0001f)
                {
                    var offsetY = pos.y - basePos.y;
                    var limitY = _computedLoopDistance.y;
                    if (offsetY <= -limitY) pos.y += limitY;
                    else if (offsetY >= limitY) pos.y -= limitY;
                }
            }

            drive.localPosition = pos;
        }

        /// <summary>
        /// Loop distance for duplicate tile placement. Only applies components on axes that are scrolling so e.g. horizontal-only scroll keeps duplicate Y aligned.
        /// </summary>
        private Vector3 GetLoopDistanceVector3()
        {
            float x = Mathf.Abs(_scrollSpeed.x) > Epsilon ? _computedLoopDistance.x : 0f;
            float y = Mathf.Abs(_scrollSpeed.y) > Epsilon ? _computedLoopDistance.y : 0f;
            return new Vector3(x, y, 0f);
        }

        private Vector2 ComputeLoopDistance()
        {
            var result = Vector2.zero;
            var source = _tile != null ? _tile : transform;

            if (source.TryGetComponent<RectTransform>(out var rectTransform))
            {
                var contentSize = GetContentSizeForLoop(rectTransform);
                if (contentSize.x > 0.0001f) result.x = contentSize.x;
                if (contentSize.y > 0.0001f) result.y = contentSize.y;
                if (result.x < 0.0001f) result.x = rectTransform.rect.size.x;
                if (result.y < 0.0001f) result.y = rectTransform.rect.size.y;
            }
            else if (source.TryGetComponent<Renderer>(out var renderer))
            {
                var size = renderer.bounds.size;
                result.x = size.x;
                result.y = size.y;
            }

            return result;
        }

        /// <summary>
        /// Gets the size of the repeating content (first descendant with non-zero rect) so loop distance matches tile width and no gap appears.
        /// </summary>
        private Vector2 GetContentSizeForLoop(RectTransform self)
        {
            var content = GetFirstNonZeroRectSize(self);
            if (content.x > 0.0001f || content.y > 0.0001f)
                return content;
            return self.rect.size;
        }

        private static Vector2 GetFirstNonZeroRectSize(RectTransform rt)
        {
            if (rt == null) return Vector2.zero;
            var r = rt.rect.size;
            if (r.x > 0.0001f || r.y > 0.0001f)
                return r;
            for (int i = 0; i < rt.childCount; i++)
            {
                var child = rt.GetChild(i) as RectTransform;
                var childSize = GetFirstNonZeroRectSize(child);
                if (childSize.x > 0.0001f || childSize.y > 0.0001f)
                    return childSize;
            }
            return Vector2.zero;
        }
    }
}

