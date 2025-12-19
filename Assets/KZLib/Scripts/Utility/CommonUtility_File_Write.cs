#if UNITY_EDITOR
using KZLib.KZUtility;

public static partial class CommonUtility
{
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