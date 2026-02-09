using ProjectUtilities.Core;
using ProjectUtilities.Options.Core;
using UnityEngine;

namespace ProjectUtilities.Options.Demo
{
    /// <summary>
    /// Bootstraps the OptionsManager for the demo scene.
    /// </summary>
    public class OptionsDemoBootstrap : MonoBehaviour
    {
        [SerializeField] private OptionsProfile _defaultProfile;

        private void Awake()
        {
            var optionsManager = new OptionsManager();
            optionsManager.Initialize(_defaultProfile, true);
            ServiceLocator.Instance.Register(optionsManager);
        }
    }
}
