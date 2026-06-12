#if UNITY_EDITOR
using UnityEngine;
using Sirenix.OdinInspector;
using KZLib.Windows;
using UnityEditor;

namespace KZLib.Attributes
{
	public class KZTexturePathAttributeDrawer : KZResourcePathAttributeDrawer<KZTexturePathAttribute>
	{
		protected override SdfIconType IconType => SdfIconType.Image;

		protected override string ResourceKind => "bmp,tif,tiff,tga,jpg,jpeg,png,psd,gif";

		protected override void OnOpenResource()
		{
			var texture2D = GetResource<Texture2D>();

			if(!texture2D)
			{
				return;
			}

			var viewer = EditorWindow.GetWindow<TextureWindow>("Texture Window");

			viewer.SetResource(texture2D);
			viewer.Show();
		}
	}
}
#endif