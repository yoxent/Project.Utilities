using UnityEngine;
using TMPro;
using ProjectUtilities.Localization.Core;
using ProjectUtilities.Core;

namespace ProjectUtilities.Localization
{
    /// <summary>
    /// Binds a single localization key to a TextMeshProUGUI. Add one component per text element.
    /// Scales to any number of keys: each GameObject has its own key; no central list to maintain.
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class LocalizedTextTMP : MonoBehaviour
    {
        [Tooltip("Localization key. Format: category_name (e.g. menu_play, options_volume, key_hello). Missing keys show the key as fallback.")]
        [SerializeField] private string _localizationKey = "";

        [Tooltip("Leave empty to use the TMP on this GameObject.")]
        [SerializeField] private TextMeshProUGUI _text;

        private LocalizationManager _manager;

        /// <summary>Current key. Set at runtime to change the bound key.</summary>
        public string LocalizationKey
        {
            get => _localizationKey;
            set => _localizationKey = value ?? "";
        }

        private void Awake()
        {
            if (_text == null)
                TryGetComponent(out _text);
        }

        private void OnEnable()
        {
            _manager = ServiceLocator.Instance?.Get<LocalizationManager>();
            if (_manager != null)
            {
                _manager.OnLanguageChanged += RefreshText;
                RefreshText();
            }
            else if (_text != null)
                _text.text = _localizationKey;
        }

        private void OnDisable()
        {
            if (_manager != null)
                _manager.OnLanguageChanged -= RefreshText;
        }

        /// <summary>Force refresh from current language (e.g. after changing key at runtime).</summary>
        public void RefreshText()
        {
            if (_text == null) return;

            if (_manager != null)
                _text.text = _manager.Get(_localizationKey);
            else
                _text.text = _localizationKey;
        }
    }
}
