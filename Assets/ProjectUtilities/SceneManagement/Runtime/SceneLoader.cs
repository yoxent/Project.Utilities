using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ProjectUtilities.SceneManagement
{
    /// <summary>
    /// Simple wrapper around Unity's async scene loading APIs.
    /// </summary>
    public class SceneLoader : MonoBehaviour
    {
        public static event Action<string> OnBeforeSceneLoad;
        public static event Action<string> OnAfterSceneLoad;

        private static SceneLoader _instance;

        public static SceneLoader Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }

                var existing = FindFirstObjectByType<SceneLoader>();
                if (existing != null)
                {
                    _instance = existing;
                    return _instance;
                }

                var go = new GameObject("SceneLoader");
                _instance = go.AddComponent<SceneLoader>();
                DontDestroyOnLoad(go);
                return _instance;
            }
        }

        public void LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            StartCoroutine(LoadSceneCoroutine(sceneName, mode));
        }

        private IEnumerator LoadSceneCoroutine(string sceneName, LoadSceneMode mode)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                yield break;
            }

            OnBeforeSceneLoad?.Invoke(sceneName);

            var op = SceneManager.LoadSceneAsync(sceneName, mode);
            if (op == null)
            {
                yield break;
            }

            while (!op.isDone)
            {
                yield return null;
            }

            OnAfterSceneLoad?.Invoke(sceneName);
        }
    }
}

