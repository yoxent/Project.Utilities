using UnityEditor;
using UnityEngine;

namespace ProjectUtilities.Options.Editor
{
    /// <summary>
    /// Custom inspector for OptionsProfile to make entries easier to edit.
    /// </summary>
    [CustomEditor(typeof(Core.OptionsProfile))]
    public class OptionsProfileEditor : UnityEditor.Editor
    {
        private SerializedProperty _entriesProperty;

        private void OnEnable()
        {
            _entriesProperty = serializedObject.FindProperty("_entries");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Options Profile", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            if (_entriesProperty != null)
            {
                for (int i = 0; i < _entriesProperty.arraySize; i++)
                {
                    var element = _entriesProperty.GetArrayElementAtIndex(i);
                    var keyProp = element.FindPropertyRelative("Key");
                    var valueProp = element.FindPropertyRelative("Value");

                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    EditorGUILayout.PropertyField(keyProp);

                    EditorGUILayout.PropertyField(valueProp.FindPropertyRelative("Type"));

                    var type = (Core.OptionValueType)valueProp.FindPropertyRelative("Type").enumValueIndex;

                    switch (type)
                    {
                        case Core.OptionValueType.Bool:
                            EditorGUILayout.PropertyField(valueProp.FindPropertyRelative("BoolValue"), new GUIContent("Value"));
                            break;
                        case Core.OptionValueType.Int:
                            EditorGUILayout.PropertyField(valueProp.FindPropertyRelative("IntValue"), new GUIContent("Value"));
                            break;
                        case Core.OptionValueType.Float:
                            EditorGUILayout.PropertyField(valueProp.FindPropertyRelative("FloatValue"), new GUIContent("Value"));
                            break;
                        case Core.OptionValueType.String:
                            EditorGUILayout.PropertyField(valueProp.FindPropertyRelative("StringValue"), new GUIContent("Value"));
                            break;
                    }

                    if (GUILayout.Button("Remove"))
                    {
                        _entriesProperty.DeleteArrayElementAtIndex(i);
                        break;
                    }

                    EditorGUILayout.EndVertical();
                }

                EditorGUILayout.Space();
                if (GUILayout.Button("Add Option"))
                {
                    _entriesProperty.InsertArrayElementAtIndex(_entriesProperty.arraySize);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}

