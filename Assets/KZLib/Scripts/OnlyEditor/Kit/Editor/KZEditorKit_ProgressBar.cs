#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static partial class KZEditorKit
{
	public static bool DisplayCancelableProgressBar(string title,string info,int current,int total)
	{
		if(total <= 0)
		{
			LogChannel.Kit.W("DisplayCancelableProgressBar: total must be greater than zero.");

			return false;
		}

		return DisplayCancelableProgressBar(title,info,Mathf.Clamp01(current/(float)total));
	}

	public static bool DisplayCancelableProgressBar(string title,string info,float progress)
	{
		return EditorUtility.DisplayCancelableProgressBar(title,info,progress);
	}

	public static void ClearProgressBar()
	{
		EditorUtility.ClearProgressBar();
	}
}
#endif