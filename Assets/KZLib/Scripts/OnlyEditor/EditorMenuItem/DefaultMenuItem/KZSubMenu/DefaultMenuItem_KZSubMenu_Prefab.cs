#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace KZLib.EditorInternal.Menus
{
	/// <summary>
	/// <c>Assets/KZSubMenu/Prefab</c> menu entries.
	/// </summary>
	public static partial class DefaultMenuItem
	{
		#region Prefab
		[MenuItem("Assets/KZSubMenu/Prefab/Show Mesh Name",false,MenuOrder.KZSubMenu.PREFAB_SHOW_MESH_NAME)]
		private static void _OnShowPrefabMeshName()
		{
			var prefab = Selection.activeObject as GameObject;
			var assetPath = AssetDatabase.GetAssetPath(prefab);
			var contentsRoot = PrefabUtility.LoadPrefabContents(assetPath);

			try
			{
				_LogMeshNameList(contentsRoot);
			}
			finally
			{
				PrefabUtility.UnloadPrefabContents(contentsRoot);
			}
		}

		[MenuItem("Assets/KZSubMenu/Prefab/Show Mesh Name",true,MenuOrder.KZSubMenu.PREFAB_SHOW_MESH_NAME)]
		private static bool _CanShowPrefabMeshName()
		{
			return _IsPrefabAsset();
		}
		#endregion Prefab
	}
}
#endif