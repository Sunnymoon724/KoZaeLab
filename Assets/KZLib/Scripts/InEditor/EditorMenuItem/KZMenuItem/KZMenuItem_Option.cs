#if UNITY_EDITOR
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using KZLib.KZUtility;
using UnityEditor.Build;

namespace KZLib.KZMenu
{
	public partial class KZMenuItem
	{
		[MenuItem("KZMenu/Option/Delete All Mgr",false,MenuOrder.Option.DELETE)]
		private static void _OnDeleteAllMgr()
		{
			if(!CommonUtility.DisplayCheck("Delete all manager","Delete all manager?"))
			{
				return;
			}

			CommonUtility.ReleaseManager();

			AssetsMenuItem.ReLoad();

			CommonUtility.DisplayInfo("Managers are deleted.");
		}

		[MenuItem("KZMenu/Option/Delete Empty Folder",false,MenuOrder.Option.DELETE)]
		private static void _OnDeleteAllEmptyFolder()
		{
			if(!CommonUtility.DisplayCheck("Delete all empty folder","Delete all empty folder?"))
			{
				return;
			}

			FileUtility.DeleteEmptyDirectory(Application.dataPath,()=>
			{
				AssetDatabase.Refresh();

				CommonUtility.DisplayInfo("Empty folder are deleted");
			});
		}

		[MenuItem("KZMenu/Option/Unload Unused Assets Immediate",false,MenuOrder.Option.DELETE)]
		private static void _OnUnloadUnusedAssets()
		{
			if(!CommonUtility.DisplayCheck("Unload unused assets","Unload unused assets?"))
			{
				return;
			}

			EditorUtility.UnloadUnusedAssetsImmediate(true);

			CommonUtility.DisplayInfo("Assets are unloaded");
		}

		[MenuItem("KZMenu/Option/Find Missing Component",false,MenuOrder.Option.FIND)]
		private static void _OnFindMissingComponent()
		{
			if(!CommonUtility.DisplayCheck("Find missing component","Find missing component?"))
			{
				return;
			}

			var stringBuilder = new StringBuilder();

			foreach(var assetPath in CommonUtility.FindAssetPathGroup("t:prefab"))
			{
				var asset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

				asset.transform.TraverseChildren((child)=>
				{
					foreach(var component in child.GetComponents<Component>())
					{
						if(component)
						{
							continue;
						}

						stringBuilder.Append($"<b> <a href=\"{assetPath}\">{asset.name}</a> </b>\n");
					}
				});
			}

			if(stringBuilder.Length == 0)
			{
				LogSvc.Editor.I("Missing component is not found.");
			}
			else
			{
				stringBuilder.Insert(0,"Missing component list\n");

				LogSvc.Editor.I(stringBuilder.ToString());
			}
		}

		[MenuItem("KZMenu/Option/Find Missing MeshFilter",false,MenuOrder.Option.FIND)]
		private static void _OnFindMissingMeshFilter()
		{
			if(!CommonUtility.DisplayCheck("Find missing meshFilter","Find missing meshFilter?"))
			{
				return;
			}

			var stringBuilder = new StringBuilder();
			var resultList = new List<string>();

			foreach(var assetPath in CommonUtility.FindAssetPathGroup("t:prefab"))
			{
				var asset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

				asset.transform.TraverseChildren((child)=>
				{
					var filter = child.GetComponent<MeshFilter>();

					if(!filter || filter.sharedMesh)
					{
						return;
					}

					stringBuilder.Append($"<b> <a href=\"{assetPath}\">{asset.name}</a> </b> [<b> {child.FindHierarchy()} </b>]");
				});
			}

			if(stringBuilder.Length == 0)
			{
				LogSvc.Editor.I("Missing meshFilter is not found.");
			}
			else
			{
				stringBuilder.Insert(0,"Missing meshFilter list\n");

				LogSvc.Editor.I(stringBuilder.ToString());
			}
		}

		[MenuItem("KZMenu/Option/Add PlayFab Module",false,MenuOrder.Option.MODULE)]
		private static void _OnAddPlayFabModule()
		{
			if(!CommonUtility.DisplayCheck("Add playFab module","Add playFab module?"))
			{
				return;
			}

			var targetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);

			CommonUtility.AddDefineSymbol("KZLIB_PLAY_FAB",NamedBuildTarget.FromBuildTargetGroup(targetGroup));
		}

		[MenuItem("KZMenu/Option/Add PlayFab Module",true,MenuOrder.Option.MODULE)]
		private static bool _IsEnablePlayFabModule()
		{
#if KZLIB_PLAY_FAB
			return false;
#else
			return true;
#endif
		}

		[MenuItem("KZMenu/Option/Check Internet",false,MenuOrder.Option.CHECK)]
		private static void _OnCheckInternet()
		{
			if(Application.internetReachability != NetworkReachability.NotReachable)
			{
				LogSvc.Editor.I($"Network is connected. [status : {Application.internetReachability}]");

				return;
			}
			else
			{
				LogSvc.Editor.W($"Network is not connected.");
			}
		}
	}
}
#endif