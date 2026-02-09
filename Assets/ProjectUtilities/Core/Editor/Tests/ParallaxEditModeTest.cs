using System.Reflection;
using NUnit.Framework;
using ProjectUtilities.Parallax.Core;
using UnityEngine;

namespace ProjectUtilities.Parallax.Editor
{
    /// <summary>
    /// EditMode tests for Parallax: pointer-based, auto-scroll, and camera-based modes.
    /// These tests operate directly on ParallaxLayer (and sometimes ParallaxController)
    /// to verify core math and mode behaviour without requiring a full scene.
    /// </summary>
    public class ParallaxEditModeTest
    {
        private GameObject _layerGo;
        private ParallaxLayer _layer;

        [SetUp]
        public void SetUp()
        {
            _layerGo = new GameObject("ParallaxLayer_Test");
            _layer = _layerGo.AddComponent<ParallaxLayer>();

            // Ensure Awake() runs so base positions are captured.
            InvokePrivateMethod(_layer, "Awake");
        }

        [TearDown]
        public void TearDown()
        {
            if (_layerGo != null)
            {
                Object.DestroyImmediate(_layerGo);
            }
        }

        [Test]
        public void PointerBased_ApplyOffset_UsesDepthAndPointerStrengthAndClamps()
        {
            // Arrange: depth and pointer strength amplify the effect, but final offset is clamped.
            SetPrivateField(_layer, "_depthFactor", 2f);
            SetPrivateField(_layer, "_pointerStrength", 1.5f);

            _layer.transform.localPosition = Vector3.zero;

            var normalizedOffset = new Vector2(1f, 0f);
            float intensity = 1f;
            float maxOffset = 0.5f; // base max

            // Act
            _layer.ApplyOffset(normalizedOffset, intensity, maxOffset);

            // depth * pointerStrength would try to push to 3 units, but clamp is maxOffset * pointerStrength = 0.75.
            var pos = _layer.transform.localPosition;
            Assert.That(pos.x, Is.InRange(0.74f, 0.76f), "Pointer-based parallax should be clamped by maxOffset * pointerStrength.");
            Assert.That(pos.y, Is.EqualTo(0f).Within(0.0001f), "Y offset should remain 0 for this test.");

            // Pointer mode should never create a duplicate tile.
            var duplicate = GetPrivateField<Transform>(_layer, "_duplicateTile");
            Assert.That(duplicate, Is.Null, "Pointer mode must not create a duplicate tile.");
        }

        [Test]
        public void AutoScroll_WithoutDuplicate_MovesSingleTile()
        {
            // Arrange: disable seamless duplicate, enable looping, set horizontal scroll speed.
            SetPrivateField(_layer, "_duplicateTileForSeamlessScroll", false);
            SetPrivateField(_layer, "_loop", true);
            SetPrivateField(_layer, "_scrollSpeed", new Vector2(1f, 0f)); // 1 unit per second
            SetPrivateField(_layer, "_depthFactor", 1f);

            _layer.transform.localPosition = Vector3.zero;

            // Act: 1 second of auto-scroll at intensity 1.
            _layer.UpdateAutoScroll(deltaTime: 1f, globalIntensity: 1f);

            var pos = _layer.transform.localPosition;
            Assert.That(pos.x, Is.InRange(0.99f, 1.01f), "Auto-scroll should move the layer by scrollSpeed.x * deltaTime when depthFactor and intensity are 1.");
        }

        [Test]
        public void CameraBased_NoLoop_OffsetsXOnly()
        {
            // Arrange: no looping, simple X parallax.
            SetPrivateField(_layer, "_loop", false);
            SetPrivateField(_layer, "_scrollFactor", 0.5f);
            SetPrivateField(_layer, "_depthFactor", 2f);

            _layer.transform.localPosition = new Vector3(0f, 5f, 0f);
            // Re-run Awake so base position is updated to current local position.
            InvokePrivateMethod(_layer, "Awake");

            var cameraOffset = new Vector2(4f, 3f); // Y should be ignored in camera mode.

            // Act
            _layer.ApplyCameraOffset(cameraOffset);

            var pos = _layer.transform.localPosition;
            // X: baseX + cameraOffset.x * scrollFactor * depthFactor = 0 + 4 * 0.5 * 2 = 4
            Assert.That(pos.x, Is.InRange(3.99f, 4.01f), "Camera-based X parallax should use cameraOffset.x * scrollFactor * depthFactor.");
            // Y should remain at the base Y from Awake.
            Assert.That(pos.y, Is.InRange(4.99f, 5.01f), "Camera-based parallax should not modify Y.");
        }

        [Test]
        public void CameraBased_WithLoop_MaintainsTileSpacing()
        {
            // Arrange: horizontal loop with a simple 2-unit tile width.
            SetPrivateField(_layer, "_loop", true);
            SetPrivateField(_layer, "_scrollFactor", 1f);
            SetPrivateField(_layer, "_depthFactor", 1f);
            SetPrivateField(_layer, "_computedLoopDistance", new Vector2(2f, 0f));

            // First call: offset 0 should create the duplicate but not move yet.
            _layer.ApplyCameraOffset(Vector2.zero);

            var duplicate = GetPrivateField<Transform>(_layer, "_duplicateTile");
            Assert.That(duplicate, Is.Not.Null, "Camera-based loop should lazily create a duplicate tile.");

            // Simulate several frames of camera movement to the right (positive X).
            _layer.ApplyCameraOffset(new Vector2(0.5f, 0f));
            _layer.ApplyCameraOffset(new Vector2(1.0f, 0f));
            _layer.ApplyCameraOffset(new Vector2(1.5f, 0f));
            _layer.ApplyCameraOffset(new Vector2(2.5f, 0f));

            var driveA = _layer.transform;
            var posA = driveA.localPosition;
            var posB = duplicate.localPosition;

            float loopX = 2f;
            float distance = Mathf.Abs(posA.x - posB.x);

            // The two tiles should remain roughly one loop distance apart, ensuring seamless coverage.
            Assert.That(distance, Is.InRange(loopX - 0.01f, loopX + 0.01f),
                "In camera-based loop mode, the main and duplicate tiles should stay one tile-width apart along X.");
        }

        #region Reflection helpers

        private static void SetPrivateField<T>(object target, string fieldName, T value)
        {
            var field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null, $"Field '{fieldName}' not found on {target.GetType().Name}.");
            field.SetValue(target, value);
        }

        private static T GetPrivateField<T>(object target, string fieldName)
        {
            var field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null, $"Field '{fieldName}' not found on {target.GetType().Name}.");
            return (T)field.GetValue(target);
        }

        private static void InvokePrivateMethod(object target, string methodName)
        {
            var method = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(method, Is.Not.Null, $"Method '{methodName}' not found on {target.GetType().Name}.");
            method.Invoke(target, null);
        }

        #endregion
    }
}

