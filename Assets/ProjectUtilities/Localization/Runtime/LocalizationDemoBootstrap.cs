using System.Collections.Generic;
using UnityEngine;
using ProjectUtilities.Localization.Core;
using ProjectUtilities.Core;

namespace ProjectUtilities.Localization
{
    /// <summary>
    /// Initializes LocalizationManager with the assigned config list and registers it with ServiceLocator.
    /// One config = list of one; multiple configs are merged in order (later override earlier for same key).
    /// Place on a GameObject in the first scene that uses localization (e.g. Localization_Demo).
    /// </summary>
    public class LocalizationDemoBootstrap : MonoBehaviour
    {
        [Tooltip("Configs to load. One entry = single config; multiple entries = merged (later override earlier for same key).")]
        [SerializeField] private List<LocalizationConfig> _configs = new List<LocalizationConfig>();

        private void Awake()
        {
            if (_configs == null || _configs.Count == 0)
            {
                Debug.LogWarning("LocalizationDemoBootstrap: no configs assigned. Add at least one LocalizationConfig to Configs.");
                return;
            }

            var trimmed = new List<LocalizationConfig>();
            foreach (var c in _configs)
            {
                if (c != null) trimmed.Add(c);
            }

            if (trimmed.Count == 0)
            {
                Debug.LogWarning("LocalizationDemoBootstrap: all configs are null.");
                return;
            }

            var manager = new LocalizationManager();
            manager.Initialize(trimmed);
        }
    }
}
