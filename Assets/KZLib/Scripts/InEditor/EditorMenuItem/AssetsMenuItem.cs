#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using KZLib.KZWindow;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace KZLib.KZMenu
{
	public static partial class AssetsMenuItem
	{
		private const int PIVOT_ORDER = 2000;

		private enum MenuType
		{
			Total				= PIVOT_ORDER+0*Global.MENU_LINE,
			Prefab				= PIVOT_ORDER+1*Global.MENU_LINE,
			Script				= PIVOT_ORDER+2*Global.MENU_LINE,
			Texture				= PIVOT_ORDER+3*Global.MENU_LINE,
			ScriptableObject	= PIVOT_ORDER+4*Global.MENU_LINE,
		}

		private static Dictionary<string,List<string>> s_AssetsPathListDict = null;

		private static Dictionary<string,List<string>> AssetsPathListDict
		{
			get
			{
				if(s_AssetsPathListDict == null)
				{
					LoadAssetsPath();
				}

				return s_AssetsPathListDict;
			}
		}

		private static void LoadAssetsPath()
		{
			if(CommonUtility.DisplayCancelableProgressBar("Asset finder load","Loading asset finder",0.1f))
			{
				CommonUtility.ClearProgressBar();

				return;
			}

			var pathGroup = CommonUtility.GetAssetPathGroup();
			var totalCount = pathGroup.Count();
			var index = 0;
			var dependantDict = new Dictionary<string,string[]>();

			foreach(var path in pathGroup)
			{
				dependantDict[path] = AssetDatabase.GetDependencies(path,false);

				if(CommonUtility.DisplayCancelableProgressBar("Asset finder load",$"Loading asset finder [{index}/{totalCount}]",index++,totalCount))
				{
					CommonUtility.ClearProgressBar();

					return;
				}
			}

			s_AssetsPathListDict = new Dictionary<string,List<string>>();

			foreach(var pair in dependantDict)
			{
				foreach(var dependant in pair.Value)
				{
					if(!s_AssetsPathListDict.ContainsKey(dependant))
					{
						s_AssetsPathListDict.Add(dependant,new List<string>());
					}

					s_AssetsPathListDict[dependant].Add(pair.Key);
				}
			}

			CommonUtility.ClearProgressBar();
		}

		public static void ReLoad()
		{
			s_AssetsPathListDict = null;
		}

		private static bool IsValidAsset()
		{
			if(Selection.activeObject == null)
			{
				LogTag.Editor.W("Selection is null.");

				return false;
			}

			var path = AssetDatabase.GetAssetPath(Selection.activeObject);

			if(!CommonUtility.IsFilePath(path))
			{
				LogTag.Editor.W("Selection is folder.");

				return false;
			}

			return true;
		}

		#region Total
		[MenuItem("Assets/KZSubMenu/Find Assets Using",false,(int) MenuType.Total)]
		private static void OnFindAssetsUsing()
		{
			if(!IsValidAsset())
			{
				return;
			}

			var textList = new List<string>();

			foreach(var selected in Selection.GetFiltered(typeof(Object),SelectionMode.Assets))
			{
				var asset = AssetDatabase.GetAssetPath(selected);

				textList.Add($"Finding assets using <b> {CommonUtility.GetOnlyName(asset)} </b>");

				if(AssetsPathListDict.TryGetValue(asset,out var dependantList))
				{
					foreach(var dependant in dependantList)
					{
						textList.Add($"<a href=\"{dependant}\">{CommonUtility.GetOnlyName(dependant)}</a> ");
					}
				}
			}

			if(textList.IsNullOrEmpty())
			{
				LogTag.Editor.I("Using asset is null.");
			}
			else
			{
				LogTag.Editor.I("Using asset list");

				foreach(var text in textList)
				{
					LogTag.Editor.I(text);
				}
			}
		}

		[MenuItem("Assets/KZSubMenu/Find Assets With",false,(int) MenuType.Total)]
		private static void OnFindAssetsWith()
		{
			if(!IsValidAsset())
			{
				return;
			}

			var textList = new List<string>();

			foreach(var selected in Selection.GetFiltered(typeof(Object),SelectionMode.Assets))
			{
				var asset = AssetDatabase.GetAssetPath(selected);

				textList.Add($"Finding assets with <b> {CommonUtility.GetOnlyName(asset)} </b>");

				foreach(var dependant in AssetDatabase.GetDependencies(asset,false))
				{
					if(!dependant.StartsWith(Global.ASSETS_HEADER))
					{
						continue;
					}

					textList.Add($"<a href=\"{dependant}\">{CommonUtility.GetOnlyName(dependant)}</a> ");
				}
			}

			if(textList.IsNullOrEmpty())
			{
				LogTag.Editor.I("Assets with is null.");
			}
			else
			{
				LogTag.Editor.I("Assets with list");

				foreach(var text in textList)
				{
					LogTag.Editor.I(text);
				}
			}
		}

		#endregion Total

		#region Script
		[MenuItem("Assets/KZSubMenu/Script/Create ScriptableObject",false,(int) MenuType.Script)]
		private static void OnCreateScriptableObject()
		{
			var selection = Selection.activeObject;
			var dataPath = CommonUtility.NormalizePath($"Resources/ScriptableObjects/{selection.name}.asset");

			if(!CommonUtility.DisplayCheck("Create scriptableObject",$"Create scriptableObject? \n Name : {selection.name} \n Path : {dataPath}"))
			{
				return;
			}

			if(CommonUtility.IsFileExist(dataPath))
			{
				CommonUtility.DisplayError(new NullReferenceException($"{dataPath} is exist."));

				return;
			}

			var script = selection as MonoScript;
			var asset = ScriptableObject.CreateInstance(script.GetClass());

			CommonUtility.SaveAsset(dataPath,asset);
		}

		[MenuItem("Assets/KZSubMenu/Script/Create ScriptableObject",true,(int) MenuType.Script)]
		private static bool IsCreateAbleScriptableObject()
		{
			var script = Selection.activeObject as MonoScript;

			if(script != null && script.GetClass() != null && script.GetClass().IsSubclassOf(typeof(ScriptableObject)))
			{
				return true;
			}

			return false;
		}
		#endregion Script

		#region Texture
		[MenuItem("Assets/KZSubMenu/Texture/Open Texture",false,(int) MenuType.Texture)]
		private static void OnOpenTexture()
		{
			var viewer = EditorWindow.GetWindow<TextureWindow>("Viewer");

			viewer.SetTexture(Selection.activeObject as Texture2D);
			viewer.Show();
		}

		[MenuItem("Assets/KZSubMenu/Texture/Open Texture",true,(int) MenuType.Texture)]
		private static bool IsOpenAbleTexture()
		{
			return Selection.activeObject as Texture2D;
		}
		#endregion Texture

		#region ScriptableObject
		[MenuItem("Assets/KZSubMenu/ScriptableObject/Open ScriptableObject",false,(int) MenuType.ScriptableObject)]
		private static void OnOpenScriptableObject()
		{
			var viewer = EditorWindow.GetWindow<ScriptableObjectWindow>("Viewer");

			viewer.SetScriptableObject(Selection.activeObject as ScriptableObject);
			viewer.Show();
		}

		[MenuItem("Assets/KZSubMenu/ScriptableObject/Open ScriptableObject",true,(int) MenuType.ScriptableObject)]
		private static bool IsOpenAbleScriptableObject()
		{
			return Selection.activeObject is ScriptableObject;
		}
		#endregion ScriptableObject
	}
}
#endif