#if UNITY_EDITOR

using UnityEditor;

#endif

public static partial class CommonUtility
{
	public static void CopyToClipBoard(string _text)
	{
#if UNITY_EDITOR
		EditorGUIUtility.systemCopyBuffer = _text;
#else
		GUIUtility.systemCopyBuffer = _text;
#endif
	}
}