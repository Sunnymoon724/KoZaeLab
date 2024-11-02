using System;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Diagnostics;

#if UNITY_EDITOR

using KZLib.KZWindow;
using UnityEditor;

#endif

namespace KZLib.KZAttribute
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,AllowMultiple = false,Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class KZTexturePathAttribute : KZResourcePathAttribute
	{
		public KZTexturePathAttribute(bool _changePathButton = false,bool _newLine = false) : base(_changePathButton,_newLine) { }
	}

#if UNITY_EDITOR
	public class KZTexturePathAttributeDrawer : KZResourcePathAttributeDrawer<KZTexturePathAttribute>
	{
		protected override SdfIconType IconType => SdfIconType.Image;

		protected override string ResourceKind => "bmp,tif,tiff,tga,jpg,jpeg,png,psd,gif";

		protected override void OnOpenResource()
		{
			var viewer = EditorWindow.GetWindow<TextureWindow>("Viewer");

			var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(CommonUtility.GetAssetsPath(ValueEntry.SmartValue));

			viewer.SetTexture(sprite.texture);
			viewer.Show();
		}
	}
#endif
}