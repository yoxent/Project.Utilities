using UnityEngine;

namespace ProjectUtilities.SceneManagement
{
    /// <summary>
    /// Simple controller with methods for UI buttons to load demo scenes.
    /// </summary>
    public class SceneLoaderDemoController : MonoBehaviour
    {
        [SerializeField] private string _menuSceneName = "SM_Menu";
        [SerializeField] private string _level1SceneName = "SM_Level1";
        [SerializeField] private string _level2SceneName = "SM_Level2";
        [SerializeField] private string _loadingSceneName = "SM_Loading";

        public void LoadMenu()
        {
            SceneLoader.Instance.LoadScene(_menuSceneName);
        }

        public void LoadLevel1()
        {
            LoadingScreenContext.NextSceneName = _level1SceneName;
            SceneLoader.Instance.LoadScene(_loadingSceneName);
        }

        public void LoadLevel2()
        {
            LoadingScreenContext.NextSceneName = _level2SceneName;
            SceneLoader.Instance.LoadScene(_loadingSceneName);
        }
    }
}
