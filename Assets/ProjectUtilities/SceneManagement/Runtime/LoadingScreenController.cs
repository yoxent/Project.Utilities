using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ProjectUtilities.SceneManagement
{
    /// <summary>
    /// Controls a loading screen with a progress bar, background/logo, and rotating hint text.
    /// Expects LoadingScreenContext.NextSceneName to be set before this scene is loaded.
    /// </summary>
    public class LoadingScreenController : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Image _progressFillImage;
        [SerializeField] private TextMeshProUGUI _progressText;
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Image _logoImage;
        [SerializeField] private TextMeshProUGUI _messageText;

        [Header("Behaviour")]
        [SerializeField] private float _minLoadingDuration = 2f;
        [SerializeField] private float _messageInterval = 3f;
        [SerializeField] private string[] _messages;

        private void Start()
        {
            var targetScene = LoadingScreenContext.NextSceneName;
            if (string.IsNullOrEmpty(targetScene))
            {
                Debug.LogError("LoadingScreenController: No target scene set in LoadingScreenContext.NextSceneName.");
                return;
            }

            StartCoroutine(LoadSceneCoroutine(targetScene));
        }

        private IEnumerator LoadSceneCoroutine(string targetScene)
        {
            float elapsed = 0f;
            float messageTimer = 0f;
            int messageIndex = 0;

            if (_progressFillImage != null)
            {
                _progressFillImage.fillAmount = 0f;
            }

            if (_messageText != null && _messages != null && _messages.Length > 0)
            {
                _messageText.text = _messages[0];
            }

            var op = SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Single);
            op.allowSceneActivation = false;

            while (!op.isDone)
            {
                elapsed += Time.unscaledDeltaTime;
                messageTimer += Time.unscaledDeltaTime;

                // Update progress: Unity reports up to 0.9f before activation.
                float rawProgress = Mathf.Clamp01(op.progress / 0.9f);

                // Time-based smoothing: use ~90% of the minimum duration to reach 100%.
                float durationForBar = _minLoadingDuration * 0.9f;
                float timeProgress = durationForBar > 0f
                    ? Mathf.Clamp01(elapsed / durationForBar)
                    : 1f;

                // Don't visually advance beyond the actual async progress.
                float visualProgress = Mathf.Min(rawProgress, timeProgress);

                if (_progressFillImage != null)
                {
                    _progressFillImage.fillAmount = visualProgress;
                }

                if (_progressText != null)
                {
                    _progressText.text = $"Loading... {(visualProgress * 100f):F0}%";
                }

                // Rotate hint messages.
                if (_messageText != null && _messages != null && _messages.Length > 0 && _messageInterval > 0f)
                {
                    if (messageTimer >= _messageInterval)
                    {
                        messageTimer = 0f;
                        messageIndex = (messageIndex + 1) % _messages.Length;
                        _messageText.text = _messages[messageIndex];
                    }
                }

                // When loading has reached 90% and minimum duration has passed, activate.
                if (op.progress >= 0.9f && elapsed >= _minLoadingDuration)
                {
                    op.allowSceneActivation = true;
                }

                yield return null;
            }
        }
    }
}
