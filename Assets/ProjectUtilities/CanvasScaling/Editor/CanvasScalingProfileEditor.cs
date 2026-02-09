using UnityEditor;
using UnityEngine;
using ProjectUtilities.CanvasScaling.Core;

namespace ProjectUtilities.CanvasScaling.Editor
{
    [CustomEditor(typeof(CanvasScalingProfile))]
    public class CanvasScalingProfileEditor : UnityEditor.Editor
    {
        private const string DefaultLabel = "Default";
        private const string PhoneLabel = "Phone";
        private const string TabletLabel = "Tablet";
        private const string DesktopLabel = "Desktop";

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawRuleBlock("_defaultRule", DefaultLabel);
            DrawRuleBlock("_phoneRule", PhoneLabel);
            DrawRuleBlock("_tabletRule", TabletLabel);
            DrawRuleBlock("_desktopRule", DesktopLabel);

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawRuleBlock(string rulePropPath, string label)
        {
            var rule = serializedObject.FindProperty(rulePropPath);
            if (rule == null) return;

            rule.isExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(rule.isExpanded, label);
            if (rule.isExpanded)
            {
                EditorGUI.indentLevel++;
                var refRes = rule.FindPropertyRelative("_referenceResolution");
                var match = rule.FindPropertyRelative("_matchWidthOrHeight");
                var scaleMode = rule.FindPropertyRelative("_scaleMode");
                if (refRes != null) EditorGUILayout.PropertyField(refRes);
                if (match != null) EditorGUILayout.PropertyField(match);
                if (scaleMode != null) EditorGUILayout.PropertyField(scaleMode);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
    }
}
