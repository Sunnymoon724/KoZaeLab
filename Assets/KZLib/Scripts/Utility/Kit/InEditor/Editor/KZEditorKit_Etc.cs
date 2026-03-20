#if UNITY_EDITOR
using UnityEditor;

public static partial class KZEditorKit
{
	#region Copy
	public static void CopyToClipBoard(string text)
	{
#if UNITY_EDITOR
		EditorGUIUtility.systemCopyBuffer = text;
#else
		GUIUtility.systemCopyBuffer = text;
#endif
	}
	#endregion Copy
}
#endif