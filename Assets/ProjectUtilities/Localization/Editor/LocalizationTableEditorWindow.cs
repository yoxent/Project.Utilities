using UnityEditor;
using UnityEngine;

namespace ProjectUtilities.Localization.Editor
{
    /// <summary>
    /// Minimal localization table editor for viewing and editing keys and values.
    /// </summary>
    public class LocalizationTableEditorWindow : EditorWindow
    {
        private Core.LocalizedTextTable _table;
        private Vector2 _scrollPosition;

        [MenuItem("ProjectUtilities/Localization/Table Editor")]
        public static void ShowWindow()
        {
            var window = GetWindow<LocalizationTableEditorWindow>("Localization Table");
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();
            _table = (Core.LocalizedTextTable)EditorGUILayout.ObjectField("Table", _table, typeof(Core.LocalizedTextTable), false);

            if (_table == null)
            {
                EditorGUILayout.HelpBox("Assign a LocalizedTextTable to edit.", MessageType.Info);
                return;
            }

            EditorGUILayout.LabelField($"Language: {_table.Language}", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            var so = new SerializedObject(_table);
            var entriesProperty = so.FindProperty("_entries");
            if (entriesProperty != null)
            {
                for (int i = 0; i < entriesProperty.arraySize; i++)
                {
                    var element = entriesProperty.GetArrayElementAtIndex(i);
                    var keyProp = element.FindPropertyRelative("Key");
                    var valueProp = element.FindPropertyRelative("Value");

                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    EditorGUILayout.PropertyField(keyProp);
                    EditorGUILayout.PropertyField(valueProp);

                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Remove"))
                    {
                        entriesProperty.DeleteArrayElementAtIndex(i);
                        break;
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                }

                EditorGUILayout.Space();
                if (GUILayout.Button("Add Entry"))
                {
                    entriesProperty.InsertArrayElementAtIndex(entriesProperty.arraySize);
                }
            }

            so.ApplyModifiedProperties();

            EditorGUILayout.EndScrollView();
        }
    }
}

