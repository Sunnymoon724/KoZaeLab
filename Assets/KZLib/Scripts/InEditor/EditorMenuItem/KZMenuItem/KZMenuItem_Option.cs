#if UNITY_EDITOR
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using KZLib.Utilities;
using UnityEditor.Build;

namespace KZLib.EditorInternal.Menus
{
	public static partial class KZMenuItem
	{
		[MenuItem("KZMenu/Option/Delete All Manager",false,MenuOrder.Option.DELETE)]
		private static void _OnDeleteAllManager()
		{
			if(!KZEditorKit.DisplayCheckBeforeExecute("Delete all manager"))
			{
				return;
			}

			CommonUtility.ReleaseManager();

			KZEditorKit.DisplayInfo("Managers are deleted.");
		}

		[MenuItem("KZMenu/Option/Delete Empty Folder",false,MenuOrder.Option.DELETE)]
		private static void _OnDeleteAllEmptyFolder()
		{
			if(!KZEditorKit.DisplayCheckBeforeExecute("Delete all empty folder"))
			{
				return;
			}

			static void _AfterDelete()
			{
				AssetDatabase.Refresh();

				KZEditorKit.DisplayInfo("Empty folder are deleted");
			}

			KZFileKit.DeleteEmptyDirectory(Application.dataPath,_AfterDelete);
		}

		[MenuItem("KZMenu/Option/Unload Unused Assets Immediate",false,MenuOrder.Option.DELETE)]
		private static void _OnUnloadUnusedAssets()
		{
			if(!KZEditorKit.DisplayCheckBeforeExecute("Unload unused assets"))
			{
				return;
			}

			EditorUtility.UnloadUnusedAssetsImmediate(true);

			KZEditorKit.DisplayInfo("Assets are unloaded");
		}

		[MenuItem("KZMenu/Option/Find Missing Component",false,MenuOrder.Option.FIND)]
		private static void _OnFindMissingComponent()
		{
			if(!KZEditorKit.DisplayCheckBeforeExecute("Find missing component"))
			{
				return;
			}

			if(KZEditorKit.DisplayCancelableProgressBar("Find missing component","Finding...",0.0f))
			{
				KZEditorKit.ClearProgressBar();

				return;
			}

			var textHashSet = new HashSet<string>();

			bool _Execute(string assetPath,int index,int totalCount)
			{
				var asset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

				if(!asset)
				{
					return true;
				}

				if(KZEditorKit.DisplayCancelableProgressBar("Find missing component",$"Finding ({index/totalCount})",index/ (float) totalCount))
				{
					KZEditorKit.ClearProgressBar();

					return false;
				}

				void _Recursive(Transform child)
				{
					var componentArray = child.GetComponents<Component>();

					for(var i=0;i<componentArray.Length;i++)
					{
						if(componentArray[i])
						{
							continue;
						}

						textHashSet.Add($"<b> <a href=\"{assetPath}\">{asset.name}</a> </b>");
					}
				}

				asset.transform.RecursiveChildren(_Recursive);

				return true;
			}

			KZAssetKit.ExecuteMatchedAssetPath("t:prefab",null,_Execute);

			KZEditorKit.ClearProgressBar();

			if(textHashSet.Count == 0)
			{
				LogChannel.Editor.I("Missing component is not found.");
			}
			else
			{
				LogChannel.Editor.I("Missing component list");

				foreach(var text in textHashSet)
				{
					LogChannel.Editor.I(text);
				}
			}
		}

		[MenuItem("KZMenu/Option/Find Missing MeshFilter",false,MenuOrder.Option.FIND)]
		private static void _OnFindMissingMeshFilter()
		{
			if(!KZEditorKit.DisplayCheckBeforeExecute("Find missing meshFilter"))
			{
				return;
			}

			if(KZEditorKit.DisplayCancelableProgressBar("Find missing meshFilter","Finding...",0.0f))
			{
				KZEditorKit.ClearProgressBar();

				return;
			}

			var resultList = new List<string>();
			var textListDict = new Dictionary<string,List<string>>();

			bool _Execute(string assetPath,int index,int totalCount)
			{
				var asset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

				if(!asset)
				{
					return true;
				}

				if(KZEditorKit.DisplayCancelableProgressBar("Find missing meshFilter",$"Finding ({index/totalCount})",index/(float) totalCount))
				{
					KZEditorKit.ClearProgressBar();

					return false;
				}

				void _Recursive(Transform child)
				{
					var filter = child.GetComponent<MeshFilter>();

					if(!filter || filter.sharedMesh)
					{
						return;
					}

					textListDict.AddOrCreate($"<b> <a href=\"{assetPath}\">{asset.name}</a> </b>",child.FindHierarchy());
				}

				asset.transform.RecursiveChildren(_Recursive);

				return true;
			}

			KZAssetKit.ExecuteMatchedAssetPath("t:prefab",null,_Execute);

			KZEditorKit.ClearProgressBar();

			if(textListDict.Count == 0)
			{
				LogChannel.Editor.I("Missing meshFilter is not found.");
			}
			else
			{
				LogChannel.Editor.I("Missing meshFilter list");
				var builder = new StringBuilder();

				foreach(var pair in textListDict)
				{
					builder.Clear();
					builder.Append($"{pair.Key}\n");

					var pathList = pair.Value;

					for(var i=0;i<pathList.Count;i++)
					{
						builder.Append($" -[{pathList[i]}]\n");
					}

					builder.AppendLine();

					LogChannel.Editor.I(builder.ToString());
				}
			}
		}

		[MenuItem("KZMenu/Option/Add PlayFab Module",false,MenuOrder.Option.MODULE)]
		private static void _OnAddPlayFabModule()
		{
			if(!KZEditorKit.DisplayCheckBeforeExecute("Add playFab module"))
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
		
		[MenuItem("KZMenu/Option/Add InAppPurchase Module",false,MenuOrder.Option.MODULE)]
		private static void _OnAddInAppPurchaseModule()
		{
			if(!KZEditorKit.DisplayCheckBeforeExecute("Add in app purchase module"))
			{
				return;
			}

			var targetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);

			CommonUtility.AddDefineSymbol("KZLIB_IN_APP_PURCHASE",NamedBuildTarget.FromBuildTargetGroup(targetGroup));
		}

		[MenuItem("KZMenu/Option/Add InAppPurchase Module",true,MenuOrder.Option.MODULE)]
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
			else
			{
				LogChannel.Editor.W($"Network is not connected.");
			}
		}
	}
}
#endif