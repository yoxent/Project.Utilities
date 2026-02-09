using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectUtilities.NumberFormatting
{
    /// <summary>
    /// One suffix tier: threshold and suffix (e.g. 1000, "K").
    /// </summary>
    [Serializable]
    public class SuffixTier
    {
        [Tooltip("Use this suffix when value >= threshold (e.g. 1000 for K).")]
        [SerializeField] private double _threshold = 1000;

        [Tooltip("Suffix to append (e.g. K, M, B, T).")]
        [SerializeField] private string _suffix = "K";

        public double Threshold => _threshold;
        public string Suffix => _suffix ?? string.Empty;

        public SuffixTier() { }

        public SuffixTier(double threshold, string suffix)
        {
            _threshold = threshold;
            _suffix = suffix ?? string.Empty;
        }
    }

    /// <summary>
    /// Configurable suffixes and formatting for large numbers (idle/incremental games).
    /// Create via Assets → Create → ProjectUtilities → NumberFormatting → Config.
    /// </summary>
    [CreateAssetMenu(fileName = "NumberFormattingConfig", menuName = "ProjectUtilities/NumberFormatting/Config")]
    public class NumberFormattingConfig : ScriptableObject
    {
        [Header("Suffix tiers (order: smallest threshold first)")]
        [SerializeField] private List<SuffixTier> _tiers = new List<SuffixTier>();

        [Header("Formatting")]
        [Tooltip("Decimal places when value is abbreviated (e.g. 1.23K).")]
        [SerializeField] [Range(0, 6)] private int _decimalPlacesWhenAbbreviated = 2;

        [Tooltip("Decimal places when value is shown in full (no suffix).")]
        [SerializeField] [Range(0, 6)] private int _decimalPlacesWhenFull = 0;

        [Tooltip("Character between thousands (e.g. ',' or ' '). Use empty for none.")]
        [SerializeField] private string _thousandsSeparator = ",";

        [Tooltip("Optional prefix (e.g. $ for currency).")]
        [SerializeField] private string _prefix = "";

        /// <summary>Tiers ordered by threshold ascending (smallest first).</summary>
        public IReadOnlyList<SuffixTier> Tiers => _tiers ?? new List<SuffixTier>();

        public int DecimalPlacesWhenAbbreviated => _decimalPlacesWhenAbbreviated;
        public int DecimalPlacesWhenFull => _decimalPlacesWhenFull;
        public string ThousandsSeparator => _thousandsSeparator ?? "";
        public string Prefix => _prefix ?? "";

        private void OnValidate()
        {
            if (_tiers == null) _tiers = new List<SuffixTier>();
            _tiers.Sort((a, b) => (a?.Threshold ?? 0).CompareTo(b?.Threshold ?? 0));
        }
    }
}
