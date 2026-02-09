using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ProjectUtilities.Core;
using ProjectUtilities.Options.Core;

namespace ProjectUtilities.Options.UI
{
    /// <summary>
    /// Simple binding between a Slider and a float OptionKey.
    /// </summary>
    [RequireComponent(typeof(Slider))]
    public class OptionSliderUGUI : MonoBehaviour
    {
        [SerializeField] private OptionKey _key = OptionKey.MasterVolume;
        [SerializeField] private float _minValue = 0f;
        [SerializeField] private float _maxValue = 1f;
        [SerializeField] private TextMeshProUGUI _valueLabel;

        private Slider _slider;
        private OptionsManager _options;

        private void Awake()
        {
            _slider = GetComponent<Slider>();
            _options = ServiceLocator.Instance.Get<OptionsManager>();

            _slider.minValue = _minValue;
            _slider.maxValue = _maxValue;

            if (_options != null && _options.TryGet<float>(_key, out var current))
            {
                _slider.value = current;
                UpdateLabel(current);
            }

            _slider.onValueChanged.AddListener(OnSliderChanged);
        }

        private void OnDestroy()
        {
            _slider.onValueChanged.RemoveListener(OnSliderChanged);
        }

        private void OnSliderChanged(float value)
        {
            if (_options == null)
            {
                _options = ServiceLocator.Instance.Get<OptionsManager>();
            }

            if (_options != null)
            {
                _options.Set(_key, OptionValue.FromFloat(value));
            }

            UpdateLabel(value);
        }

        private void UpdateLabel(float value)
        {
            if (_valueLabel != null)
            {
                _valueLabel.text = value.ToString("F2");
            }
        }
    }
}
