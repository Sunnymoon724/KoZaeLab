#if UNITY_EDITOR
using UnityEditor;

public static partial class KZEditorKit
{
	public static bool DisplayCancelableProgressBar(string title,string info,int current,int total)
	{
		return DisplayCancelableProgressBar(title,info,current/(float)total);
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