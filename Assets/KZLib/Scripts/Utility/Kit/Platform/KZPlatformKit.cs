/// <summary>
/// Utility methods for platform-specific clipboard operations.
/// </summary>
public static class KZPlatformKit
{
	/// <summary>
	/// Copies the given text to the system clipboard.
	/// Uses EditorGUIUtility in the editor and GUIUtility at runtime.
	/// </summary>
	public static void CopyToClipBoard(string text)
	{
#if UNITY_EDITOR
		UnityEditor.EditorGUIUtility.systemCopyBuffer = text;
#else
		UnityEngine.GUIUtility.systemCopyBuffer = text;
#endif
	}
}
