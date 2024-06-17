#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;

using Object = UnityEngine.Object;

public static partial class CommonUtility
{
	public static IEnumerable<string> GetAssetPathGroup(string _filter = null,string[] _searchInFolders = null)
	{
		return ToConvert(AssetDatabase.FindAssets(_filter,_searchInFolders),(guid)=>
		{
			return AssetDatabase.GUIDToAssetPath(guid);
		});
	}

	public static IEnumerable<(string,TObject)> LoadAssetDataGroup<TObject>(string _filter = null,string[] _searchInFolders = null) where TObject : Object
	{
		return ToConvert(AssetDatabase.FindAssets(_filter,_searchInFolders),(guid)=>
		{
			var assetPath = AssetDatabase.GUIDToAssetPath(guid);

			return (assetPath,AssetDatabase.LoadAssetAtPath<TObject>(assetPath));
		});
	}

	public static IEnumerable<TObject> LoadAssetGroup<TObject>(string _filter = null,string[] _searchInFolders = null) where TObject : Object
	{
		return ToConvert(AssetDatabase.FindAssets(_filter,_searchInFolders),(guid)=>
		{
			return AssetDatabase.LoadAssetAtPath<TObject>(AssetDatabase.GUIDToAssetPath(guid));
		});
	}

	public static IEnumerable<TObject> LoadAssetGroupInFolder<TObject>(string _folderPath) where TObject : Object
	{
		return ToConvert(GetAllFilePathInFolder(GetFullPath(_folderPath)),(path)=>
		{
			return AssetDatabase.LoadAssetAtPath<TObject>(GetAssetsPath(path));
		});
	}

	public static TObject LoadAsset<TObject>(string _filter = null,string[] _searchInFolders = null) where TObject : Object
	{
		var guidArray = AssetDatabase.FindAssets(_filter,_searchInFolders);

		if(guidArray.Length == 0)
		{
			return null;
		}

		if(guidArray.Length > 1)
		{
			Log.System.W("검색된 결과가 2개 이상입니다.");
		}

		return AssetDatabase.LoadAssetAtPath<TObject>(AssetDatabase.GUIDToAssetPath(guidArray[0]));
	}

	public static void SaveAsset(string _dataPath,Object _asset)
	{
		var folderPath = GetParentPath(_dataPath);
		var assetPath = GetAssetsPath(_dataPath);

		CreateFolder(folderPath);

		if(IsExistFile(_dataPath))
		{
			AssetDatabase.DeleteAsset(assetPath);
		}

		AssetDatabase.CreateAsset(_asset,assetPath);

		EditorUtility.SetDirty(_asset);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();

		Log.System.I("{0}가 {1}에 저장 되었습니다.",_asset.name,_dataPath);
	}

	private static IEnumerable<TResult> ToConvert<TResult>(IEnumerable<string> _pathGroup,Func<string,TResult> _onConvert)
	{
		var resultList = new List<TResult>();

		foreach(var path in _pathGroup)
		{
			var result = _onConvert(path);

			if(result != null)
			{
				resultList.Add(result);
			}
		}

		return resultList;
	}
}
#endif