using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectUtilities.Options.Core
{
    /// <summary>
    /// Simple JSON-based persistence for options, using PlayerPrefs as backing storage.
    /// </summary>
    public static class OptionsPersistence
    {
        private const string DefaultPrefsKey = "ProjectUtilities.Options";

        [Serializable]
        private class PersistedEntry
        {
            public OptionKey Key;
            public OptionValueType Type;
            public bool BoolValue;
            public int IntValue;
            public float FloatValue;
            public string StringValue;
        }

        [Serializable]
        private class PersistedData
        {
            public List<PersistedEntry> Entries = new List<PersistedEntry>();
            public int Version = 1;
        }

        public static void Save(IReadOnlyDictionary<OptionKey, OptionValue> values, string prefsKey = DefaultPrefsKey)
        {
            var data = new PersistedData();

            foreach (var pair in values)
            {
                var entry = new PersistedEntry
                {
                    Key = pair.Key,
                    Type = pair.Value.Type,
                    BoolValue = pair.Value.BoolValue,
                    IntValue = pair.Value.IntValue,
                    FloatValue = pair.Value.FloatValue,
                    StringValue = pair.Value.StringValue
                };

                data.Entries.Add(entry);
            }

            var json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString(prefsKey, json);
            PlayerPrefs.Save();
        }

        public static void Load(IDictionary<OptionKey, OptionValue> target, string prefsKey = DefaultPrefsKey)
        {
            if (!PlayerPrefs.HasKey(prefsKey))
            {
                return;
            }

            var json = PlayerPrefs.GetString(prefsKey, string.Empty);
            if (string.IsNullOrEmpty(json))
            {
                return;
            }

            var data = JsonUtility.FromJson<PersistedData>(json);
            if (data?.Entries == null)
            {
                return;
            }

            foreach (var entry in data.Entries)
            {
                var value = new OptionValue
                {
                    Type = entry.Type,
                    BoolValue = entry.BoolValue,
                    IntValue = entry.IntValue,
                    FloatValue = entry.FloatValue,
                    StringValue = entry.StringValue
                };

                target[entry.Key] = value;
            }
        }
    }
}

