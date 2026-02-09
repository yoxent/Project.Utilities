using System;
using System.Collections.Generic;
using ProjectUtilities.Core;
using UnityEngine;

namespace ProjectUtilities.Options.Core
{
    /// <summary>
    /// Central options manager storing strongly-typed settings and raising change events.
    /// Initialize with an OptionsProfile and optionally load persisted values.
    /// </summary>
    public class OptionsManager
    {
        private readonly Dictionary<OptionKey, OptionValue> _values = new Dictionary<OptionKey, OptionValue>();

        /// <summary>
        /// Fired whenever an option value changes.
        /// </summary>
        public event Action<OptionKey, OptionValue> OnOptionChanged;

        /// <summary>
        /// Initialize the manager with a profile and optionally load persisted values.
        /// </summary>
        public void Initialize(OptionsProfile defaultProfile, bool loadPersisted = true)
        {
            _values.Clear();

            if (defaultProfile != null)
            {
                foreach (var entry in defaultProfile.Entries)
                {
                    _values[entry.Key] = entry.Value;
                }
            }

            if (loadPersisted)
            {
                OptionsPersistence.Load(_values);
            }

            ServiceLocator.Instance.Register(this);
        }

        public T Get<T>(OptionKey key)
        {
            return _values.TryGetValue(key, out var value) ? value.As<T>() : default;
        }

        public bool TryGet<T>(OptionKey key, out T value)
        {
            if (_values.TryGetValue(key, out var optionValue))
            {
                value = optionValue.As<T>();
                return true;
            }

            value = default;
            return false;
        }

        public void Set(OptionKey key, OptionValue value, bool applyImmediately = true)
        {
            _values[key] = value;
            if (applyImmediately)
            {
                OnOptionChanged?.Invoke(key, value);
            }
        }

        public void Save()
        {
            OptionsPersistence.Save(_values);
        }

        public IReadOnlyDictionary<OptionKey, OptionValue> GetAll() => _values;
    }
}

