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
		if(!_object)
		{
			LogTag.System.E("Object is null");

			return null;
		}

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
			LogTag.System.E("Object is null");

			return null;
		}

		using var stream = new MemoryStream();
		var binary = new BinaryFormatter();

		binary.Serialize(stream, _object);

		return stream.ToArray();
	}

#if UNITY_EDITOR
	public static bool IsExistAsset(string _filter = null,string[] _searchInFolders = null)
	{
		var guidArray = AssetDatabase.FindAssets(_filter,_searchInFolders);

		return guidArray.Length > 0;
	}

	public static IEnumerable<(string,TObject)> LoadAssetDataGroup<TObject>(string _filter = null,string[] _searchInFolders = null) where TObject : Object
	{
		foreach(var guid in AssetDatabase.FindAssets(_filter,_searchInFolders))
		{
			var path = AssetDatabase.GUIDToAssetPath(guid);
			var asset = AssetDatabase.LoadAssetAtPath<TObject>(AssetDatabase.GUIDToAssetPath(guid));

			if(!path.IsEmpty() && asset != null)
			{
				yield return (path,asset);
			}
		}
	}

	public static IEnumerable<string> GetAssetPathGroup(string _filter = null,string[] _searchInFolders = null)
	{
		foreach(var guid in AssetDatabase.FindAssets(_filter,_searchInFolders))
		{
			var path = AssetDatabase.GUIDToAssetPath(guid);

			if(!path.IsEmpty())
			{
				yield return path;
			}
		}
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
		foreach(var guid in AssetDatabase.FindAssets(_filter,_searchInFolders))
		{
			var asset = AssetDatabase.LoadAssetAtPath<TObject>(AssetDatabase.GUIDToAssetPath(guid));

			if(asset != null)
			{
				yield return asset;
			}
		}
	}

	public static IEnumerable<TObject> LoadAssetGroupInFolder<TObject>(string _folderPath) where TObject : Object
	{
		foreach(var path in GetAllFilePathInFolder(GetAbsolutePath(_folderPath,true)))
		{
			var asset = AssetDatabase.LoadAssetAtPath<TObject>(GetAssetsPath(path));

			if(asset != null)
			{
				yield return asset;
			}
		}
	}

	public static void SaveAsset(string _dataPath,Object _asset)
	{
		if(_dataPath.IsEmpty() || !_asset)
		{
			LogTag.System.E($"Path or asset is null {_dataPath} or {_asset}");

			return;
		}

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
}