using UnityEngine;
using UnityEngine.InputSystem;
using ProjectUtilities.Localization.Core;
using ProjectUtilities.Core;

namespace ProjectUtilities.Localization
{
    /// <summary>
    /// Testing helper: switch between English and Spanish with keyboard keys.
    /// Add to a GameObject in a scene that uses localization (e.g. demo). 1 = English, 2 = Spanish.
    /// </summary>
    public class LocalizationKeyboardSwitch : MonoBehaviour
    {
        [Tooltip("Key to set language to English.")]
        [SerializeField] private Key _keyEnglish = Key.Digit1;

        [Tooltip("Key to set language to Spanish.")]
        [SerializeField] private Key _keySpanish = Key.Digit2;

        private LocalizationManager _manager;

        private void Update()
        {
            if (Keyboard.current == null) return;

            if (_manager == null)
                _manager = ServiceLocator.Instance?.Get<LocalizationManager>();

            if (_manager == null) return;

            if (Keyboard.current[_keyEnglish].wasPressedThisFrame)
                _manager.SetLanguage(LanguageCode.English);
            else if (Keyboard.current[_keySpanish].wasPressedThisFrame)
                _manager.SetLanguage(LanguageCode.Spanish);
        }
    }
}
