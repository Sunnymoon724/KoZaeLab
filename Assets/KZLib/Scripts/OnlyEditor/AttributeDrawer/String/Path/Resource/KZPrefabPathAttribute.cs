#if UNITY_EDITOR
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;

namespace KZLib.Attributes
{
	/// <summary><see cref="KZPrefabPathAttribute"/> drawer. Prefab preview window.</summary>
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
}
#endif