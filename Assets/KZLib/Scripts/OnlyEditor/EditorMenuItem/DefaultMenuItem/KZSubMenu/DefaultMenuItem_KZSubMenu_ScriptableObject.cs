#if UNITY_EDITOR
using KZLib.Windows;
using UnityEditor;
using UnityEngine;

namespace KZLib.EditorInternal.Menus
{
	/// <summary>
	/// <c>Assets/KZSubMenu/ScriptableObject</c> menu entries.
	/// </summary>
	public static partial class DefaultMenuItem
	{
		#region ScriptableObject
		[MenuItem("Assets/KZSubMenu/ScriptableObject/Open ScriptableObject",false,MenuOrder.KZSubMenu.SCRIPTABLE_OBJECT)]
		private static void _OnOpenScriptableObject()
		{
			var viewer = EditorWindow.GetWindow<ScriptableObjectWindow>("Viewer");

			viewer.SetResource(Selection.activeObject as ScriptableObject);
			viewer.Show();
		}

		[MenuItem("Assets/KZSubMenu/ScriptableObject/Open ScriptableObject",true,MenuOrder.KZSubMenu.SCRIPTABLE_OBJECT)]
		private static bool _CanOpenScriptableObject()
		{
			return _IsScriptableObjectAsset();
		}
		#endregion ScriptableObject
	}
}
#endif