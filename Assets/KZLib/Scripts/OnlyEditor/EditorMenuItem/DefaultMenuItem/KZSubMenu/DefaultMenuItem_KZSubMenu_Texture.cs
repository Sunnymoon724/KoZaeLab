#if UNITY_EDITOR
using KZLib.Windows;
using UnityEditor;
using UnityEngine;

namespace KZLib.EditorInternal.Menus
{
	/// <summary>
	/// <c>Assets/KZSubMenu/Texture</c> menu entries.
	/// </summary>
	public static partial class DefaultMenuItem
	{
		#region Texture
		[MenuItem("Assets/KZSubMenu/Texture/Open Texture",false,MenuOrder.KZSubMenu.TEXTURE)]
		private static void _OnOpenTexture()
		{
			var viewer = EditorWindow.GetWindow<TextureWindow>("Viewer");

			viewer.SetResource(Selection.activeObject as Texture2D);
			viewer.Show();
		}

		[MenuItem("Assets/KZSubMenu/Texture/Open Texture",true,MenuOrder.KZSubMenu.TEXTURE)]
		private static bool _CanOpenTexture()
		{
			return _IsTexture2D();
		}
		#endregion Texture
	}
}
#endif