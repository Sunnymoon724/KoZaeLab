#if UNITY_EDITOR
using KZLib.KZWindow;
using UnityEditor;
using UnityEngine;

namespace KZLib.KZMenu
{
	public static partial class AssetsMenuItem
	{
		#region Open Texture
		[MenuItem("Assets/KZSubMenu/Texture/Open Texture",false,AssetsCategory.Texture)]
		private static void OnOpenTexture()
		{
			var data = Selection.activeObject as Texture2D;
			var viewer = EditorWindow.GetWindow<TextureWindow>("Viewer");

			viewer.SetTexture(data);
			viewer.Show();
		}

		[MenuItem("Assets/KZSubMenu/Texture/Open Texture",true)]
		private static bool IsOpenAbleTexture()
		{
			return Selection.activeObject as Texture2D;
		}
		#endregion Open Texture
	}
}	
#endif