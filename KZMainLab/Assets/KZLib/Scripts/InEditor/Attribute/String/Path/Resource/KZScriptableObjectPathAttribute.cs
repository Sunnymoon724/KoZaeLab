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
	public class KZScriptableObjectPathAttribute : KZResourcePathAttribute
	{
		public KZScriptableObjectPathAttribute(bool _changePathButton = false,bool _newLine = false) : base(_changePathButton,_newLine) { }
	}

#if UNITY_EDITOR
	public class KZScriptableObjectPathAttributeDrawer : KZResourcePathAttributeDrawer<KZScriptableObjectPathAttribute>
	{

		protected override SdfIconType IconType => SdfIconType.Journal;

		protected override string ResourceKind => "asset";

		protected override void OnOpenResource()
		{
			var viewer = EditorWindow.GetWindow<ScriptableObjectWindow>("Viewer");

			var data = AssetDatabase.LoadAssetAtPath<ScriptableObject>(CommonUtility.GetAssetsPath(ValueEntry.SmartValue));

			viewer.SetScriptableObject(data);
			viewer.Show();
		}
	}
#endif
}