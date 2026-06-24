#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace KZLib.EditorInternal.Menus
{
	/// <summary>
	/// <c>Assets/KZSubMenu/Script</c> menu entries.
	/// </summary>
	public static partial class DefaultMenuItem
	{
		#region Script
		[MenuItem("Assets/KZSubMenu/Script/Create ScriptableObject",false,MenuOrder.KZSubMenu.SCRIPT)]
		private static void _OnCreateScriptableObject()
		{
			var script = Selection.activeObject as MonoScript;
			// Intentionally fixed to Resources/ScriptableObject/ so runtime loading stays predictable.
			// Do not use the Project window selection folder for the output path.
			var assetPath = KZFileKit.NormalizePath($"Resources/ScriptableObject/{script.name}.asset");

			if(!KZEditorKit.DisplayCheck("Create ScriptableObject",$"Do you want to create ScriptableObject? \n Name : {script.name} \n Path : {assetPath}"))
			{
				return;
			}

			if(KZFileKit.IsFileExist(assetPath))
			{
				KZEditorKit.DisplayInfo($"{assetPath} already exists.");

				return;
			}

			var asset = ScriptableObject.CreateInstance(script.GetClass());

			KZAssetKit.CreateAsset(assetPath,asset,true);
		}

		[MenuItem("Assets/KZSubMenu/Script/Create ScriptableObject",true,MenuOrder.KZSubMenu.SCRIPT)]
		private static bool _CanCreateScriptableObject()
		{
			return _IsCreatableScriptableObjectScript();
		}
		#endregion Script
	}
}
#endif