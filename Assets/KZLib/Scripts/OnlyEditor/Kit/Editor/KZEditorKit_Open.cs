#if UNITY_EDITOR
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;

/// <summary>
/// Editor-only utility methods for opening external files, scripts, and scenes.
/// </summary>
public static partial class KZEditorKit
{
	/// <summary>
	/// Opens a file with the OS default application when it exists on disk.
	/// </summary>
	public static void Open(string absolutePath)
	{
		if(absolutePath.IsEmpty())
		{
			LogChannel.Kit.W("Open: path is null or empty.");

			return;
		}

		if(!KZFileKit.IsFileExist(absolutePath))
		{
			LogChannel.Kit.W($"Open: file does not exist. {absolutePath}");

			return;
		}

		EditorUtility.OpenWithDefaultApp(absolutePath);
	}

	/// <summary>
	/// Launches a batch or script file via the shell.
	/// </summary>
	public static void OpenBatchFile(string absolutePath)
	{
		if(absolutePath.IsEmpty())
		{
			LogChannel.Kit.W("OpenBatchFile: path is null or empty.");

			return;
		}

		if(!KZFileKit.IsFileExist(absolutePath))
		{
			LogChannel.Kit.W($"OpenBatchFile: file does not exist. {absolutePath}");

			return;
		}

		Process.Start(new ProcessStartInfo
		{
			FileName = absolutePath,
			UseShellExecute = true,
		});
	}

	/// <summary>
	/// Opens a text file in the external editor at the given line.
	/// </summary>
	public static void OpenTextFile(string absoluteFilePath,int line = 1)
	{
		if(absoluteFilePath.IsEmpty())
		{
			LogChannel.Kit.W("OpenTextFile: path is null or empty.");

			return;
		}

		if(!KZFileKit.IsFileExist(absoluteFilePath))
		{
			LogChannel.Kit.W($"OpenTextFile: file does not exist. {absoluteFilePath}");

			return;
		}

		line = line <= 0 ? 1 : line;

		InternalEditorUtility.OpenFileAtLineExternal(absoluteFilePath,line);
	}

	/// <summary>
	/// Finds scenes by exact file name, asks for confirmation, then opens the selected scene in the editor.
	/// </summary>
	public static void OpenSceneInEditor(string sceneName)
	{
		if(sceneName.IsEmpty())
		{
			LogChannel.Kit.W("OpenSceneInEditor: scene name is null or empty.");

			return;
		}

		var matchedPathList = _FindScenePathListByName(sceneName);

		if(matchedPathList.IsNullOrEmpty())
		{
			LogChannel.Kit.W($"OpenSceneInEditor: scene not found. {sceneName}");

			return;
		}

		var scenePath = _PickScenePath(sceneName,matchedPathList);

		if(scenePath.IsEmpty())
		{
			return;
		}

		EditorSceneManager.OpenScene(scenePath);
	}

	private static List<string> _FindScenePathListByName(string sceneName)
	{
		var guidArray = AssetDatabase.FindAssets("t:Scene");
		var matchedPathList = new List<string>();

		for(var i=0;i<guidArray.Length;i++)
		{
			var scenePath = AssetDatabase.GUIDToAssetPath(guidArray[i]);

			if(Path.GetFileNameWithoutExtension(scenePath).IsEqual(sceneName))
			{
				matchedPathList.Add(scenePath);
			}
		}

		return matchedPathList;
	}

	private static string _PickScenePath(string sceneName,List<string> matchedPathList)
	{
		if(matchedPathList.Count == 1)
		{
			return DisplayCheck($"Open {sceneName}",$"Do you want to open the scene?\n{matchedPathList[0]}") ? matchedPathList[0] : null;
		}

		for(var i=0;i<matchedPathList.Count;i++)
		{
			var scenePath = matchedPathList[i];

			if(DisplayCheck($"Open {sceneName}",$"Multiple scenes matched ({i+1}/{matchedPathList.Count}).\n\nOpen this scene?\n{scenePath}"))
			{
				return scenePath;
			}
		}

		return null;
	}
}
#endif
