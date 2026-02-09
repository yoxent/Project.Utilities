using System.IO;
using ProjectUtilities.Localization.Core;
using ProjectUtilities.Options.Core;
using ProjectUtilities.Pooling.Core;
using ProjectUtilities.SceneManagement;
using UnityEditor;
using UnityEngine;

namespace ProjectUtilities.Editor
{
    /// <summary>
    /// Lightweight setup wizard that can create common ScriptableObjects and a Bootstrapper prefab.
    /// </summary>
    public class SetupWizard : EditorWindow
    {
        private const string RootPath = "Assets/ProjectUtilities";

        [MenuItem("ProjectUtilities/Setup Wizard")]
        public static void ShowWindow()
        {
            var window = GetWindow<SetupWizard>("ProjectUtilities Setup");
            window.minSize = new Vector2(320, 180);
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("ProjectUtilities Setup Wizard", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            if (GUILayout.Button("Create Default Assets (Localization, Options, Pooling)"))
            {
                CreateDefaultAssets();
            }

            if (GUILayout.Button("Create Bootstrapper Prefab"))
            {
                CreateBootstrapperPrefab();
            }
        }

        private static void CreateDefaultAssets()
        {
            Directory.CreateDirectory(RootPath);

            // Localization config
            var locConfig = ScriptableObject.CreateInstance<LocalizationConfig>();
            SaveAssetIfNotExists(locConfig, $"{RootPath}/Localization/LocalizationConfig.asset");

            // Options profile
            var optionsProfile = ScriptableObject.CreateInstance<OptionsProfile>();
            SaveAssetIfNotExists(optionsProfile, $"{RootPath}/Options/DefaultOptionsProfile.asset");

            // Pool group config
            var poolGroup = ScriptableObject.CreateInstance<PoolGroupConfig>();
            SaveAssetIfNotExists(poolGroup, $"{RootPath}/Pooling/DefaultPoolGroup.asset");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void CreateBootstrapperPrefab()
        {
            Directory.CreateDirectory($"{RootPath}/SceneManagement");

            var go = new GameObject("Bootstrapper");
            var bootstrapper = go.AddComponent<Bootstrapper>();

            var prefabPath = $"{RootPath}/SceneManagement/Bootstrapper.prefab";
            var prefab = PrefabUtility.SaveAsPrefabAsset(go, prefabPath);

            Object.DestroyImmediate(go);

            Selection.activeObject = prefab;
        }

        private static void SaveAssetIfNotExists(ScriptableObject asset, string path)
        {
            var existing = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
            if (existing != null)
            {
                Object.DestroyImmediate(asset);
                return;
            }

            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            AssetDatabase.CreateAsset(asset, path);
        }
    }
}

