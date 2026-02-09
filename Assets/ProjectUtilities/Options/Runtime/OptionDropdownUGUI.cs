using TMPro;
using UnityEngine;
using ProjectUtilities.Core;
using ProjectUtilities.Options.Core;

namespace ProjectUtilities.Options.UI
{
    /// <summary>
    /// Simple binding between a TMP_Dropdown and an int OptionKey (e.g., quality level).
    /// </summary>
    [RequireComponent(typeof(TMP_Dropdown))]
    public class OptionDropdownUGUI : MonoBehaviour
    {
        [SerializeField] private OptionKey _key = OptionKey.GraphicsQuality;

        private TMP_Dropdown _dropdown;
        private OptionsManager _options;

        private void Awake()
        {
            _dropdown = GetComponent<TMP_Dropdown>();
            _options = ServiceLocator.Instance.Get<OptionsManager>();

            if (_options != null && _options.TryGet<int>(_key, out var current))
            {
                _dropdown.value = current;
            }

            _dropdown.onValueChanged.AddListener(OnDropdownChanged);
        }

        private void OnDestroy()
        {
            _dropdown.onValueChanged.RemoveListener(OnDropdownChanged);
        }

        private void OnDropdownChanged(int value)
        {
            if (_options == null)
            {
                _options = ServiceLocator.Instance.Get<OptionsManager>();
            }

            if (_options != null)
            {
                _options.Set(_key, OptionValue.FromInt(value));
            }
        }
    }
}
