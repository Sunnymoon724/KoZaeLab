#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;

using Object = UnityEngine.Object;

public static partial class CommonUtility
{
	public static bool IsExistAsset(string _filter = null,string[] _searchInFolders = null)
	{
		var guidArray = GetAssetGuidArray(_filter,_searchInFolders);

		return !guidArray.IsNullOrEmpty();
	}

	public static TObject LoadAsset<TObject>(string _filter = null,string[] _searchInFolders = null) where TObject : Object
	{
		var guidArray = GetAssetGuidArray(_filter,_searchInFolders);

		if(guidArray.Length == 0)
		{
			return null;
		}

		if(guidArray.Length > 1)
		{
			LogTag.System.W("검색된 결과가 2개 이상입니다. [{0} 사용]",guidArray[0]);
		}

		return AssetDatabase.LoadAssetAtPath<TObject>(AssetDatabase.GUIDToAssetPath(guidArray[0]));
	}

	public static IEnumerable<string> GetAssetPathGroup(string _filter = null,string[] _searchInFolders = null)
	{
		return ToConvert(GetAssetGuidArray(_filter,_searchInFolders),(guid)=>
		{
			return AssetDatabase.GUIDToAssetPath(guid);
		});
	}

	public static IEnumerable<(string,TObject)> LoadAssetDataGroup<TObject>(string _filter = null,string[] _searchInFolders = null) where TObject : Object
	{
		return ToConvert(GetAssetGuidArray(_filter,_searchInFolders),(guid)=>
		{
			var assetPath = AssetDatabase.GUIDToAssetPath(guid);

			return (assetPath,AssetDatabase.LoadAssetAtPath<TObject>(assetPath));
		});
	}

	public static IEnumerable<TObject> LoadAssetGroup<TObject>(string _filter = null,string[] _searchInFolders = null) where TObject : Object
	{
		return ToConvert(GetAssetGuidArray(_filter,_searchInFolders),(guid)=>
		{
			return AssetDatabase.LoadAssetAtPath<TObject>(AssetDatabase.GUIDToAssetPath(guid));
		});
	}

	public static IEnumerable<TObject> LoadAssetGroupInFolder<TObject>(string _folderPath) where TObject : Object
	{
		return ToConvert(FileUtility.GetAllFilePathInFolder(FileUtility.GetFullPath(_folderPath)),(path)=>
		{
			return AssetDatabase.LoadAssetAtPath<TObject>(FileUtility.GetAssetsPath(path));
		});
	}

	public static void SaveAsset(string _dataPath,Object _asset)
	{
		var folderPath = FileUtility.GetParentAbsolutePath(_dataPath);
		var assetPath = FileUtility.GetAssetsPath(_dataPath);

		FileUtility.CreateFolder(folderPath);

		if(FileUtility.IsExistFile(_dataPath))
		{
			AssetDatabase.DeleteAsset(assetPath);
		}

		AssetDatabase.CreateAsset(_asset,assetPath);

		EditorUtility.SetDirty(_asset);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();

		LogTag.System.I("{0}가 {1}에 저장 되었습니다.",_asset.name,_dataPath);
	}

	private static string[] GetAssetGuidArray(string _filter = null,string[] _searchInFolders = null)
	{
		return AssetDatabase.FindAssets(_filter,_searchInFolders);
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