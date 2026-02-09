using UnityEditor;
using UnityEngine;
using ProjectUtilities.NumberFormatting;

namespace ProjectUtilities.NumberFormatting.Editor
{
    [CustomEditor(typeof(NumberFormattingConfig))]
    public class NumberFormattingConfigEditor : UnityEditor.Editor
    {
        private double _previewValue = 1234567.89;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawDefaultInspector();

            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);
            _previewValue = EditorGUILayout.DoubleField("Value", _previewValue);
            var config = (NumberFormattingConfig)target;
            string formatted = NumberFormatter.Format(_previewValue, config);
            EditorGUILayout.HelpBox($"Formatted: {formatted}", MessageType.None);
            EditorGUILayout.LabelField("Long sample: " + NumberFormatter.Format(12345678901234, config));

            serializedObject.ApplyModifiedProperties();
        }
    }
}
