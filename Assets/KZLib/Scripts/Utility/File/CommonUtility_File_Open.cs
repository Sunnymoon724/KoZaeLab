#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditorInternal;

public static partial class CommonUtility
{
	/// <param name="_path">The absolute path of the file or folder.</param>
	public static void Open(string _path)
	{
		if(!_path.IsEmpty())
		{
			EditorUtility.OpenWithDefaultApp(_path);
		}
		else
		{
			new NullReferenceException("Path is null.");
		}
	}

	/// <param name="_path">The absolute path of the file.</param>
	public static void OpenTextFile(string _filePath,int _line,bool _isCreate = false)
	{
		var result = IsFileExist(_filePath,!_isCreate);

		if(!result)
		{
			return;
		}

		CreateFile(_filePath);

		InternalEditorUtility.OpenFileAtLineExternal(_filePath,_line);
	}
}
#endif