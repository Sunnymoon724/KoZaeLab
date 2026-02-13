#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using KZLib.Utilities;
using KZLib.Windows;
using UnityEditor;
using UnityEngine;

namespace KZLib.EditorInternal.Menus
{
	public static partial class DefaultMenuItem
	{
		private static Dictionary<string,List<string>> s_assetPathDict = null;

		private static Dictionary<string,List<string>> AssetPathDict
		{
			get
			{
				if(s_assetPathDict == null)
				{
					_LoadAssetsPath();
				}

				return s_assetPathDict;
			}
		}

		private static void _LoadAssetsPath()
		{
			if(CommonUtility.DisplayCancelableProgressBar("Asset finder load","Loading asset finder",0.1f))
			{
				CommonUtility.ClearProgressBar();

				return;
			}

			var pathGroup = CommonUtility.FindAssetPathGroup();
			var totalCount = pathGroup.Count();
			var index = 0;
			var dependantListDict = new Dictionary<string,string[]>();

			foreach(var path in pathGroup)
			{
				dependantListDict[path] = AssetDatabase.GetDependencies(path,false);

				if(CommonUtility.DisplayCancelableProgressBar("Asset finder load",$"Loading asset finder [{index}/{totalCount}]",index++,totalCount))
				{
					CommonUtility.ClearProgressBar();

					return;
				}
			}

			s_assetPathDict = new Dictionary<string,List<string>>();

			foreach(var pair in dependantListDict)
			{
				var dependantList =  pair.Value;

				for(var i=0;i<dependantList.Length;i++)
				{
					var dependant = dependantList[i];

					if(!s_assetPathDict.ContainsKey(dependant))
					{
						s_assetPathDict.Add(dependant,new List<string>());
					}

					s_assetPathDict[dependant].Add(pair.Key);
				}
			}

			CommonUtility.ClearProgressBar();
		}

		public static void ReLoad()
		{
			s_assetPathDict = null;
		}

		#region Script
		[MenuItem("Assets/KZSubMenu/Script/Create ScriptableObject",false,MenuOrder.Asset.SCRIPT)]
		private static void _OnCreateScriptableObject()
		{
			var selection = Selection.activeObject;
			var assetPath = FileUtility.NormalizePath($"Resources/ScriptableObject/{selection.name}.asset");

			if(!CommonUtility.DisplayCheck("Create scriptableObject",$"Create scriptableObject? \n Name : {selection.name} \n Path : {assetPath}"))
			{
				return;
			}

			if(FileUtility.IsFileExist(assetPath))
			{
				CommonUtility.DisplayError(new NullReferenceException($"{assetPath} is exist."));

				return;
			}

			var script = selection as MonoScript;
			var asset = ScriptableObject.CreateInstance(script.GetClass());

			CommonUtility.CreateAsset(assetPath,asset,true);
		}

		[MenuItem("Assets/KZSubMenu/Script/Create ScriptableObject",true,MenuOrder.Asset.SCRIPT)]
		private static bool _IsCreateAbleScriptableObject()
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
		[MenuItem("Assets/KZSubMenu/Texture/Open Texture",false,MenuOrder.Asset.TEXTURE)]
		private static void _OnOpenTexture()
		{
			var viewer = EditorWindow.GetWindow<TextureWindow>("Viewer");

			viewer.SetResource(Selection.activeObject as Texture2D);
			viewer.Show();
		}

		[MenuItem("Assets/KZSubMenu/Texture/Open Texture",true,MenuOrder.Asset.TEXTURE)]
		private static bool _IsOpenAbleTexture()
		{
			return Selection.activeObject as Texture2D;
		}
		#endregion Texture

		#region ScriptableObject
		[MenuItem("Assets/KZSubMenu/ScriptableObject/Open ScriptableObject",false,MenuOrder.Asset.SCRIPTABLE_OBJECT)]
		private static void _OnOpenScriptableObject()
		{
			var viewer = EditorWindow.GetWindow<ScriptableObjectWindow>("Viewer");

			viewer.SetResource(Selection.activeObject as ScriptableObject);
			viewer.Show();
		}

		[MenuItem("Assets/KZSubMenu/ScriptableObject/Open ScriptableObject",true,MenuOrder.Asset.SCRIPTABLE_OBJECT)]
		private static bool _IsOpenAbleScriptableObject()
		{
			return Selection.activeObject is ScriptableObject;
		}
		#endregion ScriptableObject
	}
}
#endif