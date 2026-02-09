using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectUtilities.Localization.Core
{
    /// <summary>
    /// Key/value string table for one language. One asset per language; assign to a LocalizationConfig.
    /// </summary>
    [CreateAssetMenu(fileName = "LocalizedTextTable", menuName = "ProjectUtilities/Localization/TextTable")]
    public class LocalizedTextTable : ScriptableObject
    {
        [Serializable]
        public struct Entry
        {
            public string Key;
            [TextArea]
            public string Value;
        }

        [SerializeField] private LanguageCode _language = LanguageCode.English;
        [SerializeField] private List<Entry> _entries = new List<Entry>();

        private Dictionary<string, string> _lookup;

        public LanguageCode Language => _language;

        private void OnEnable()
        {
            BuildLookup();
        }

        private void OnValidate()
        {
            BuildLookup();
        }

        private void BuildLookup()
        {
            _lookup = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (var entry in _entries)
            {
                if (string.IsNullOrEmpty(entry.Key))
                {
                    continue;
                }

                _lookup[entry.Key] = entry.Value ?? string.Empty;
            }
        }

        public bool TryGet(string key, out string value)
        {
            if (_lookup == null)
            {
                BuildLookup();
            }

            return _lookup.TryGetValue(key, out value);
        }

        /// <summary>
        /// Returns all key/value entries for merging into the localization manager. Used by LocalizationManager only.
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> GetAllEntries()
        {
            EnsureLookup();
            foreach (var kvp in _lookup)
                yield return kvp;
        }

        private void EnsureLookup()
        {
            if (_lookup == null)
                BuildLookup();
        }
    }
}

