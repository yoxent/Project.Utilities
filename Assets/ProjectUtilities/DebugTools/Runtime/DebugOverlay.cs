using System.Text;
using TMPro;
using UnityEngine;

namespace ProjectUtilities.DebugTools.Runtime
{
    /// <summary>
    /// Simple FPS and memory usage overlay for diagnostics.
    /// Attach to a Canvas Text element.
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class DebugOverlay : MonoBehaviour
    {
        [SerializeField] private float _updateInterval = 0.5f;

        private TextMeshProUGUI _text;
        private float _accumulatedTime;
        private int _frameCount;
        private float _timeLeft;

        private void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();
            _timeLeft = _updateInterval;
        }

        private void Update()
        {
            _timeLeft -= Time.unscaledDeltaTime;
            _accumulatedTime += Time.unscaledDeltaTime;
            _frameCount++;

            if (_timeLeft <= 0f)
            {
                var fps = _frameCount / Mathf.Max(_accumulatedTime, 0.0001f);
                var memoryMb = (System.GC.GetTotalMemory(false) / (1024f * 1024f));

                var sb = new StringBuilder(64);
                sb.Append("FPS: ").Append(fps.ToString("F1")).AppendLine();
                sb.Append("Memory: ").Append(memoryMb.ToString("F1")).Append(" MB");

                _text.text = sb.ToString();

                _timeLeft = _updateInterval;
                _accumulatedTime = 0f;
                _frameCount = 0;
            }
        }
    }
}

