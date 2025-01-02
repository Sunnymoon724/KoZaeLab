using UnityEngine;
using UnityEngine.UI;

public static class LayoutGroupExtension
{
	public static void ForceRebuild(this LayoutGroup layoutGroup,bool isRecursive = true)
	{
		if(!IsValid(layoutGroup))
		{
			return;
		}

		if(isRecursive)
		{
			RecursiveLayoutRebuild(layoutGroup.transform as RectTransform);
		}
		else
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.transform as RectTransform);
		}
	}

	public static void MarkForRebuild(this LayoutGroup layoutGroup,bool isRecursive = true)
	{
		if(!IsValid(layoutGroup))
		{
			return;
		}

		if(isRecursive)
		{
			RecursiveMarkForRebuild(layoutGroup.transform as RectTransform);
		}
		else
		{
			LayoutRebuilder.MarkLayoutForRebuild(layoutGroup.transform as RectTransform);
		}
	}

	private static void RecursiveMarkForRebuild(RectTransform rectTransform)
	{
		for(var i=0;i<rectTransform.childCount;i++)
		{
			var childRectTransform = rectTransform.GetChild(i) as RectTransform;

			if(childRectTransform)
			{
				RecursiveMarkForRebuild(childRectTransform);
			}
		}

		LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
	}

	private static void RecursiveLayoutRebuild(RectTransform rectTransform)
	{
		for(var i=0;i<rectTransform.childCount;i++)
		{
			var childRectTransform = rectTransform.GetChild(i) as RectTransform;

			if(childRectTransform)
			{
				RecursiveLayoutRebuild(childRectTransform);
			}
		}

		LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
	}

	private static bool IsValid(LayoutGroup layoutGroup)
	{
		if(!layoutGroup)
		{
			LogTag.System.E("LayoutGroup is null");

			return false;
		}

		return true;
	}
}