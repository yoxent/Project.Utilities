using UnityEngine;
using TMPro;

namespace ProjectUtilities.NumberFormatting
{
    /// <summary>
    /// Demo controller for BigNumbers_Demo: shows formatted numbers using the assigned config.
    /// Assign a NumberFormattingConfig and optional TextMeshProUGUI labels; samples update in Update.
    /// </summary>
    public class BigNumbersDemoController : MonoBehaviour
    {
        [SerializeField] private NumberFormattingConfig _config;
        [SerializeField] private double _sampleValue = 1234567.89;

        [Header("Optional labels (TMP)")]
        [SerializeField] private TextMeshProUGUI _mainLabel;
        [SerializeField] private TextMeshProUGUI _samplesLabel;

        private void Update()
        {
            if (_config == null) return;

            if (_mainLabel != null)
                _mainLabel.text = $"Value: {NumberFormatter.Format(_sampleValue, _config)}";

            if (_samplesLabel != null)
            {
                _samplesLabel.text = "Samples:\n"
                    + $"  1,000 → {NumberFormatter.Format(1000, _config)}\n"
                    + $"  1.5M  → {NumberFormatter.Format(1500000, _config)}\n"
                    + $"  2.3B  → {NumberFormatter.Format(2300000000, _config)}\n"
                    + $"  7.89T → {NumberFormatter.Format(7890000000000, _config)}";
            }
        }
    }
}
