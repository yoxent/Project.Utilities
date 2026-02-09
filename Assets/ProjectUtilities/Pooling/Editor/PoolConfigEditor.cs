using UnityEditor;
using UnityEngine;

namespace ProjectUtilities.Pooling.Editor
{
    /// <summary>
    /// Custom inspector for PoolConfig for quick validation.
    /// </summary>
    [CustomEditor(typeof(Core.PoolConfig))]
    public class PoolConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var poolIdProp = serializedObject.FindProperty("_poolId");
            var prefabProp = serializedObject.FindProperty("_prefab");
            var initialSizeProp = serializedObject.FindProperty("_initialSize");
            var maxSizeProp = serializedObject.FindProperty("_maxSize");
            var autoExpandProp = serializedObject.FindProperty("_autoExpand");

            EditorGUILayout.PropertyField(poolIdProp);
            EditorGUILayout.PropertyField(prefabProp);
            EditorGUILayout.PropertyField(initialSizeProp);
            EditorGUILayout.PropertyField(maxSizeProp);
            EditorGUILayout.PropertyField(autoExpandProp);

            if (prefabProp.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox("Assign a prefab that implements IPoolable.", MessageType.Warning);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}

