using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

namespace WATP.UI.Editor
{
    [CustomEditor(typeof(Tab), true)]
    public class TabEditor : ButtonEditor
    {
        SerializedProperty m_tabMenu;
        SerializedProperty m_targetObject;

        protected override void OnEnable()
        {
            m_tabMenu = serializedObject.FindProperty("tabMenu");
            m_targetObject = serializedObject.FindProperty("targetObject");

            base.OnEnable();
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_tabMenu);
            EditorGUILayout.PropertyField(m_targetObject);
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
        }

    }
}