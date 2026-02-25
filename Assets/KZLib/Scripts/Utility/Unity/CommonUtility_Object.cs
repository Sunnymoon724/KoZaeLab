using System;
using System.Collections.Generic;
using KZLib.Utilities;

#if UNITY_EDITOR

using UnityEditor;

#endif

using Object = UnityEngine.Object;

public static partial class CommonUtility
{
#if UNITY_EDITOR
	public static bool IsExistAsset(string filter = null,string[] searchInFolderArray = null)
	{
		return AssetDatabase.FindAssets(filter,searchInFolderArray).Length > 0;
	}

	public static IEnumerable<TObject> FindAssetGroupInFolder<TObject>(string folderPath) where TObject : Object
	{
		foreach(var path in KZFileKit.FindFilePathGroup(KZFileKit.GetAbsolutePath(folderPath,true)))
		{
			var asset = AssetDatabase.LoadAssetAtPath<TObject>(KZFileKit.GetAssetPath(path));

			if(asset != null)
			{
				yield return asset;
			}
		}
	}

	public static TObject[] FindAssetListInFolder<TObject>(string folderPath) where TObject : Object
	{
		var list = new List<TObject>();

		foreach(var path in KZFileKit.FindFilePathGroup(KZFileKit.GetAbsolutePath(folderPath,true)))
		{
			var asset = AssetDatabase.LoadAssetAtPath<TObject>(KZFileKit.GetAssetPath(path));

			if(asset != null)
			{
				list.Add(asset);
			}
		}

		return list.ToArray();
	}
#endif
}