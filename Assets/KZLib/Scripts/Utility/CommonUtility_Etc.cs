#if UNITY_EDITOR

using UnityEditor;

#else

using UnityEngine;

#endif

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

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