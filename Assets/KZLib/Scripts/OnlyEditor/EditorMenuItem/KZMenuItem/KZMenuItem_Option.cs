#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KZLib.EditorInternal.Menus
{
	public static partial class KZMenuItem
	{
		[MenuItem("KZMenu/Option/Delete All Manager",false,MenuOrder.Option.DELETE_ALL_MANAGER)]
		private static void _OnDeleteAllManager()
		{
			if(!KZEditorKit.DisplayConfirm("Delete all managers"))
			{
				return;
			}

			KZGameKit.ReleaseManager();

			KZEditorKit.DisplayInfo("Managers are deleted.");
		}

		[MenuItem("KZMenu/Option/Delete Empty Folder",false,MenuOrder.Option.DELETE_EMPTY_FOLDER)]
		private static void _OnDeleteAllEmptyFolder()
		{
			if(!KZEditorKit.DisplayConfirm("Delete all empty folders"))
			{
				return;
			}

			static void _AfterDelete()
			{
				AssetDatabase.Refresh();

				KZEditorKit.DisplayInfo("Empty folders have been deleted.");
			}

			KZFileKit.DeleteEmptyFolders(Application.dataPath,_AfterDelete);
		}

		[MenuItem("KZMenu/Option/Unload Unused Assets Immediate",false,MenuOrder.Option.DELETE_UNUSED_ASSETS)]
		private static void _OnUnloadUnusedAssets()
		{
			if(!KZEditorKit.DisplayConfirm("Unload unused assets"))
			{
				return;
			}

			EditorUtility.UnloadUnusedAssetsImmediate(true);

			KZEditorKit.DisplayInfo("Assets are unloaded");
		}

		[MenuItem("KZMenu/Option/Find Missing Component",false,MenuOrder.Option.FIND_MISSING_COMPONENT)]
		private static void _OnFindMissingComponent()
		{
			static void _OnVisitTransform(string sourceLabel,Transform child,Dictionary<string,List<string>> resultDict)
			{
				var componentArray = child.GetComponents<Component>();

				for(var i=0;i<componentArray.Length;i++)
				{
					if(componentArray[i])
					{
						continue;
					}

					resultDict.AddOrCreate(sourceLabel,child.BuildHierarchyPath());
				}
			}

			_FindMissingInPrefabsAndOpenScenes("Find missing component","Missing component is not found.","Missing component list",_OnVisitTransform);
		}

		[MenuItem("KZMenu/Option/Find Missing MeshFilter",false,MenuOrder.Option.FIND_MISSING_MESH_FILTER)]
		private static void _OnFindMissingMeshFilter()
		{
			static void _OnVisitTransform(string sourceLabel,Transform child,Dictionary<string,List<string>> resultDict)
			{
				var filter = child.GetComponent<MeshFilter>();

				if(!filter || filter.sharedMesh)
				{
					return;
				}

				resultDict.AddOrCreate(sourceLabel,child.BuildHierarchyPath());
			}

			_FindMissingInPrefabsAndOpenScenes("Find missing meshFilter","Missing meshFilter is not found.","Missing meshFilter list",_OnVisitTransform);
		}

		/// <summary>
		/// Open scenes in the editor and prefab assets (<c>t:prefab</c>).
		/// </summary>
		private static void _FindMissingInPrefabsAndOpenScenes(string title,string emptyResultMessage,string resultListTitle,Action<string,Transform,Dictionary<string,List<string>>> onVisitTransform)
		{
			if(!KZEditorKit.DisplayConfirm(title))
			{
				return;
			}

			var resultDict = new Dictionary<string,List<string>>();
			var cancelled = false;
			var openSceneList = new List<Scene>();

			for(var i=0;i<SceneManager.sceneCount;i++)
			{
				var scene = SceneManager.GetSceneAt(i);

				if(scene.isLoaded)
				{
					openSceneList.Add(scene);
				}
			}

			var prefabGuidArray = AssetDatabase.FindAssets("t:prefab");
			var totalCount = openSceneList.Count + prefabGuidArray.Length;

			if(totalCount == 0)
			{
				LogChannel.Editor.I(emptyResultMessage);

				return;
			}

			var index = 0;

			for(var i=0;i<openSceneList.Count;i++)
			{
				if(_TryCancelFindMissingProgress(title,ref cancelled,index,totalCount))
				{
					break;
				}

				_ScanFindMissingOpenScene(openSceneList[i],onVisitTransform,resultDict);

				index++;
			}

			for(var i=0;!cancelled && i<prefabGuidArray.Length;i++)
			{
				if(_TryCancelFindMissingProgress(title,ref cancelled,index,totalCount))
				{
					break;
				}

				var assetPath = AssetDatabase.GUIDToAssetPath(prefabGuidArray[i]);

				if(!KZFileKit.IsFileExist(assetPath))
				{
					index++;

					continue;
				}

				var asset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

				if(asset)
				{
					var sourceLabel = _GetFindMissingSourceLabel(asset.name,assetPath);

					void _Recursive(Transform child)
					{
						onVisitTransform(sourceLabel,child,resultDict);
					}

					asset.transform.RecursiveChildren(_Recursive);
				}

				index++;
			}

			KZEditorKit.ClearProgressBar();

			_LogMissingFindResults(title,resultDict,emptyResultMessage,resultListTitle,cancelled);
		}

		private static bool _TryCancelFindMissingProgress(string title,ref bool cancelled,int index,int totalCount)
		{
			if(!KZEditorKit.DisplayCancelableProgressBar(title,$"Finding ({index + 1}/{totalCount})",index + 1,totalCount))
			{
				return false;
			}

			cancelled = true;

			return true;
		}

		private static void _ScanFindMissingOpenScene(Scene scene,Action<string,Transform,Dictionary<string,List<string>>> onVisitTransform,Dictionary<string,List<string>> resultDict)
		{
			var sourceLabel = _GetFindMissingSourceLabel(scene.name,scene.path);
			var rootArray = scene.GetRootGameObjects();

			for(var i=0;i<rootArray.Length;i++)
			{
				void _Recursive(Transform child)
				{
					onVisitTransform(sourceLabel,child,resultDict);
				}

				rootArray[i].transform.RecursiveChildren(_Recursive);
			}
		}

		private static string _GetFindMissingSourceLabel(string displayName,string assetPath)
		{
			if(assetPath.IsEmpty())
			{
				return $"<b>{displayName} (open scene)</b>";
			}

			return $"<b> <a href=\"{assetPath}\">{displayName}</a> </b>";
		}

		private static void _LogMissingFindResults(
			string title,
			Dictionary<string,List<string>> resultDict,
			string emptyResultMessage,
			string resultListTitle,
			bool cancelled)
		{
			if(cancelled)
			{
				LogChannel.Editor.W($"{title} was cancelled.");
			}

			if(resultDict.Count == 0)
			{
				if(!cancelled)
				{
					LogChannel.Editor.I(emptyResultMessage);
				}

				return;
			}

			LogChannel.Editor.I(resultListTitle);
			DefaultMenuItem._LogGroupedPathList(resultDict);
		}

		[MenuItem("KZMenu/Option/Add PlayFab Module",false,MenuOrder.Option.MODULE_PLAY_FAB)]
		private static void _OnAddPlayFabModule()
		{
			if(!KZEditorKit.DisplayConfirm("Add playFab module"))
			{
				return;
			}

			KZEditorKit.AddDefineSymbolForAllBuildTargets("KZLIB_PLAY_FAB");

			KZEditorKit.DisplayInfo("PlayFab define symbol is added to Standalone, Android, and iOS.");
		}

		[MenuItem("KZMenu/Option/Add PlayFab Module",true,MenuOrder.Option.MODULE_PLAY_FAB)]
		private static bool _IsEnablePlayFabModule()
		{
#if KZLIB_PLAY_FAB
			return false;
#else
			return true;
#endif
		}

		[MenuItem("KZMenu/Option/Add InAppPurchase Module",false,MenuOrder.Option.MODULE_IN_APP_PURCHASE)]
		private static void _OnAddInAppPurchaseModule()
		{
			if(!KZEditorKit.DisplayConfirm("Add in app purchase module"))
			{
				return;
			}

			KZEditorKit.AddDefineSymbolForAllBuildTargets("KZLIB_IN_APP_PURCHASE");

			KZEditorKit.DisplayInfo("InAppPurchase define symbol is added to Standalone, Android, and iOS.");
		}

		[MenuItem("KZMenu/Option/Add InAppPurchase Module",true,MenuOrder.Option.MODULE_IN_APP_PURCHASE)]
		private static bool _IsEnableInAppPurchaseModule()
		{
#if KZLIB_IN_APP_PURCHASE
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
				LogChannel.Editor.I($"Network is connected. [status : {Application.internetReachability}]");

				return;
			}

			LogChannel.Editor.W("Network is not connected.");
		}
	}
}
#endif