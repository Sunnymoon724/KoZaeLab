#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace KZLib.KZMenu
{
	public static partial class AssetsMenuItem
	{
		#region Create ScriptableObject
		[MenuItem("Assets/KZSubMenu/Script/Create ScriptableObject",false,AssetsCategory.Script)]
		private static void OnCreateScriptableObject()
		{
			var selected = Selection.activeObject;
			var dataPath = CommonUtility.PathCombine(GameSettings.In.MetaAssetPath,string.Format("{0}.asset",selected.name));

			if(!CommonUtility.DisplayCheck("애셋 생성",string.Format("애셋을 생성하시겠습니까? \n 에셋 이름 : {0} \n 생성 경로 : {1} (프로젝트 세팅)",selected.name,dataPath)))
			{
				return;
			}

			if(CommonUtility.IsExistFile(dataPath))
			{
				CommonUtility.DisplayError(string.Format("{0}에 파일이 이미 존재합니다.",dataPath));

				return;
			}

			var script = selected as MonoScript;
			var asset = ScriptableObject.CreateInstance(script.GetClass());

			CommonUtility.SaveAsset(dataPath,asset);
		}

		[MenuItem("Assets/KZSubMenu/Script/Create ScriptableObject",true)]
		private static bool IsCreateAbleScript()
		{
			var script = Selection.activeObject as MonoScript;

			if(script != null && script.GetClass() != null && script.GetClass().IsSubclassOf(typeof(ScriptableObject)))
			{
				return true;
			}

			return false;
		}
		#endregion Create ScriptableObject
	}
}	
#endif