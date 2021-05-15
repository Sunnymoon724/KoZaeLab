using UnityEngine;
using UnityEditor;

namespace Helpers.HierarchyIcons
{
    [InitializeOnLoad]
    public class HierarchyIconOnHierarchyView
    {
        const float k_MaxIconSize = 16;

        static HierarchyIconOnHierarchyView()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemCallback;
        }

        static void HierarchyWindowItemCallback(int instanceID, Rect selectionRect)
        {
            GameObject gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

            if (!gameObject) return;

            HierarchyIcon[] hierarchyIcons = gameObject.GetComponents<HierarchyIcon>();
            foreach (var hierarchyIcon in hierarchyIcons)
            {
                Texture2D icon = hierarchyIcon.icon ? hierarchyIcon.icon : TextureHelper.NoIcon;
                float width = Mathf.Min(icon.width, k_MaxIconSize);
                float height = Mathf.Min(icon.height, k_MaxIconSize);

                float x = selectionRect.x + selectionRect.width - width - hierarchyIcon.position * k_MaxIconSize - 1;
                Rect rect = new Rect(x, selectionRect.y, width, height);

                // draw the icon
                GUI.DrawTexture(rect, icon);

                // set link cursor
                EditorGUIUtility.AddCursorRect(rect, MouseCursor.Link);

                // and draw a button for changue the icon and display the tooltip
                if (GUI.Button(rect, new GUIContent(string.Empty, hierarchyIcon.tooltip), EditorStyles.label))
                {
                    PopupWindow.Show(rect, new PickIconWindow(hierarchyIcon));
                }

            }
        }
    }
}