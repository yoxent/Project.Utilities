using System.Collections.Generic;
using UnityEngine;

namespace ProjectUtilities.Localization.Core
{
    /// <summary>
    /// Configuration for the localization system: default language and a list of string tables.
    /// Each table is a ScriptableObject per language; config references all tables used at runtime.
    /// </summary>
    [CreateAssetMenu(fileName = "LocalizationConfig", menuName = "ProjectUtilities/Localization/Config")]
    public class LocalizationConfig : ScriptableObject
    {
        [SerializeField] private LanguageCode _defaultLanguage = LanguageCode.English;
        [SerializeField] private List<LocalizedTextTable> _tables = new List<LocalizedTextTable>();

        public LanguageCode DefaultLanguage => _defaultLanguage;
        public IReadOnlyList<LocalizedTextTable> Tables => _tables;
    }
}

