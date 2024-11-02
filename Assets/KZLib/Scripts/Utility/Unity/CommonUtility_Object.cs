using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

using Object = UnityEngine.Object;

public static partial class CommonUtility
{	
	public static TObject CopyObject<TObject>(TObject _object,Transform _parent = null) where TObject : Object
	{
		var data = Object.Instantiate(_object,_parent);
		data.name = _object.name;

		return data;
	}

	public static void DestroyObject(Object _object)
	{
		if(!_object)
		{
			return;
		}

		if(Application.isPlaying)
		{
			Object.Destroy(_object);
		}
		else
		{
			Object.DestroyImmediate(_object);
		}
	}

	public static byte[] GetBytesFromObject(Object _object)
	{
		if(!_object)
		{
			return null;
		}
		
		var binary = new BinaryFormatter();
		var stream = new MemoryStream();

		binary.Serialize(stream,_object);
		
		return stream.ToArray();
	}

#if UNITY_EDITOR
	public static bool IsExistAsset(string _filter = null,string[] _searchInFolders = null)
	{
		var guidArray = AssetDatabase.FindAssets(_filter,_searchInFolders);

		return !guidArray.IsNullOrEmpty();
	}

	public static IEnumerable<(string,TObject)> LoadAssetDataGroup<TObject>(string _filter = null,string[] _searchInFolders = null) where TObject : Object
	{
		var dataList = new List<(string,TObject)>();

		foreach(var guid in AssetDatabase.FindAssets(_filter,_searchInFolders))
		{
			var path = AssetDatabase.GUIDToAssetPath(guid);
			var asset = AssetDatabase.LoadAssetAtPath<TObject>(AssetDatabase.GUIDToAssetPath(guid));

			if(!path.IsEmpty() && asset != null)
			{
				dataList.Add((path,asset));
			}
		}

		return dataList;
	}

	public static IEnumerable<string> GetAssetPathGroup(string _filter = null,string[] _searchInFolders = null)
	{
		var pathList = new List<string>();

		foreach(var guid in AssetDatabase.FindAssets(_filter,_searchInFolders))
		{
			var path = AssetDatabase.GUIDToAssetPath(guid);

			if(!path.IsEmpty())
			{
				pathList.Add(path);
			}
		}

		return pathList;
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
			LogTag.System.W($"Result is not one. -> Use {guidArray[0]}");
		}

		return AssetDatabase.LoadAssetAtPath<TObject>(AssetDatabase.GUIDToAssetPath(guidArray[0]));
	}

	public static IEnumerable<TObject> LoadAssetGroup<TObject>(string _filter = null,string[] _searchInFolders = null) where TObject : Object
	{
		var assetList = new List<TObject>();

		foreach(var guid in AssetDatabase.FindAssets(_filter,_searchInFolders))
		{
			var asset = AssetDatabase.LoadAssetAtPath<TObject>(AssetDatabase.GUIDToAssetPath(guid));

			if(asset != null)
			{
				assetList.Add(asset);
			}
		}

		return assetList;
	}

	public static IEnumerable<TObject> LoadAssetGroupInFolder<TObject>(string _folderPath) where TObject : Object
	{
		var assetList = new List<TObject>();

		foreach(var path in GetAllFilePathInFolder(GetAbsolutePath(_folderPath,true)))
		{
			var asset = AssetDatabase.LoadAssetAtPath<TObject>(GetAssetsPath(path));

			if(asset != null)
			{
				assetList.Add(asset);
			}
		}

		return assetList;
	}

	public static void SaveAsset(string _dataPath,Object _asset)
	{
		var folderPath = GetParentAbsolutePath(_dataPath,true);
		var assetPath = GetAssetsPath(_dataPath);

		CreateFolder(folderPath);

		if(IsFileExist(_dataPath))
		{
			AssetDatabase.DeleteAsset(assetPath);
		}

		AssetDatabase.CreateAsset(_asset,assetPath);

		EditorUtility.SetDirty(_asset);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();

		LogTag.System.I($"{_asset.name} is saved in {_dataPath}.");
	}
#endif

	// public static bool SetShadowsOffInMeshRenderer(GameObject _object)
	// {
	// 	var rendererArray = _object.GetComponentsInChildren<MeshRenderer>(true);
	// 	var changed = false;

	// 	for(int i=0;i<rendererArray.Length;i++)
	// 	{
	// 		if(rendererArray[i].receiveShadows)
	// 		{
	// 			changed = true;

	// 			rendererArray[i].receiveShadows = false;
	// 			rendererArray[i].shadowCastingMode = ShadowCastingMode.Off;
	// 		}
	// 	}

	// 	return changed;
	// }
}