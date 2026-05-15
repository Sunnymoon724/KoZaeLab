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
	public class KZScriptableObjectPathAttribute : KZResourcePathAttribute
	{
		public KZScriptableObjectPathAttribute(bool changePathButton = false,bool newLine = false) : base(changePathButton,newLine) { }
	}

#if UNITY_EDITOR
	public class KZScriptableObjectPathAttributeDrawer : KZResourcePathAttributeDrawer<KZScriptableObjectPathAttribute>
	{

		protected override SdfIconType IconType => SdfIconType.Journal;

		protected override string ResourceKind => "asset";

		protected override void OnOpenResource()
		{
			var scriptableObject = GetResource<ScriptableObject>();

			if(!scriptableObject)
			{
				return;
			}

			var viewer = EditorWindow.GetWindow<ScriptableObjectWindow>("ScriptableObject Window");

			viewer.SetResource(scriptableObject);
			viewer.Show();
		}
	}
#endif
}