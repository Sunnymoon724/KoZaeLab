using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public static partial class FileUtility
{
	/// <summary>
	/// 모든 경로 합침
	/// </summary>
	public static string PathCombine(params string[] _pathArray)
	{
		return NormalizePath(Path.Combine(_pathArray));
	}

	/// <summary>
	/// Assets 폴더를 기준으로 나온다. (Assets의 외부는 프로젝트 부모 폴더 기준)
	/// </summary>
	public static string GetAbsolutePath(string _path,bool _isIncludeAssets)
	{
		if(Path.IsPathRooted(_path))
		{
			return NormalizePath(_path);
		}
		else if(_isIncludeAssets)
		{
			//? AssetsPath로 변경 후 사용
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
	/// 이름 + 확장자명 반환 (폴더는 이름만)
	/// </summary>
	public static string GetFileName(string _filePath)
	{
		return Path.GetFileName(_filePath);
	}

	/// <summary>
	/// 이름만 반환
	/// </summary>
	public static string GetOnlyName(string _path)
	{
		return Path.GetFileNameWithoutExtension(_path);
	}

	/// <summary>
	/// 확장자명만 반환
	/// </summary>
	public static string GetExtension(string _filePath)
	{
		return Path.GetExtension(_filePath);
	}

	/// <summary>
	/// 부모 경로 반환
	/// </summary>
	public static string GetParentPath(string _path)
	{
		return Path.GetDirectoryName(_path);
	}

	/// <summary>
	/// 현재 경로에서 확장자명을 빼고 반환
	/// </summary>
	public static string GetPathWithoutExtension(string _path)
	{
		return Regex.Replace(_path,@"\.[^.]*$","");
	}

	/// <summary>
	/// 확장자명을 변경함
	/// </summary>
	public static string ChangeExtension(string _path,string _extension)
	{
		return Path.ChangeExtension(_path,_extension);
	}

	/// <summary>
	/// 부모 경로를 절대경로로 반환
	/// </summary>
	public static string GetParentAbsolutePath(string _path,bool _isIncludeAssets)
	{
		return GetAbsolutePath(GetParentPath(_path),_isIncludeAssets);
	}

	/// <summary>
	/// 프로젝트의 경로 반환 (Assets폴더의 부모)
	/// </summary>
	public static string GetProjectPath()
	{
		var directoryInfo = Directory.GetParent(Application.dataPath);

		return NormalizePath(directoryInfo.FullName);
	}

	/// <summary>
	/// 프로젝트의 상위 경로 반환 (Assets폴더의 부모의 부모)
	/// </summary>
	public static string GetProjectParentPath()
	{
		var directoryInfo = Directory.GetParent(Application.dataPath);
		var parentDirectoryInfo = Directory.GetParent(directoryInfo.FullName);

		return NormalizePath(parentDirectoryInfo.FullName);
	}

	/// <summary>
	/// Assets에서부터 시작하는 경로를 반환
	/// </summary>
	public static string GetAssetsPath(string _path)
	{
		return IsStartWithAssetsHeader(_path) ? NormalizePath(_path) : PathCombine(Global.ASSETS_HEADER,_path);
	}

	/// <summary>
	/// Assets 밑의 로컬 경로를 반환
	/// </summary>
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

	/// <summary>
	/// 경로가 폴더인지 파일인지 파악
	/// </summary>
	public static bool IsFilePath(string _filePath)
	{
		return Path.HasExtension(_filePath);
	}

	/// <summary>
	/// 존재 여부 파악 (에디터가 아니면 Empty만 파악)
	/// </summary>
	public static bool IsExist(string _path,bool _needException = false)
	{
		if(_path.IsEmpty())
		{
			if(_needException)
			{
				throw new NullReferenceException("경로가 null 입니다.");
			}

			return false;
		}

#if UNITY_EDITOR
		//? 내부 경로 체크
		var fullPath = GetAbsolutePath(_path,true);

		if(File.Exists(fullPath))
		{
			return true;
		}
		else if(Directory.Exists(fullPath))
		{
			return true;
		}

		//? 외부 경로 체크
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
				throw new FileNotFoundException(string.Format("파일이 존재하지 않습니다. [{0}]",fullPath));
			}
		}
		else
		{
			if(_needException)
			{
				throw new DirectoryNotFoundException(string.Format("폴더가 존재하지 않습니다. [{0}]",fullPath));
			}
		}

		return false;
#else
		return true;
#endif
	}

	/// <summary>
	/// header를 제거한 파일 경로를 반환함.
	/// </summary>
	public static string RemoveHeaderDirectory(string _path,string _header)
	{
		var path = NormalizePath(_path);
		var header = NormalizePath(_header);

		return path[(path.IndexOf(header)+header.Length+1)..];
	}

	/// <summary>
	/// header를 Assets을 사용.
	/// </summary>
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
			newPath = PathCombine(directory,string.Format("{0} ({1}){2}",name,count,extension));
			count++;
		}

		return newPath;
	}
}