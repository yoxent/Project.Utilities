using UnityEngine;
using UnityEngine.UI;
using ProjectUtilities.Core;
using ProjectUtilities.Options.Core;

namespace ProjectUtilities.Options.UI
{
    /// <summary>
    /// Simple binding between a Toggle and a bool OptionKey.
    /// </summary>
    [RequireComponent(typeof(Toggle))]
    public class OptionToggleUGUI : MonoBehaviour
    {
        [SerializeField] private OptionKey _key = OptionKey.Fullscreen;

        private Toggle _toggle;
        private OptionsManager _options;

        private void Awake()
        {
            _toggle = GetComponent<Toggle>();
            _options = ServiceLocator.Instance.Get<OptionsManager>();

            if (_options != null && _options.TryGet<bool>(_key, out var current))
            {
                _toggle.isOn = current;
            }

            _toggle.onValueChanged.AddListener(OnToggleChanged);
        }

        private void OnDestroy()
        {
            _toggle.onValueChanged.RemoveListener(OnToggleChanged);
        }

        private void OnToggleChanged(bool value)
        {
            if (_options == null)
            {
                _options = ServiceLocator.Instance.Get<OptionsManager>();
            }

            if (_options != null)
            {
                _options.Set(_key, OptionValue.FromBool(value));
            }
        }
    }
}
