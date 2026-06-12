#if UNITY_EDITOR
using UnityEditor;

/// <summary>
/// Partial editor utility for miscellaneous editor helpers.
/// </summary>
public static partial class KZEditorKit
{
	#region Copy
	/// <summary>
	/// Copies the given text to the system clipboard.
	/// </summary>
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
