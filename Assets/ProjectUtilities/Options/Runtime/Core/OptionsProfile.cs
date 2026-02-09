using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectUtilities.Options.Core
{
    /// <summary>
    /// ScriptableObject defining default values for a set of options.
    /// </summary>
    [CreateAssetMenu(fileName = "OptionsProfile", menuName = "ProjectUtilities/Options/Profile")]
    public class OptionsProfile : ScriptableObject
    {
        [Serializable]
        public struct Entry
        {
            public OptionKey Key;
            public OptionValue Value;
        }

        [SerializeField] private List<Entry> _entries = new List<Entry>();

        public IReadOnlyList<Entry> Entries => _entries;
    }
}

