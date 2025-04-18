﻿using System.Collections.Generic;
using KZLib.KZUtility;

#if UNITY_EDITOR

using UnityEditor;

#endif

using Object = UnityEngine.Object;

public static partial class CommonUtility
{
#if UNITY_EDITOR
	public static bool IsExistAsset(string filter = null,string[] searchInFolderArray = null)
	{
		return _FindAssetArray(filter,searchInFolderArray).Length > 0;
	}

	public static IEnumerable<string> FindAssetPathGroup(string filter = null,string[] searchInFolderArray = null)
	{
		foreach(var guid in _FindAssetArray(filter,searchInFolderArray))
		{
			var path = AssetDatabase.GUIDToAssetPath(guid);

			if(!path.IsEmpty())
			{
				yield return path;
			}
		}
	}

	public static TObject FindAsset<TObject>(string filter = null,string[] searchInFolderArray = null) where TObject : Object
	{
		var guidArray = _FindAssetArray(filter,searchInFolderArray);

		if(guidArray.Length == 0)
		{
			return null;
		}

		if(guidArray.Length > 1)
		{
			LogTag.System.W($"Result is not one. -> Use {guidArray[0]}");
		}

		return AssetDatabase.LoadAssetAtPath<TObject>(AssetDatabase.GUIDToAssetPath(guidArray[0]));
	}

	public static IEnumerable<TObject> FindAssetGroupInFolder<TObject>(string folderPath) where TObject : Object
	{
		foreach(var path in FileUtility.FindFilePathGroup(FileUtility.GetAbsolutePath(folderPath,true)))
		{
			var asset = AssetDatabase.LoadAssetAtPath<TObject>(FileUtility.GetAssetsPath(path));

			if(asset != null)
			{
				yield return asset;
			}
		}
	}

	private static string[] _FindAssetArray(string filter,string[] searchInFolderArray)
	{
		return AssetDatabase.FindAssets(filter,searchInFolderArray);
	}

	public static void SaveAsset(string dataPath,Object asset)
	{
		if(dataPath.IsEmpty() || !asset)
		{
			LogTag.System.E($"Path or asset is null {dataPath} or {asset}");

			return;
		}

		var folderPath = FileUtility.GetParentAbsolutePath(dataPath,true);
		var assetPath = FileUtility.GetAssetsPath(dataPath);

		FileUtility.CreateFolder(folderPath);

		if(FileUtility.IsFileExist(dataPath))
		{
			AssetDatabase.DeleteAsset(assetPath);
		}

		AssetDatabase.CreateAsset(asset,assetPath);

		EditorUtility.SetDirty(asset);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();

		LogTag.System.I($"{asset.name} is saved in {dataPath}.");
	}
#endif
}