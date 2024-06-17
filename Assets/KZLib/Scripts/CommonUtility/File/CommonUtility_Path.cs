using System;
using System.IO;
using UnityEngine;

public static partial class CommonUtility
{
	/// <summary>
	/// 모든 경로 합침
	/// </summary>
	public static string PathCombine(params string[] _pathArray)
	{
		return Path.Combine(_pathArray).Replace("\\","/");
	}

	/// <summary>
	/// 실제 경로를 반환
	/// </summary>
	public static string GetFullPath(string _path)
	{
		return IsStartWithAssetsHeader(_path) ? _path : PathCombine(Application.dataPath,_path);
	}

	/// <summary>
	/// 파일 이름 + 확장자명 반환
	/// </summary>
	public static string GetFileName(string _filePath)
	{
		return IsFilePath(_filePath) ? Path.GetFileName(_filePath) : string.Empty;
	}

	/// <summary>
	/// 파일 or 폴더 이름만 반환
	/// </summary>
	public static string GetOnlyName(string _path)
	{
		return IsFilePath(_path) ? Path.GetFileNameWithoutExtension(_path) : Path.GetDirectoryName(_path);
	}

	/// <summary>
	/// 확장자명 반환
	/// </summary>
	public static string GetExtension(string _filePath)
	{
		return IsFilePath(_filePath) ? Path.GetExtension(_filePath) : string.Empty;
	}

	/// <summary>
	/// 현재 경로를 반환
	/// </summary>
	public static string GetPathWithoutExtension(string _path)
	{
		return PathCombine(Path.GetDirectoryName(_path),Path.GetFileNameWithoutExtension(_path));
	}

	/// <summary>
	/// 부모 경로 반환
	/// </summary>
	public static string GetParentPath(string _path)
	{
		return Directory.GetParent(GetFullPath(_path)).FullName;
	}

	/// <summary>
	/// 프로젝트의 상위 경로 반환
	/// </summary>
	public static string GetProjectParentPath()
	{
		return Directory.GetParent(Application.dataPath).Parent.FullName;
	}

	/// <summary>
	/// 프로젝트의 경로 반환
	/// </summary>
	public static string GetProjectPath()
	{
		return Directory.GetParent(Application.dataPath).FullName;
	}

	/// <summary>
	/// 프로젝트 밖에 있는 파일(폴더)의 절대 경로 반환
	/// </summary>
	public static string GetAbsoluteFullPath(string _path)
	{
		if(_path.IsEmpty())
		{
			return null;
		}

		return PathCombine(GetProjectParentPath(),_path);
	}

	/// <summary>
	/// Assets에서부터 시작하는 경로를 반환
	/// </summary>
	public static string GetAssetsPath(string _path)
	{
		//? 이미 있으므로 생략
		if(IsStartWithAssetsHeader(_path))
		{
			return _path;
		}

		//? Assets이 포함되어 있으므로 우선 Assets이전까지 제거
		if(_path.Contains(Global.ASSETS_HEADER))
		{
			_path = RemoveAssetsHeader(_path);
		}

		return PathCombine(Global.ASSETS_HEADER,_path);
	}

	/// <summary>
	/// Assets 밑의 로컬 경로를 반환
	/// </summary>
	public static string GetLocalPath(string _path)
	{
		return IsStartWithAssetsHeader(_path) ? RemoveAssetsHeader(_path) : _path;
	}

	public static bool IsIncludeAssetsHeader(string _path)
	{
		return _path.Contains(Global.ASSETS_HEADER);
	}

	public static bool IsStartWithAssetsHeader(string _path)
	{
		return _path.StartsWith(Global.ASSETS_HEADER);
	}

	/// <summary>
	/// 경로가 폴더인지 파일인지 파악
	/// </summary>
	public static bool IsFilePath(string _path)
	{
		return Path.HasExtension(_path);
	}

	/// <summary>
	/// 이 경로가 파일 경로인지 파악 (빌드 시 경로 파악이 어려울 수 있으므로 Empty 정도만 파악)
	/// </summary>
	public static bool IsExistFile(string _path,bool _needException = false)
	{
		if(_path.IsEmpty())
		{
			if(_needException)
			{
				throw new NullReferenceException("파일의 경로가 null 입니다.");
			}

			return false;
		}

#if UNITY_EDITOR
		var fullPath = GetFullPath(_path);

		if(!File.Exists(fullPath))
		{
			if(_needException)
			{
				throw new NullReferenceException(string.Format("파일이 존재하지 않습니다. 경로 : {0}",fullPath));
			}

			return false;
		}
#endif
		return true;
	}

	public static bool IsExistFolder(string _path,bool _needException = false)
	{
		if(_path.IsEmpty())
		{
			if(_needException)
			{
				throw new NullReferenceException("폴더의 경로가 null 입니다.");
			}
			
			return false;
		}

#if UNITY_EDITOR
		var fullPath = GetFullPath(_path);

		if(!Directory.Exists(fullPath))
		{
			if(_needException)
			{
				throw new NullReferenceException(string.Format("폴더가 존재하지 않습니다. 경로 : {0}",fullPath));
			}

			return false;
		}
#endif
		return true;
	}

	/// <summary>
	/// header를 제거한 파일 경로를 반환함.
	/// </summary>
	public static string RemoveHeaderDirectory(string _path,string _header)
	{
		var path = _path.Replace("\\","/");
		var header = _header.Replace("\\","/");

		return path[(path.IndexOf(header)+header.Length+1)..];
	}

	/// <summary>
	/// header를 Assets을 사용.
	/// </summary>
	public static string RemoveAssetsHeader(string _path)
	{
		return RemoveHeaderDirectory(_path,Global.ASSETS_HEADER);
	}
}