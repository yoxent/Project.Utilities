using System;
using System.Collections.Generic;
using ProjectUtilities.Core;
using UnityEngine;

namespace ProjectUtilities.Localization.Core
{
    /// <summary>
    /// Central localization manager. Initialize early (e.g., in a bootstrap scene)
    /// with a LocalizationConfig, then use Get/TryGet to retrieve localized strings.
    /// </summary>
    public class LocalizationManager
    {
        private readonly Dictionary<LanguageCode, Dictionary<string, string>> _tablesByLanguage
            = new Dictionary<LanguageCode, Dictionary<string, string>>();

        private LanguageCode _currentLanguage;
        private LocalizationConfig _config;

        /// <summary>
        /// Fired when the active language changes.
        /// </summary>
        public event Action OnLanguageChanged;

        /// <summary>
        /// Current active language.
        /// </summary>
        public LanguageCode CurrentLanguage => _currentLanguage;

        /// <summary>
        /// Initialize the manager with a single configuration asset.
        /// </summary>
        public void Initialize(LocalizationConfig config)
        {
            if (config == null)
            {
                Debug.LogError("LocalizationManager.Initialize called with null config.");
                return;
            }

            Initialize(new[] { config });
        }

        /// <summary>
        /// Initialize the manager with multiple configs. Tables are merged in order;
        /// later configs override earlier ones for the same key. Default language is taken from the first config.
        /// Use this for base + add-on configs (e.g. core UI + DLC or per-scene tables).
        /// Runtime is O(total entries) — each entry is visited once.
        /// </summary>
        public void Initialize(IReadOnlyList<LocalizationConfig> configs)
        {
            if (configs == null || configs.Count == 0)
            {
                Debug.LogError("LocalizationManager.Initialize called with null or empty config list.");
                return;
            }

            _config = null;
            _tablesByLanguage.Clear();

            foreach (var config in configs)
            {
                if (config == null) continue;

                if (_config == null)
                    _config = config;

                foreach (var table in config.Tables)
                {
                    if (!table) continue;
                    MergeTable(table);
                }
            }

            if (_config != null)
                SetLanguage(_config.DefaultLanguage);

            ServiceLocator.Instance.Register(this);
        }

        /// <summary>
        /// Merges one table's entries into the per-language dictionary. One dict lookup + one pass over entries.
        /// </summary>
        private void MergeTable(LocalizedTextTable table)
        {
            if (!_tablesByLanguage.TryGetValue(table.Language, out var dict))
            {
                dict = new Dictionary<string, string>(StringComparer.Ordinal);
                _tablesByLanguage[table.Language] = dict;
            }

            foreach (var kvp in table.GetAllEntries())
                dict[kvp.Key] = kvp.Value;
        }

        /// <summary>
        /// Change the current language.
        /// </summary>
        public void SetLanguage(LanguageCode language)
        {
            _currentLanguage = language;
            OnLanguageChanged?.Invoke();
        }

        /// <summary>
        /// Try to get a localized string for the current language.
        /// Falls back to default language when the key is missing in the current locale
        /// or when the current locale has no table loaded.
        /// </summary>
        public bool TryGet(string key, out string value)
        {
            value = null;

            if (string.IsNullOrEmpty(key))
                return false;

            // Try current language first.
            if (_tablesByLanguage.TryGetValue(_currentLanguage, out var dict) &&
                dict.TryGetValue(key, out value))
                return true;

            // Fallback to default language (missing key or missing locale).
            if (_config != null &&
                _tablesByLanguage.TryGetValue(_config.DefaultLanguage, out var fallbackDict) &&
                fallbackDict.TryGetValue(key, out value))
                return true;

            return false;
        }

        /// <summary>
        /// Get a localized string or the key itself if not found.
        /// </summary>
        public string Get(string key)
        {
            return TryGet(key, out var value) ? value : key;
        }
    }
}

