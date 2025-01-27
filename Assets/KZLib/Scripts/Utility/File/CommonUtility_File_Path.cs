using System.IO;
using System.Text.RegularExpressions;

public static partial class CommonUtility
{
	/// <summary>
	/// It is based on the Assets folder.
	/// </summary>
	public static string GetAbsolutePath(string path,bool isIncludeAssets)
	{
		if(!IsPathExist(path,false))
		{
			return null;
		}

		//? ex) @"C:~"
		if(Path.IsPathRooted(path))
		{
			return NormalizePath(path);
		}
		else if(isIncludeAssets)
		{
			//? Change AssetsPath
			return NormalizePath(Path.GetFullPath(GetAssetsPath(path)));
		}
		else
		{
			return NormalizePath(Path.GetFullPath(path,Global.PROJECT_PARENT_PATH));
		}
	}

	public static string NormalizePath(string path)
	{
		return path.Replace('/',Path.DirectorySeparatorChar).Replace('\\',Path.DirectorySeparatorChar);
	}

	/// <summary>
	/// File : name+extension / Folder : name
	/// </summary>
	public static string GetFileName(string path)
	{
		return IsPathExist(path,true) ? Path.GetFileName(path) : null;
	}

	public static string GetOnlyName(string path)
	{
		return IsPathExist(path,true) ? Path.GetFileNameWithoutExtension(path) : null;
	}

	public static string GetExtension(string path)
	{
		return IsPathExist(path,true) ? Path.GetExtension(path) : null;
	}

	public static string GetParentPath(string path)
	{
		return IsPathExist(path,true) ? Path.GetDirectoryName(path) : null;
	}

	/// <summary>
	/// Remove extension from path
	/// </summary>
	public static string GetPathWithoutExtension(string path)
	{
		return IsPathExist(path,true) ? Regex.Replace(path,@"\.[^.]*$","") : null;
	}

	public static string ChangeExtension(string path,string extension)
	{
		return IsPathExist(path,true) ? Path.ChangeExtension(path,extension) : null;
	}

	public static string GetParentAbsolutePath(string path,bool isIncludeAssets)
	{
		return IsPathExist(path,true) ? GetAbsolutePath(GetParentPath(path),isIncludeAssets) : null;
	}

	/// <summary>
	/// Assets/... -> ... (Local Path)
	/// </summary>
	public static string GetLocalPath(string path)
	{
		if(!IsPathExist(path,false))
		{
			return null;
		}

		return IsIncludeAssetsHeader(path) ? RemoveAssetsHeader(path) : NormalizePath(path);
	}

	public static string GetAssetsPath(string path)
	{
		if(!IsPathExist(path,false))
		{
			return null;
		}

		return Path.Combine(Global.ASSETS_HEADER,GetLocalPath(path));
	}

	public static bool IsIncludeAssetsHeader(string path)
	{
		if(!IsPathExist(path,false))
		{
			return false;
		}

		return path.Contains(Global.ASSETS_HEADER);
	}

	public static bool IsStartWithAssetsHeader(string path)
	{
		if(!IsPathExist(path,false))
		{
			return false;
		}

		return path.StartsWith(Global.ASSETS_HEADER);
	}

	public static bool IsFilePath(string filePath)
	{
		if(!IsPathExist(filePath,false))
		{
			return false;
		}

		return Path.HasExtension(filePath);
	}

	public static string[] GetFilePathArray(string folderPath,string pattern = null)
	{
		if(!IsPathExist(folderPath,false))
		{
			return null;
		}

		return pattern == null ? Directory.GetFiles(folderPath) : Directory.GetFiles(folderPath,pattern);
	}

	public static string[] GetFolderPathArray(string folderPath,string pattern = null)
	{
		if(!IsPathExist(folderPath,false))
		{
			return null;
		}

		return pattern == null ? Directory.GetDirectories(folderPath) : Directory.GetDirectories(folderPath,pattern);
	}

	private static bool IsPathExist(string path,bool showError = false)
	{
		if(path.IsEmpty())
		{
			if(showError)
			{
				LogTag.System.E("Path is null");
			}

			return false;
		}

		return true;
	}

	/// <param name="filePath">The absolute file path.</param>
	public static bool IsFileExist(string filePath,bool showError = false)
	{
		if(!IsPathExist(filePath,showError))
		{
			return false;
		}

		var result = File.Exists(filePath);

		if(!result && showError)
		{
			LogTag.System.E($"{filePath} is not file path");

			return false;
		}

		return result;
	}

	/// <param name="filePath">The absolute folder path.</param>
	public static bool IsFolderExist(string folderPath,bool showError = false)
	{
		if(!IsPathExist(folderPath,showError))
		{
			return false;
		}

		var result = Directory.Exists(folderPath);

		if(!result && showError)
		{
			LogTag.System.E($"{folderPath} is not folder path");

			return false;
		}

		return result;
	}

	public static string RemoveTextInPath(string path,string text)
	{
		if(!IsPathExist(path,true))
		{
			return path;
		}

		if(text.IsEmpty())
		{
			return path;
		}

		var normalizePath = NormalizePath(path);
		var textPath = NormalizePath(text);

		var index = normalizePath.IndexOf(textPath);

		if(index == Global.INVALID_INDEX)
		{
			return path;
		}

		int startIndex = index+textPath.Length;

		if(startIndex < normalizePath.Length && (normalizePath[startIndex] == '/'))
		{
			startIndex++;
		}

		return normalizePath[startIndex..];
	}

	public static string RemoveAssetsHeader(string path)
	{
		if(!IsPathExist(path,true))
		{
			return null;
		}

		return RemoveTextInPath(path,Global.ASSETS_HEADER);
	}

	private static string GetUniquePath(string path)
	{
		if(!IsPathExist(path,true))
		{
			return null;
		}

		var directory = GetParentPath(path);
		var name = GetOnlyName(path);
		var extension = GetExtension(path);

		var count = 1;
		var newPath = path;

		while(IsFileExist(newPath))
		{
			newPath = Path.Combine(directory,$"{name} ({count}){extension}");
			count++;
		}

		return newPath;
	}
}