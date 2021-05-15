using UnityEngine;
using UnityEditor;

namespace Helpers.HierarchyIcons
{
    public class TextureHelper
    {
        public static Texture2D NoIcon = Load("icons/buildsettings.standalonebroadcom.small.png");

        public static Texture2D ButtonOn = Load(ButtonOnBytes);
        public static Texture2D ClearSearch = Load("icons/winbtn_win_close_h.png");

        static byte[] ButtonOnBytes
        {
            get
            {
                return new byte[] { 137, 80, 78, 71, 13, 10, 26, 10, 0, 0, 0, 13, 73, 72, 68, 82, 0, 0, 0, 2, 0, 0, 0, 2, 8, 2, 0, 0, 0, 253, 212, 154, 115, 0, 0, 0, 19, 73, 68, 65, 84, 8, 29, 99, 180, 171, 125, 206, 192, 192, 192, 4, 196, 64, 0, 0, 19, 25, 1, 166, 188, 203, 191, 138, 0, 0, 0, 0, 73, 69, 78, 68, 174, 66, 96, 130 };
            }
        }



        public static Texture2D Load(string path)
        {
            Texture2D tex = EditorGUIUtility.Load(path) as Texture2D;
            if (tex)
            {
                tex.hideFlags = HideFlags.HideAndDontSave;
            }
            else
            {
                Debug.Log("Texture at path: '" + path + "' not found");
            }
            return tex;
        }

        public static Texture2D Load(byte[] bytes)
        {
            Texture2D texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            texture.hideFlags = HideFlags.HideAndDontSave;
            texture.LoadImage(bytes, true);

            return texture;
        }
    }
}