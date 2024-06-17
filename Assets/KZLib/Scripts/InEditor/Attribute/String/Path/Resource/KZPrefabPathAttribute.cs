using System;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Diagnostics;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace KZLib.KZAttribute
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,AllowMultiple = false,Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class KZPrefabPathAttribute : KZResourcePathAttribute
	{
		public KZPrefabPathAttribute(bool _changePathButton = false,bool _newLine = false) : base(_changePathButton,_newLine) { }
	}

#if UNITY_EDITOR
	public class KZPrefabPathAttributeDrawer : KZResourcePathAttributeDrawer<KZPrefabPathAttribute>
	{
		protected override SdfIconType IconType => SdfIconType.Box;

		protected override string ResourceKind => "prefab";

		protected override void OnOpenResource()
		{
			var viewer = EditorWindow.GetWindow<PrefabViewer>("Prefab Viewer");

			viewer.Initialize(ValueEntry.SmartValue);
			viewer.Show();
		}

		protected class PrefabViewer : ResourceViewer<GameObject> { }
	}
#endif
}