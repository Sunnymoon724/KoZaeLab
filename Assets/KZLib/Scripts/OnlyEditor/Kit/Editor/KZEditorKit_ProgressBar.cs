#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// Editor-only utility methods for cancelable progress bar display.
/// </summary>
public static partial class KZEditorKit
{
	/// <summary>
	/// Shows a cancelable progress bar using current and total counts. Returns true when cancelled.
	/// </summary>
	public static bool DisplayCancelableProgressBar(string title,string info,int current,int total)
	{
		if(total <= 0)
		{
			LogChannel.Kit.W("DisplayCancelableProgressBar: total must be greater than zero.");

			return false;
		}

		return DisplayCancelableProgressBar(title,info,Mathf.Clamp01(current/(float)total));
	}

	/// <summary>
	/// Shows a cancelable progress bar. Returns true when the user clicks Cancel.
	/// </summary>
	public static bool DisplayCancelableProgressBar(string title,string info,float progress)
	{
		return EditorUtility.DisplayCancelableProgressBar(title,info,progress);
	}

	/// <summary>
	/// Clears any active editor progress bar.
	/// </summary>
	public static void ClearProgressBar()
	{
		EditorUtility.ClearProgressBar();
	}
}
#endif
