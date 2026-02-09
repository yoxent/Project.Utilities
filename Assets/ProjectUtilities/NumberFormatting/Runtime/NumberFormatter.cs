using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

namespace ProjectUtilities.NumberFormatting
{
    /// <summary>
    /// Formats numbers with abbreviations (K, M, B, T) using a <see cref="NumberFormattingConfig"/>.
    /// Use static methods with a config reference, or create an instance and assign config once.
    /// </summary>
    public class NumberFormatter
    {
        private static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;

        private NumberFormattingConfig _config;

        /// <summary>Config used when calling instance Format methods. Assign in inspector or code.</summary>
        public NumberFormattingConfig Config
        {
            get => _config;
            set => _config = value;
        }

        public NumberFormatter() { }

        public NumberFormatter(NumberFormattingConfig config)
        {
            _config = config;
        }

        /// <summary>Format with the instance's assigned config. Returns raw number if config is null.</summary>
        public string Format(double value)
        {
            return Format(value, _config);
        }

        /// <summary>Format with the instance's assigned config. Returns raw number if config is null.</summary>
        public string Format(long value)
        {
            return Format((double)value, _config);
        }

        /// <summary>Format a value using the given config. Returns value.ToString() if config is null.</summary>
        public static string Format(double value, NumberFormattingConfig config)
        {
            if (config == null)
                return value.ToString(Invariant);

            var tiers = config.Tiers;
            if (tiers == null || tiers.Count == 0)
                return FormatFull(value, config);

            double abs = System.Math.Abs(value);
            SuffixTier best = null;
            foreach (var t in tiers)
            {
                if (t == null) continue;
                if (abs >= t.Threshold)
                    best = t;
            }

            if (best == null)
                return FormatFull(value, config);

            double scaled = value / best.Threshold;
            int decimals = config.DecimalPlacesWhenAbbreviated;
            string num = decimals > 0
                ? scaled.ToString("F" + decimals, Invariant)
                : System.Math.Round(scaled, 0).ToString(Invariant);
            return config.Prefix + num + best.Suffix;
        }

        /// <summary>Format a value using the given config. Returns value.ToString() if config is null.</summary>
        public static string Format(long value, NumberFormattingConfig config)
        {
            return Format((double)value, config);
        }

        private static string FormatFull(double value, NumberFormattingConfig config)
        {
            int decimals = config.DecimalPlacesWhenFull;
            string sep = config.ThousandsSeparator ?? "";
            string num = decimals > 0
                ? value.ToString("F" + decimals, Invariant)
                : System.Math.Round(value, 0).ToString(Invariant);
            if (!string.IsNullOrEmpty(sep))
                num = AddThousandsSeparator(num, sep);
            return config.Prefix + num;
        }

        private static string AddThousandsSeparator(string num, string separator)
        {
            int dot = num.IndexOf('.');
            string integerPart = dot >= 0 ? num.Substring(0, dot) : num;
            string decimalPart = dot >= 0 ? num.Substring(dot) : "";
            bool negative = integerPart.StartsWith("-");
            if (negative) integerPart = integerPart.Substring(1);
            var grouped = new List<string>();
            for (int i = integerPart.Length; i > 0; i -= 3)
            {
                int start = System.Math.Max(0, i - 3);
                grouped.Add(integerPart.Substring(start, i - start));
            }
            grouped.Reverse();
            string result = string.Join(separator, grouped);
            if (negative) result = "-" + result;
            return result + decimalPart;
        }
    }
}
