#if UNITY_EDITOR
using KZLib.Utilities;

public static partial class CommonUtility
{
	public static string FindTemplateText(string absoluteFilePath)
	{
		if(!KZFileKit.IsPathExist(absoluteFilePath))
		{
			return null;
		}

		return KZFileKit.ReadFileToText(FindTemplateFileAbsolutePath(absoluteFilePath));
	}
}
#endif