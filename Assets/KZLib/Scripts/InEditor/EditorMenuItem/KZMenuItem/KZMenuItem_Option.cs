#if UNITY_EDITOR
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using KZLib.KZUtility;
using UnityEditor.Build;

namespace KZLib.KZMenu
{
	public static partial class KZMenuItem
	{
		[MenuItem("KZMenu/Option/Delete All Manager",false,MenuOrder.Option.DELETE)]
		private static void _OnDeleteAllManager()
		{
			if(!CommonUtility.DisplayCheck("Delete all manager","Delete all manager?"))
			{
				return;
			}

			CommonUtility.ReleaseManager();

			DefaultMenuItem.ReLoad();

			CommonUtility.DisplayInfo("Managers are deleted.");
		}

		[MenuItem("KZMenu/Option/Delete Empty Folder",false,MenuOrder.Option.DELETE)]
		private static void _OnDeleteAllEmptyFolder()
		{
			if(!CommonUtility.DisplayCheck("Delete all empty folder","Delete all empty folder?"))
			{
				return;
			}

			static void _AfterDelete()
			{
				AssetDatabase.Refresh();

				CommonUtility.DisplayInfo("Empty folder are deleted");
			}

			FileUtility.DeleteEmptyDirectory(Application.dataPath,_AfterDelete);
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

			var textList = new List<string>();

			foreach(var assetPath in CommonUtility.FindAssetPathGroup("t:prefab"))
			{
				var asset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

				void _Recursive(Transform child)
				{
					foreach(var component in child.GetComponents<Component>())
					{
						if(component)
						{
							continue;
						}

						textList.Add($"<b> <a href=\"{assetPath}\">{asset.name}</a> </b>");
					}
				}

				asset.transform.RecursiveChildren(_Recursive);
			}

			if(textList.Count == 0)
			{
				LogSvc.Editor.I("Missing component is not found.");
			}
			else
			{
				LogSvc.Editor.I("Missing component list");

				foreach(var text in textList)
				{
					LogSvc.Editor.I(text);
				}
			}
		}

		[MenuItem("KZMenu/Option/Find Missing MeshFilter",false,MenuOrder.Option.FIND)]
		private static void _OnFindMissingMeshFilter()
		{
			if(!CommonUtility.DisplayCheck("Find missing meshFilter","Find missing meshFilter?"))
			{
				return;
			}

			var resultList = new List<string>();

			var textListDict = new Dictionary<string,List<string>>();

			foreach(var assetPath in CommonUtility.FindAssetPathGroup("t:prefab"))
			{
				var asset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

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
			}

			if(textListDict.Count == 0)
			{
				LogSvc.Editor.I("Missing meshFilter is not found.");
			}
			else
			{
				LogSvc.Editor.I("Missing meshFilter list");
				var builder = new StringBuilder();

				foreach(var pair in textListDict)
				{
					builder.Clear();

					builder.Append($"{pair.Key}\n");

					foreach(var path in pair.Value)
					{
						builder.Append($" -[{path}]\n");
					}

					builder.AppendLine();

					LogSvc.Editor.I(builder.ToString());
				}
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
		
		[MenuItem("KZMenu/Option/Add InAppPurchase Module",false,MenuOrder.Option.MODULE)]
		private static void _OnAddInAppPurchaseModule()
		{
			if(!CommonUtility.DisplayCheck("Add in app purchase module","Add in app purchase module?"))
			{
				return;
			}

			var targetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);

			CommonUtility.AddDefineSymbol("KZLIB_IN_APP_PURCHASE",NamedBuildTarget.FromBuildTargetGroup(targetGroup));
		}

		[MenuItem("KZMenu/Option/Add InAppPurchase Module",true,MenuOrder.Option.MODULE)]
		private static bool _IsEnableInAppPurchaseModule()
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