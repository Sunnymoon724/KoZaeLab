using System;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Diagnostics;

#if UNITY_EDITOR

using KZLib.Windows;
using UnityEditor;

#endif

namespace KZLib.Attributes
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,AllowMultiple = false,Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class KZTexturePathAttribute : KZResourcePathAttribute
	{
		public KZTexturePathAttribute(bool changePathButton = false,bool newLine = false) : base(changePathButton,newLine) { }
	}

#if UNITY_EDITOR
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
#endif
}