using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public static partial class FileUtility
{
	/// <summary>
	/// Combine all path
	/// </summary>
	public static string PathCombine(params string[] _pathArray)
	{
		return NormalizePath(Path.Combine(_pathArray));
	}

	/// <summary>
	/// It is based on the Assets folder.
	/// </summary>
	public static string GetAbsolutePath(string _path,bool _isIncludeAssets)
	{
		if(Path.IsPathRooted(_path))
		{
			return NormalizePath(_path);
		}
		else if(_isIncludeAssets)
		{
			//? Change AssetsPath
			return NormalizePath(Path.GetFullPath(GetAssetsPath(_path)));
		}
		else
		{
			return NormalizePath(Path.GetFullPath(_path,GetProjectParentPath()));
		}
	}

	public static string NormalizePath(string _path)
	{
		return _path.Replace("\\","/");
	}

	/// <summary>
	/// File : name+extension / Folder : name
	/// </summary>
	public static string GetFileName(string _filePath)
	{
		return Path.GetFileName(_filePath);
	}

	public static string GetOnlyName(string _path)
	{
		return Path.GetFileNameWithoutExtension(_path);
	}

	public static string GetExtension(string _filePath)
	{
		return Path.GetExtension(_filePath);
	}

	public static string GetParentPath(string _path)
	{
		return Path.GetDirectoryName(_path);
	}

	/// <summary>
	/// Remove extension from path
	/// </summary>
	public static string GetPathWithoutExtension(string _path)
	{
		return Regex.Replace(_path,@"\.[^.]*$","");
	}

	public static string ChangeExtension(string _path,string _extension)
	{
		return Path.ChangeExtension(_path,_extension);
	}

	public static string GetParentAbsolutePath(string _path,bool _isIncludeAssets)
	{
		return GetAbsolutePath(GetParentPath(_path),_isIncludeAssets);
	}

	public static string GetProjectPath()
	{
		var directoryInfo = Directory.GetParent(Application.dataPath);

		return NormalizePath(directoryInfo.FullName);
	}

	public static string GetProjectParentPath()
	{
		var directoryInfo = Directory.GetParent(Application.dataPath);
		var parentDirectoryInfo = Directory.GetParent(directoryInfo.FullName);

		return NormalizePath(parentDirectoryInfo.FullName);
	}

	public static string GetAssetsPath(string _path)
	{
		return IsStartWithAssetsHeader(_path) ? NormalizePath(_path) : PathCombine(Global.ASSETS_HEADER,_path);
	}

	public static string GetLocalPath(string _path)
	{
		return IsStartWithAssetsHeader(_path) ? RemoveAssetsHeader(_path) : NormalizePath(_path);
	}

	public static bool IsIncludeAssetsHeader(string _path)
	{
		return _path.Contains(Global.ASSETS_HEADER);
	}

	public static bool IsStartWithAssetsHeader(string _path)
	{
		return _path.StartsWith(Global.ASSETS_HEADER);
	}

	public static bool IsFilePath(string _filePath)
	{
		return Path.HasExtension(_filePath);
	}

	public static bool IsExist(string _path,bool _needException = false)
	{
		if(_path.IsEmpty())
		{
			if(_needException)
			{
				throw new NullReferenceException("Path is null.");
			}

			return false;
		}

#if UNITY_EDITOR
		//? Check inner path
		var fullPath = GetAbsolutePath(_path,true);

		if(File.Exists(fullPath))
		{
			return true;
		}
		else if(Directory.Exists(fullPath))
		{
			return true;
		}

		//? Check outer path
		fullPath = GetAbsolutePath(_path,false);

		if(File.Exists(fullPath))
		{
			return true;
		}
		else if(Directory.Exists(fullPath))
		{
			return true;
		}

		if(IsFilePath(_path))
		{
			if(_needException)
			{
				throw new FileNotFoundException($"File is not exist. [{fullPath}]");
			}
		}
		else
		{
			if(_needException)
			{
				throw new DirectoryNotFoundException($"Folder is not exist. [{fullPath}]");
			}
		}

		return false;
#else
		return true;
#endif
	}

	public static string RemoveHeaderDirectory(string _path,string _header)
	{
		var path = NormalizePath(_path);
		var header = NormalizePath(_header);

		return path[(path.IndexOf(header)+header.Length+1)..];
	}

	public static string RemoveAssetsHeader(string _path)
	{
		return RemoveHeaderDirectory(_path,Global.ASSETS_HEADER);
	}

	private static string GetUniquePath(string _path)
	{
		var directory = GetParentPath(_path);
		var name = GetOnlyName(_path);
		var extension = GetExtension(_path);

		var count = 1;
		var newPath = _path;

		while(File.Exists(newPath))
		{
			newPath = PathCombine(directory,$"{name} ({count}){extension}");
			count++;
		}

		return newPath;
	}
}