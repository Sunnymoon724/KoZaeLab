#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace KZLib.KZMenu
{
	public partial class KZMenuItem
	{
		[MenuItem("KZMenu/Option/Delete All Mgr",false,KZCategory.Option_Delete)]
		private static void OnDeleteAllMgr()
		{
			if(!CommonUtility.DisplayCheck("기본 매니저 삭제","기본 매니저를 삭제하시겠습니까?"))
			{
				return;
			}

			CommonUtility.ReleaseManager();

			AssetsMenuItem.ReLoad();

			CommonUtility.DisplayInfo("기본 매니저가 전부 지워졌습니다.");
		}

		[MenuItem("KZMenu/Option/Delete Empty Directory",false,KZCategory.Option_Delete)]
		private static void OnDeleteAllEmptyDirectory()
		{
			if(!CommonUtility.DisplayCheck("빈 파일 삭제","빈 파일 삭제하시겠습니까?"))
			{
				return;
			}

			CommonUtility.DeleteEmptyDirectory(Application.dataPath,()=>
			{
				AssetDatabase.Refresh();

				CommonUtility.DisplayInfo("모든 빈 파일을 삭제했습니다.");
			});
		}

		[MenuItem("KZMenu/Option/Unload Unused Assets Immediate",false,KZCategory.Option_Delete)]
		private static void OnUnloadUnusedAssetsImmediate()
		{
			if(!CommonUtility.DisplayCheck("사용하지 않는 에셋 반환","사용하지 않는 에셋을 반환하시겠습니까?"))
			{
				return;
			}

			EditorUtility.UnloadUnusedAssetsImmediate(true);

			CommonUtility.DisplayInfo("사용하지 않는 에셋을 반환했습니다.");
		}

		[MenuItem("KZMenu/Option/Find Missing Component",false,KZCategory.Option_Find)]
		private static void OnFindMissingComponent()
		{
			if(!CommonUtility.DisplayCheck("미싱 컴퍼넌트 찾기","미싱 컴퍼넌트를 찾으시겠습니까?"))
			{
				return;
			}

			var resultList = new List<string>();

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

						resultList.Add(string.Format("프리펩 <b> <a href=\"{1}\">{0}</a> </b>이며 내부 경로는 <b> {2} </b>입니다.",pair.Item2.name,pair.Item1,child.GetHierarchy()));
					}
				});
			}

			if(resultList.IsNullOrEmpty())
			{
				Log.Editor.I("컴포넌트가 오류인 프리펩은 없습니다.");
			}
			else
			{
				Log.Editor.I("컴포넌트가 오류인 프리펩 리스트 입니다.");

				foreach(var result in resultList)
				{
					Log.Editor.I(result);
				}
			}
		}

		[MenuItem("KZMenu/Option/Find Missing MeshFilter",false,KZCategory.Option_Find)]
		private static void OnFindMissingMeshFilter()
		{
			if(!CommonUtility.DisplayCheck("미싱 메쉬 필터 찾기","미싱 메쉬 필터를 찾으시겠습니까?"))
			{
				return;
			}

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

					resultList.Add(string.Format("프리펩 <b> <a href=\"{1}\">{0}</a> </b>이며 내부 경로는 <b> {2} </b>입니다.",pair.Item2.name,pair.Item1,child.GetHierarchy()));
				});
			}

			if(resultList.IsNullOrEmpty())
			{
				Log.Editor.I("메쉬 필터가 오류인 프리펩은 없습니다.");
			}
			else
			{
				Log.Editor.I("메쉬 필터가 오류인 프리펩 리스트 입니다.");

				foreach(var result in resultList)
				{
					Log.Editor.I(result);
				}
			}
		}

		[MenuItem("KZMenu/Option/AddOn/Add Build Settings",false,KZCategory.Option_Add)]
		private static void OnAddBuildSettings()
		{
			if(AddSettings("빌드 설정",BuildSettings.IsExist))
			{
				BuildSettings.CreateSettings();
			}
		}

		[MenuItem("KZMenu/Option/AddOn/Add Build Settings",true)]
		private static bool IsExistBuildSettings()
		{
			return !BuildSettings.IsExist;
		}

		[MenuItem("KZMenu/Option/AddOn/Add Meta Settings",false,KZCategory.Option_Add)]
		private static void OnAddMetaSettings()
		{
			if(AddSettings("메타 설정",MetaSettings.IsExist))
			{
				MetaSettings.CreateSettings();
			}
		}

		[MenuItem("KZMenu/Option/AddOn/Add Meta Settings",true)]
		private static bool IsExistMetaSettings()
		{
			return !MetaSettings.IsExist;
		}

		[MenuItem("KZMenu/Option/AddOn/Add Language Settings",false,KZCategory.Option_Add)]
		private static void OnAddLanguageSettings()
		{
			if(AddSettings("언어 설정",LanguageSettings.IsExist))
			{
				LanguageSettings.CreateSettings();
			}
		}

		[MenuItem("KZMenu/Option/AddOn/Add Language Settings",true)]
		private static bool IsExistLanguageSettings()
		{
			return !LanguageSettings.IsExist;
		}

		[MenuItem("KZMenu/Option/AddOn/Add Network Settings",false,KZCategory.Option_Add)]
		private static void OnAddNetworkSettings()
		{
			if(AddSettings("통신 설정",NetworkSettings.IsExist))
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
				CommonUtility.DisplayInfo(string.Format("{0}이 이미 존재합니다.\n",_name));

				return false;
			}
			else
			{
				return CommonUtility.DisplayCheck(string.Format("{0} 추가",_name),string.Format("{0} 추가하시겠습니까?",_name));
			}
		}

		[MenuItem("KZMenu/Option/Check Internet",false,KZCategory.Option_Check)]
		private static void OnCheckInternet()
		{
			if(Application.internetReachability != NetworkReachability.NotReachable)
			{
				Log.Editor.I("인터넷이 연결되어 있습니다. [상태 : {0}]",Application.internetReachability);

				return;
			}

			Log.Editor.W("인터넷이 연결되어 있지 않습니다.");
		}
	}
}
#endif