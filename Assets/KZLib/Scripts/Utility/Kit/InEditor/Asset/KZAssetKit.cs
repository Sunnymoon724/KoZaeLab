#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;

using Object = UnityEngine.Object;

public static class KZAssetKit
{
	public static string GetObjectUniqueID(Object target)
	{

		var path = AssetDatabase.GetAssetPath(target);
		var guid = AssetDatabase.AssetPathToGUID(path);

		AssetDatabase.TryGetGUIDAndLocalFileIdentifier(target,out string _, out long localId);

		return $"{guid}_{localId}";
	}

	public static void ExecuteMatchedAssetPath(string filter = null,string[] searchInFolderArray = null,Func<string,int,int,bool> onFunc = null)
	{
		var guidArray = AssetDatabase.FindAssets(filter,searchInFolderArray);
		var totalCnt = guidArray.Length;

		for(var i=0;i<guidArray.Length;i++)
		{
			var assetPath = AssetDatabase.GUIDToAssetPath(guidArray[i]);

			if(!KZFileKit.IsFilePath(assetPath))
			{
				continue;
			}

			var result = onFunc.Invoke(assetPath,i,totalCnt);

			if(!result)
			{
				return;
			}
		}
	}

	public static void ExecuteMatchedAsset<TObject>(string filter = null,string[] searchInFolderArray = null,Func<TObject,int,int,bool> onFunc = null) where TObject : Object
	{
		bool _Execute(string assetPath,int index,int totalCount)
		{
			var asset = AssetDatabase.LoadAssetAtPath<TObject>(assetPath);

			if(!asset)
			{
				return true;
			}

			if(onFunc == null)
			{
				return true;
			}

			return onFunc(asset,index,totalCount);
		}

		ExecuteMatchedAssetPath(filter,searchInFolderArray,_Execute);
	}

	public static void CreateAsset(string path,Object asset,bool isOverride)
	{
		if(path.IsEmpty() || !asset)
		{
			LogChannel.Kit.E($"Path or asset is null {path} or {asset}");

			return;
		}

		var folderPath = KZFileKit.GetParentAbsolutePath(path,true);
		var assetPath = KZFileKit.GetAssetPath(path);

		KZFileKit.CreateFolder(folderPath);

		if(KZFileKit.IsFileExist(path))
		{
			if(isOverride)
			{
				AssetDatabase.DeleteAsset(assetPath);
			}
			else
			{
				return;
			}
		}

		AssetDatabase.CreateAsset(asset,assetPath);

		EditorUtility.SetDirty(asset);
		SaveAsset();

		LogChannel.Kit.I($"{asset.name} is saved in {path}.");
	}

	public static void SaveAsset()
	{
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}

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

		foreach(var asset in FindAssetGroupInFolder<TObject>(folderPath))
		{
			list.Add(asset);
		}

		return list.ToArray();
	}
}
#endif