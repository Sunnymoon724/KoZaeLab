#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;

public static partial class CommonUtility
{
	/// <param name="path">The absolute path of the file or folder.</param>
	public static void Open(string path)
	{
		if(!IsPathExist(path,true))
		{
			return;
		}

		EditorUtility.OpenWithDefaultApp(path);
	}

	/// <param name="filePath">The absolute path of the file.</param>
	public static void OpenTextFile(string filePath,int line,bool isCreate = false)
	{
		var result = IsFileExist(filePath,!isCreate);

		if(!result)
		{
			return;
		}

		CreateFile(filePath);

		InternalEditorUtility.OpenFileAtLineExternal(filePath,line);
	}
}
#endif