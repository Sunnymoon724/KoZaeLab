using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;

namespace Helpers.HierarchyIcons
{
    internal class PickIconWindow : PopupWindowContent
    {
        SerializedProperty m_iconProperty;
        Vector2 m_scrollPosition;
        int m_myUndoGroup;

        static GUIContent[] s_IconContents;
        static GUIStyle s_ButtonStyle;
        static Texture2D s_NoneIcon = TextureHelper.Load("icons/sv_icon_none.png");
        static string s_searchWord = "";
        static bool s_loadAllIcons;

        const int k_ButtonStylePadding = 2;
        const float k_ButtonSize = 16 + k_ButtonStylePadding * 2;
        const float k_Columns = 12;
        const int k_HeaderHeight = 22;
        const int k_BodyHeight = 200;
        const int k_BottomHeight = 26;

        static PickIconWindow()
        {
            s_ButtonStyle = new GUIStyle();
            s_ButtonStyle.fixedWidth = k_ButtonSize;
            s_ButtonStyle.fixedHeight = k_ButtonSize;
            s_ButtonStyle.alignment = TextAnchor.MiddleCenter;
            s_ButtonStyle.onNormal.background = TextureHelper.ButtonOn;
            s_ButtonStyle.padding = new RectOffset(k_ButtonStylePadding, k_ButtonStylePadding, k_ButtonStylePadding, k_ButtonStylePadding);
        }

        public PickIconWindow(HierarchyIcon hierarchyIcon) : this(new SerializedObject(hierarchyIcon).FindProperty("icon"))
        {
            if(m_iconProperty == null)
            {
                Debug.LogError("'icon' property not found in the HierachyIcon script.");
            }
        }

        public PickIconWindow(SerializedProperty target)
        {
            LoadIcons(false);

            m_iconProperty = target;
            m_myUndoGroup = -1;
        }

        public override void OnGUI(Rect rect)
        {
            if (m_iconProperty == null) return;

            // body - scroll view with a selection grid
            {
                float rows = Mathf.Ceil(s_IconContents.Length / k_Columns);
                float height = rows * k_ButtonSize;

                // calculate rects
                Rect rectScrollViewPosition = rect;
                rectScrollViewPosition.y = k_HeaderHeight;
                rectScrollViewPosition.height -= k_HeaderHeight + k_BottomHeight;

                Rect rectScrollView = new Rect(0, 0, rect.width - k_ButtonSize, height);

                m_scrollPosition = GUI.BeginScrollView(rectScrollViewPosition, m_scrollPosition, rectScrollView, false, true);
                {
                    float x = 3;
                    Rect rectButtons = new Rect(x, 1, k_ButtonSize, k_ButtonSize);
                    int counter = 0; // used in the case we are searching
                    for (int i = 0; i < s_IconContents.Length; i++)
                    {
                        GUIContent content = s_IconContents[i];

                        if (!ContainsSearchWord(content.tooltip))
                        {
                            continue;
                        }

                        bool selected = (content.image == m_iconProperty.objectReferenceValue);
                        EditorGUI.BeginChangeCheck();
                        GUI.Toggle(rectButtons, selected, content, s_ButtonStyle);
                        if (EditorGUI.EndChangeCheck())
                        {
                            AssignIconAndRecordUndo(i);
                        }

                        rectButtons.x += k_ButtonSize;

                        if ((counter + 1) % k_Columns == 0)
                        {
                            rectButtons.y += k_ButtonSize;
                            rectButtons.x = x;
                        }

                        counter++;
                    }
                }
                GUI.EndScrollView();
            }

            // header
            {
                // search
                {
                    GUI.Label(new Rect(3, 3, 50, 17), "Search:");
                    Rect rectCloseSearch = new Rect(167, 4, 16, 16);

                    // the button for clicking
                    if (GUI.Button(rectCloseSearch, "", EditorStyles.label))
                    {
                        s_searchWord = "";
                    }

                    s_searchWord = GUI.TextField(new Rect(53, 3, 130, 17), s_searchWord, EditorStyles.helpBox);

                    // the button graphic on top of the text field
                    if (s_searchWord != "")
                    {
                        GUI.Label(rectCloseSearch, new GUIContent(TextureHelper.ClearSearch, "Clear Search"));
                    }
                }

                // none button
                {
                    if (!m_iconProperty.objectReferenceValue) // disable the button if no icon
                        GUI.enabled = false;

                    GUIContent noneIconContent = new GUIContent("None", s_NoneIcon, "Remove Icon");
                    float width = EditorStyles.label.CalcSize(noneIconContent).x + 5;
                    if (GUI.Button(new Rect(rect.width - width, 4, width, k_HeaderHeight), noneIconContent, EditorStyles.label))
                    {
                        AssignIconAndRecordUndo(-1);
                        //editorWindow.Close();
                    }
                    GUI.enabled = true;
                }

                // horizontal line
                DrawLineH(0, k_HeaderHeight, rect.width, Color.gray);
            }

            Event e = Event.current;

            // other icon
            {
                // horizontal line
                DrawLineH(0, rect.height - k_BottomHeight, rect.width, Color.gray);

                float marginLeftRight = 50;
                float marginTopBottom = 3;
                Rect rectButton = new Rect(
                    marginLeftRight,
                    rect.height - k_BottomHeight + marginTopBottom,
                    rect.width - marginLeftRight * 2,
                    k_BottomHeight - marginTopBottom * 2);

                int controlID = GUIUtility.GetControlID("rectButtonOtherIcon***".GetHashCode(), FocusType.Keyboard, rectButton);
                if (GUI.Button(rectButton, "Other..."))
                {
                    GUIUtility.keyboardControl = controlID;
                    EditorGUIUtility.ShowObjectPicker<Texture2D>(m_iconProperty.objectReferenceValue, false, string.Empty, controlID);
                }

                // handle object picker commands
                if (e.type == EventType.ExecuteCommand)
                {
                    string commandName = e.commandName;
                    if (EditorGUIUtility.GetObjectPickerControlID() == controlID && GUIUtility.keyboardControl == controlID)
                    {
                        GUI.changed = true;
                        e.Use();

                        if (commandName == "ObjectSelectorUpdated")
                        {
                            Texture2D icon = EditorGUIUtility.GetObjectPickerObject() as Texture2D;
                            AssignIconAndRecordUndo(icon);
                        }
                    }
                }

                // toggle load all icons
                {
                    float width = 40, height = 17;
                    Rect r = new Rect(rect.width - width, rect.height - height - 3, width, height);
                    EditorGUI.BeginChangeCheck();
                    s_loadAllIcons = GUI.Toggle(r, s_loadAllIcons, new GUIContent("All", "Show all unity textures. Including yours in the project view."));
                    if (EditorGUI.EndChangeCheck())
                    {
                        LoadIcons(true);
                    }
                }
            }


            // cancel with escape
            {
                if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape)
                {
                    editorWindow.Close();
                }
            }
        }

        static bool ContainsSearchWord(string name)
        {
            string searchWordLower = s_searchWord.Trim().ToLower();

            if (name.ToLower().Contains(searchWordLower))
                return true;

            return false;
        }

        public static void DrawRect(Rect r, Color c)
        {
            GUI.color = c;
            GUI.DrawTexture(r, EditorGUIUtility.whiteTexture);
            GUI.color = Color.white;
        }

        public static void DrawLineH(float x, float y, float width, Color c)
        {
            DrawRect(new Rect(x, y, width, 1), c);
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(
                k_ButtonSize * k_Columns + k_ButtonSize,
                k_HeaderHeight + k_BodyHeight + k_BottomHeight
                );
        }

        void AssignIconAndRecordUndo(Texture2D newIcon)
        {
            if (m_iconProperty.objectReferenceValue != newIcon)
            {
                if (m_myUndoGroup == -1)
                {
                    Undo.IncrementCurrentGroup();
                    m_myUndoGroup = Undo.GetCurrentGroup();
                }

                m_iconProperty.objectReferenceValue = newIcon;
                m_iconProperty.serializedObject.ApplyModifiedProperties();

                // repaint the hierarchy
                EditorApplication.RepaintHierarchyWindow();
            }
        }

        void AssignIconAndRecordUndo(int newBuiltinIconIndex)
        {
            AssignIconAndRecordUndo(GetIcon(newBuiltinIconIndex));
        }

        public override void OnClose()
        {
            Undo.CollapseUndoOperations(m_myUndoGroup);
        }

        public static Texture2D GetIcon(int index)
        {
            LoadIcons(false);

            if (index >= 0 && index < s_IconContents.Length)
                return (Texture2D)s_IconContents[index].image;

            return null;
        }

        static void LoadIcons(bool showAllChanged)
        {
            if (s_IconContents == null || showAllChanged)
            {
                LoadAssetPreviews();

                List<GUIContent> listIcons = new List<GUIContent>();

                // find all unity textures
                Texture[] textures = Resources.FindObjectsOfTypeAll<Texture>();
                Array.Sort(textures, (pA, pB) => String.Compare(pA.name, pB.name, StringComparison.OrdinalIgnoreCase));

                string icon = "icon";
                foreach (var texture in textures)
                {
                    string name = texture.name;
                    if (name == string.Empty) continue;

                    if (!s_loadAllIcons && !name.ToLower().Contains(icon)) continue;

                    listIcons.Add(new GUIContent(texture, name));
                }

                s_IconContents = listIcons.ToArray();
            }
        }

        /// <summary>
        /// Load all assets preview so that we can find it
        /// </summary>
        static void LoadAssetPreviews()
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (!assembly.FullName.StartsWith("UnityEngine")) // load only unity engine assemblies
                    continue;

                foreach (Type type in assembly.GetTypes())
                {
                    AssetPreview.GetMiniTypeThumbnail(type);
                }
            }
        }
    }
}
