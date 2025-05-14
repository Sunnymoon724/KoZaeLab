using UnityEngine;
using UnityEngine.UI;

public static class LayoutGroupExtension
{
	public static void ForceRebuild(this LayoutGroup layoutGroup,bool isRecursive = true)
	{
		if(!_IsValid(layoutGroup))
		{
			return;
		}

		if(isRecursive)
		{
			_RecursiveLayoutRebuild(layoutGroup.transform as RectTransform);
		}
		else
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.transform as RectTransform);
		}
	}

	public static void MarkForRebuild(this LayoutGroup layoutGroup,bool isRecursive = true)
	{
		if(!_IsValid(layoutGroup))
		{
			return;
		}

		if(isRecursive)
		{
			_RecursiveMarkForRebuild(layoutGroup.transform as RectTransform);
		}
		else
		{
			LayoutRebuilder.MarkLayoutForRebuild(layoutGroup.transform as RectTransform);
		}
	}

	private static void _RecursiveMarkForRebuild(RectTransform rectTransform)
	{
		for(var i=0;i<rectTransform.childCount;i++)
		{
			var childRectTransform = rectTransform.GetChild(i) as RectTransform;

			if(childRectTransform)
			{
				_RecursiveMarkForRebuild(childRectTransform);
			}
		}

		LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
	}

	private static void _RecursiveLayoutRebuild(RectTransform rectTransform)
	{
		for(var i=0;i<rectTransform.childCount;i++)
		{
			var childRectTransform = rectTransform.GetChild(i) as RectTransform;

			if(childRectTransform)
			{
				_RecursiveLayoutRebuild(childRectTransform);
			}
		}

		LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
	}

	private static bool _IsValid(LayoutGroup layoutGroup)
	{
		if(!layoutGroup)
		{
			KZLogType.System.E("LayoutGroup is null");

			return false;
		}

		return true;
	}
}