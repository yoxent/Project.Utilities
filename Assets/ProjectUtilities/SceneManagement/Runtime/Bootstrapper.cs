using ProjectUtilities.Core;
using ProjectUtilities.Localization.Core;
using ProjectUtilities.Options.Core;
using ProjectUtilities.Pooling.Core;
using UnityEngine;

namespace ProjectUtilities.SceneManagement
{
    /// <summary>
    /// Ensures core managers are initialized once and persist across scenes.
    /// Place this in a boot scene.
    /// </summary>
    public class Bootstrapper : MonoBehaviour
    {
        [Header("Localization")]
        [SerializeField] private LocalizationConfig _localizationConfig;

        [Header("Options")]
        [SerializeField] private OptionsProfile _defaultOptionsProfile;

        [Header("Pooling")]
        [SerializeField] private PoolGroupConfig[] _poolGroups;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            InitializeLocalization();
            InitializeOptions();
            InitializePooling();
        }

        private void InitializeLocalization()
        {
            if (_localizationConfig == null)
            {
                return;
            }

            var manager = new LocalizationManager();
            manager.Initialize(_localizationConfig);
        }

        private void InitializeOptions()
        {
            var manager = new OptionsManager();
            manager.Initialize(_defaultOptionsProfile, true);
        }

        private void InitializePooling()
        {
            if (_poolGroups == null || _poolGroups.Length == 0)
            {
                return;
            }

            var root = new GameObject("PoolRoot").transform;
            DontDestroyOnLoad(root.gameObject);

            var poolManager = new PoolManager(root);
            poolManager.Initialize(_poolGroups);
        }
    }
}

