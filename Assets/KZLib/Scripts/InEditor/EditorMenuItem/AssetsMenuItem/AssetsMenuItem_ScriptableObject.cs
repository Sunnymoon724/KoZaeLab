#if UNITY_EDITOR
using KZLib.KZWindow;
using UnityEditor;
using UnityEngine;

namespace KZLib.KZMenu
{
	public static partial class AssetsMenuItem
	{
		#region Open ScriptableObject
		[MenuItem("Assets/KZSubMenu/ScriptableObject/Open ScriptableObject",false,AssetsCategory.ScriptableObject)]
		private static void OnOpenScriptableObject()
		{
			var data = Selection.activeObject as ScriptableObject;
			var viewer = EditorWindow.GetWindow<ScriptableObjectWindow>("Viewer");

			viewer.SetScriptableObject(data);
			viewer.Show();
		}

		[MenuItem("Assets/KZSubMenu/ScriptableObject/Open ScriptableObject",true)]
		private static bool IsOpenAbleScriptableObject()
		{
			return Selection.activeObject is ScriptableObject;
		}
		#endregion Open ScriptableObject
	}
}	
#endif