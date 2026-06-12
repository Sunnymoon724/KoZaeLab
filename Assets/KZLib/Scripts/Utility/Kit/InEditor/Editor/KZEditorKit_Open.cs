#if UNITY_EDITOR
using System.Diagnostics;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;

/// <summary>
/// Partial editor utility for opening files, batch scripts, and scenes from the editor.
/// </summary>
public static partial class KZEditorKit
{
	/// <summary>
	/// Opens the file at the given absolute path with the system default application.
	/// </summary>
	public static void Open(string absolutePath)
	{
		if(!KZFileKit.IsPathExist(absolutePath))
		{
			return;
		}

		EditorUtility.OpenWithDefaultApp(absolutePath);
	}

	/// <summary>
	/// Launches the batch file at the given absolute path via the shell.
	/// </summary>
	public static void OpenBatchFile(string absolutePath)
	{
		if(!KZFileKit.IsPathExist(absolutePath))
		{
			return;
		}

		Process.Start(new ProcessStartInfo
		{
			FileName = absolutePath,
			UseShellExecute = true,
		});
	}

	/// <summary>
	/// Opens a text file in the external editor at the specified line number.
	/// </summary>
	public static void OpenTextFile(string absoluteFilePath,int line = 1)
	{
		if(!KZFileKit.IsFileExist(absoluteFilePath))
		{
			return;
		}

		line = line <= 0 ? 1 : line;

		InternalEditorUtility.OpenFileAtLineExternal(absoluteFilePath,line);
	}

	/// <summary>
	/// Prompts for confirmation and opens the first scene asset matching the given scene name.
	/// </summary>
	public static void OpenSceneInEditor(string sceneName)
	{
		if(!DisplayCheck($"Open {sceneName}",$"Do you want to open the {sceneName}?"))
		{
			return;
		}

		var guidArray = AssetDatabase.FindAssets($"t:Scene {sceneName}");
		var scenePath = guidArray.Length < 1 ? string.Empty : AssetDatabase.GUIDToAssetPath(guidArray[0]);

		if(scenePath.IsEmpty())
		{
			return;
		}

		EditorSceneManager.OpenScene(scenePath);
	}
}
#endif
