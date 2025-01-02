#if UNITY_EDITOR

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
}