#if UNITY_EDITOR
using System;
using System.IO;
using KZLib.KZUtility;
using UnityEditor;

public static partial class CommonUtility
{
	/// <param name="folderPath">The absolute path of the folder.</param>
	[Obsolete("dummy check")]
	public static void AddOrUpdateTemplateText(string folderPath,string templateName,string scriptName,string newData,Func<string,string> onUpdate)
	{
		LogSvc.Editor.E("사용??");

		var absolutePath = FileUtility.GetAbsolutePath(Path.Combine(folderPath,scriptName),true);
		var templateText = FileUtility.IsFolderExist(absolutePath) ? FileUtility.ReadFileToText(absolutePath) : FindTemplateText(templateName);

		if(templateText.IsEmpty() || templateText.Contains(newData))
		{
			return;
		}

		templateText = onUpdate(templateText);

		FileUtility.WriteTextToFile(absolutePath,templateText);

		AssetDatabase.Refresh();
	}

	public static string FindTemplateText(string absoluteFilePath)
	{
		if(!FileUtility.IsPathExist(absoluteFilePath))
		{
			return null;
		}

		return FileUtility.ReadFileToText(FindTemplateFileAbsolutePath(absoluteFilePath));
	}
}
#endif