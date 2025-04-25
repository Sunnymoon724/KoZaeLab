#if UNITY_EDITOR

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;

#endif

public static partial class CommonUtility
{
	public static void CopyToClipBoard(string text)
	{
#if UNITY_EDITOR
		EditorGUIUtility.systemCopyBuffer = text;
#else
		GUIUtility.systemCopyBuffer = text;
#endif
	}

	public static string PrettifyJson(string text)
	{
		try
		{
			var parsedText = JToken.Parse(text);

			return parsedText.ToString(Formatting.Indented);
		}
		catch
		{
			return text;
		}
	}
}