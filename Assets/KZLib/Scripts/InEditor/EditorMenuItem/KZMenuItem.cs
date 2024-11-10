#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KZLib.KZWindow;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace KZLib.KZMenu
{
	public class KZMenuItem
	{
		private const int PIVOT = 5;

		private enum MenuType
		{
			Option				= 0 << PIVOT,

			Option_Delete		= Option+0*Global.MENU_LINE,
			Option_Find			= Option+1*Global.MENU_LINE,
			Option_Add			= Option+2*Global.MENU_LINE,
			Option_Check		= Option+3*Global.MENU_LINE,

			Build				= 1 << PIVOT,

			Build_Quick			= Build+0*Global.MENU_LINE,

			Window				= 2 << PIVOT,

			Window_Settings		= Window+0*Global.MENU_LINE,
			Window_Data			= Window+1*Global.MENU_LINE,
			Window_Manual		= Window+2*Global.MENU_LINE,

			Scene				= 3 << PIVOT,

			Scene_Quick			= Scene+0*Global.MENU_LINE,
			Scene_Core			= Scene+1*Global.MENU_LINE,
		}

		#region Option
		[MenuItem("KZMenu/Option/Delete All Mgr",false,(int) MenuType.Option_Delete)]
		private static void OnDeleteAllMgr()
		{
			if(!CommonUtility.DisplayCheck("Delete all manager","Delete all manager?"))
			{
				return;
			}

			CommonUtility.ReleaseManager();

			AssetsMenuItem.ReLoad();

			CommonUtility.DisplayInfo("Managers are deleted.");
		}

		[MenuItem("KZMenu/Option/Delete Empty Folder",false,(int) MenuType.Option_Delete)]
		private static void OnDeleteAllEmptyFolder()
		{
			if(!CommonUtility.DisplayCheck("Delete all empty folder","Delete all empty folder?"))
			{
				return;
			}

			CommonUtility.DeleteEmptyDirectory(Application.dataPath,()=>
			{
				AssetDatabase.Refresh();

				CommonUtility.DisplayInfo("Empty folder are deleted");
			});
		}

		[MenuItem("KZMenu/Option/Unload Unused Assets Immediate",false,(int) MenuType.Option_Delete)]
		private static void OnUnloadUnusedAssets()
		{
			if(!CommonUtility.DisplayCheck("Unload unused assets","Unload unused assets?"))
			{
				return;
			}

			EditorUtility.UnloadUnusedAssetsImmediate(true);

			CommonUtility.DisplayInfo("Assets are unloaded");
		}

		[MenuItem("KZMenu/Option/Find Missing Component",false,(int) MenuType.Option_Find)]
		private static void OnFindMissingComponent()
		{
			if(!CommonUtility.DisplayCheck("Find missing component","Find missing component?"))
			{
				return;
			}

			var builder = new StringBuilder();

			foreach(var pair in CommonUtility.LoadAssetDataGroup<GameObject>("t:prefab"))
			{
				pair.Item2.transform.TraverseChildren((child)=>
				{
					foreach(var component in child.GetComponents<Component>())
					{
						if(component)
						{
							continue;
						}

						builder.Append($"<b> <a href=\"{pair.Item1}\">{pair.Item2.name}</a> </b>\n");
					}
				});
			}

			if(builder.Length == 0)
			{
				LogTag.Editor.I("missing component is not found.");
			}
			else
			{
				builder.Insert(0,"missing component list\n");

				LogTag.Editor.I(builder.ToString());
			}
		}

		[MenuItem("KZMenu/Option/Find Missing MeshFilter",false,(int) MenuType.Option_Find)]
		private static void OnFindMissingMeshFilter()
		{
			if(!CommonUtility.DisplayCheck("Find missing meshFilter","Find missing meshFilter?"))
			{
				return;
			}

			var builder = new StringBuilder();
			var resultList = new List<string>();

			foreach(var pair in CommonUtility.LoadAssetDataGroup<GameObject>("t:prefab"))
			{
				pair.Item2.transform.TraverseChildren((child)=>
				{
					var filter = child.GetComponent<MeshFilter>();

					if(!filter || filter.sharedMesh)
					{
						return;
					}

					builder.Append($"<b> <a href=\"{pair.Item1}\">{pair.Item2.name}</a> </b> [<b> {child.GetHierarchy()} </b>]");
				});
			}

			if(builder.Length == 0)
			{
				LogTag.Editor.I("Missing meshFilter is not found.");
			}
			else
			{
				builder.Insert(0,"Missing meshFilter list\n");

				LogTag.Editor.I(builder.ToString());
			}
		}

		[MenuItem("KZMenu/Option/AddOn/Add Build Settings",false,(int) MenuType.Option_Add)]
		private static void OnAddBuildSettings()
		{
			if(AddSettings("Build Settings",BuildSettings.IsExist))
			{
				BuildSettings.CreateSettings();
			}
		}

		[MenuItem("KZMenu/Option/AddOn/Add Build Settings",true)]
		private static bool IsExistBuildSettings()
		{
			return !BuildSettings.IsExist;
		}

		[MenuItem("KZMenu/Option/AddOn/Add Meta Settings",false,(int) MenuType.Option_Add)]
		private static void OnAddMetaSettings()
		{
			if(AddSettings("Meta Settings",MetaSettings.IsExist))
			{
				MetaSettings.CreateSettings();
			}
		}

		[MenuItem("KZMenu/Option/AddOn/Add Meta Settings",true)]
		private static bool IsExistMetaSettings()
		{
			return !MetaSettings.IsExist;
		}

		[MenuItem("KZMenu/Option/AddOn/Add Config Settings",false,(int) MenuType.Option_Add)]
		private static void OnAddConfigSettings()
		{
			if(AddSettings("Config Settings",ConfigSettings.IsExist))
			{
				ConfigSettings.CreateSettings();
			}
		}

		[MenuItem("KZMenu/Option/AddOn/Add Config Settings",true)]
		private static bool IsExistConfigSettings()
		{
			return !ConfigSettings.IsExist;
		}

		[MenuItem("KZMenu/Option/AddOn/Add Language Settings",false,(int) MenuType.Option_Add)]
		private static void OnAddLanguageSettings()
		{
			if(AddSettings("Language Settings",LanguageSettings.IsExist))
			{
				LanguageSettings.CreateSettings();
			}
		}

		[MenuItem("KZMenu/Option/AddOn/Add Language Settings",true)]
		private static bool IsExistLanguageSettings()
		{
			return !LanguageSettings.IsExist;
		}

		[MenuItem("KZMenu/Option/AddOn/Add Network Settings",false,(int) MenuType.Option_Add)]
		private static void OnAddNetworkSettings()
		{
			if(AddSettings("Network Settings",NetworkSettings.IsExist))
			{
				NetworkSettings.CreateSettings();
			}
		}

		[MenuItem("KZMenu/Option/AddOn/Add Network Settings",true)]
		private static bool IsExistNetworkSettings()
		{
			return !NetworkSettings.IsExist;
		}

		private static bool AddSettings(string _name,bool _isExist)
		{
			if(_isExist)
			{
				CommonUtility.DisplayInfo($"{_name} is already exist.\n");

				return false;
			}
			else
			{
				return CommonUtility.DisplayCheck($"Add {_name}",$"Add {_name}?");
			}
		}

		[MenuItem("KZMenu/Option/Check Internet",false,(int) MenuType.Option_Check)]
		private static void OnCheckInternet()
		{
			if(Application.internetReachability != NetworkReachability.NotReachable)
			{
				LogTag.Editor.I($"Network is connected. [status : {Application.internetReachability}]");

				return;
			}
			else
			{
				LogTag.Editor.W($"Network is not connected.");
			}
		}
		#endregion Option

		#region Build
		[MenuItem("KZMenu/Build/Quick App Build",false,(int) MenuType.Build_Quick)]
		private static void OnQuickAppBuild()
		{
			BuildSettings.In.OnBuildApp();
		}

		[MenuItem("KZMenu/Build/Quick Bundle Build",false,(int) MenuType.Build_Quick)]
		private static void OnQuickBundleBuild()
		{
			BuildSettings.In.OnBuildBundle();
		}

		[MenuItem("KZMenu/Build/Quick Bundle Build",true)]
		private static bool IsEnableBundleBuild()
		{
			return false;
		}
		#endregion Build

		#region Window
		[MenuItem("KZMenu/Window/Open Settings Window",false,(int) MenuType.Window_Settings)]
		private static void OnOpenSettingsWindow()
		{
			EditorWindow.GetWindow<SettingsWindow>().Show();
		}

		[MenuItem("KZMenu/Window/Open Save Data Window",false,(int) MenuType.Window_Data)]
		private static void OnOpenSaveDataWindow()
		{
			EditorWindow.GetWindow<SaveDataWindow>().Show();
		}

		[MenuItem("KZMenu/Window/Open Manual Window",false,(int) MenuType.Window_Manual)]
		private static void OnOpenManualWindow()
		{
			EditorWindow.GetWindow<ManualWindow>().Show();
		}
		#endregion Window

		#region Scene
		[MenuItem("KZMenu/Scene/Open Main Scene",false,(int) MenuType.Scene_Core)]
		private static void OnOpenMainScene()
		{
			OnOpenScene("Main","MainScene");
		}

		private static string GetScenePath(string _sceneName)
		{
			var pathGroup = CommonUtility.GetAssetPathGroup($"t:Scene {_sceneName}");

			return pathGroup.IsNullOrEmpty() ? string.Empty : pathGroup.First();
		}

		private static void OnOpenScene(string _title,string _sceneName)
		{
			if(CommonUtility.DisplayCheck($"Open {_title} scene",$"Open {_title} scene?"))
			{
				var scenePath = GetScenePath(_sceneName);

				if(scenePath.IsEmpty())
				{
					return;
				}

				EditorSceneManager.OpenScene(scenePath);
			}
		}
		#endregion Scene
	}
}
#endif