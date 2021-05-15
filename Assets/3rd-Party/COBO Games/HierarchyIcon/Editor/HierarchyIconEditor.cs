using UnityEngine;
using UnityEditor;

namespace Helpers.HierarchyIcons
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(HierarchyIcon))]
    public class HierarchyIconEditor : Editor
    {
        SerializedProperty m_iconProperty;
        SerializedProperty m_tooltipProperty;
        SerializedProperty m_positionProperty;

        const float k_IconButtonSize = 28;

        void OnEnable()
        {
            m_iconProperty = serializedObject.FindProperty("icon");
            m_tooltipProperty = serializedObject.FindProperty("tooltip");
            m_positionProperty = serializedObject.FindProperty("position");
        }

        public override void OnInspectorGUI()
        {
            // draw the script header
            {
                GUI.enabled = false;
                DrawPropertiesExcluding(serializedObject, m_iconProperty.name, m_tooltipProperty.name, m_positionProperty.name);
                GUI.enabled = true;
            }

            // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
            serializedObject.Update();

            // draw the pick icon button
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.PrefixLabel("Icon");

                HierarchyIcon hierarchyIcon = (HierarchyIcon)target;
                Rect rectButton = EditorGUILayout.GetControlRect(GUILayout.Width(k_IconButtonSize), GUILayout.Height(k_IconButtonSize));

                if (!m_iconProperty.hasMultipleDifferentValues)
                {
                    if (GUI.Button(rectButton, hierarchyIcon.icon))
                    {
                        PopupWindow.Show(rectButton, new PickIconWindow(m_iconProperty));
                    }
                }
                else
                {
                    if (GUI.Button(rectButton, "_"))
                    {
                        PopupWindow.Show(rectButton, new PickIconWindow(m_iconProperty));
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            // draw the position
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(m_positionProperty);
                if (EditorGUI.EndChangeCheck())
                {
                    // repaint the hierarchy
                    EditorApplication.RepaintHierarchyWindow();
                }
            }

            // draw the tooltip
            EditorGUILayout.PropertyField(m_tooltipProperty);


            // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
            serializedObject.ApplyModifiedProperties();
        }

    }
}