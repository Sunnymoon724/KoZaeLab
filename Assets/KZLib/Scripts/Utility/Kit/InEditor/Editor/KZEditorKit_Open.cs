#if UNITY_EDITOR
using System.Diagnostics;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;

public static partial class KZEditorKit
{
	public static void Open(string absolutePath)
	{
		if(!KZFileKit.IsPathExist(absolutePath))
		{
			return;
		}

		EditorUtility.OpenWithDefaultApp(absolutePath);
	}

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

	public static void OpenTextFile(string absoluteFilePath,int line = 1)
	{
		if(!KZFileKit.IsFileExist(absoluteFilePath))
		{
			return;
		}

		line = line <= 0 ? 1 : line;

		InternalEditorUtility.OpenFileAtLineExternal(absoluteFilePath,line);
	}

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