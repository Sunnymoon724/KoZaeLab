#if UNITY_EDITOR
using UnityEngine;
using Sirenix.OdinInspector;
using KZLib.Windows;
using UnityEditor;

namespace KZLib.Attributes
{
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
}
#endif