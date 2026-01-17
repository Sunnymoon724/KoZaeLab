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

	private static void _RecursiveMarkForRebuild(RectTransform rectTrans)
	{
		for(var i=0;i<rectTrans.childCount;i++)
		{
			var childRectTrans = rectTrans.GetChild(i).GetComponent<RectTransform>();

			if(childRectTrans)
			{
				_RecursiveMarkForRebuild(childRectTrans);
			}
		}

		LayoutRebuilder.MarkLayoutForRebuild(rectTrans);
	}

	private static void _RecursiveLayoutRebuild(RectTransform rectTrans)
	{
		for(var i=0;i<rectTrans.childCount;i++)
		{
			var childRectTrans = rectTrans.GetChild(i).GetComponent<RectTransform>();

			if(childRectTrans)
			{
				_RecursiveLayoutRebuild(childRectTrans);
			}
		}

		LayoutRebuilder.ForceRebuildLayoutImmediate(rectTrans);
	}

	private static bool _IsValid(LayoutGroup layoutGroup)
	{
		if(!layoutGroup)
		{
			LogChannel.System.E("LayoutGroup is null");

			return false;
		}

		return true;
	}
}