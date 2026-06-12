using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Extension methods for <see cref="LayoutGroup"/> layout rebuild and dirty marking.
/// </summary>
public static class LayoutGroupExtension
{
	/// <summary>
	/// Forces an immediate layout rebuild; when <paramref name="isRecursive"/> is true, rebuilds descendants first.
	/// </summary>
	public static void ForceRebuild(this LayoutGroup layoutGroup,bool isRecursive = true)
	{
		if(!_IsValid(layoutGroup))
		{
			return;
		}

		var rectTrans = layoutGroup.transform as RectTransform;

		if(!rectTrans)
		{
			LogChannel.Kit.E("LayoutGroup transform is not a RectTransform.");

			return;
		}

		if(isRecursive)
		{
			_RecursiveLayoutRebuild(rectTrans);
		}
		else
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(rectTrans);
		}
	}

	/// <summary>
	/// Marks layout dirty for rebuild on the next canvas update; when <paramref name="isRecursive"/> is true, marks descendants first.
	/// </summary>
	public static void MarkForRebuild(this LayoutGroup layoutGroup,bool isRecursive = true)
	{
		if(!_IsValid(layoutGroup))
		{
			return;
		}

		var rectTrans = layoutGroup.transform as RectTransform;

		if(!rectTrans)
		{
			LogChannel.Kit.E("LayoutGroup transform is not a RectTransform.");

			return;
		}

		if(isRecursive)
		{
			_RecursiveMarkForRebuild(rectTrans);
		}
		else
		{
			LayoutRebuilder.MarkLayoutForRebuild(rectTrans);
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
			LogChannel.Kit.E("LayoutGroup is null");

			return false;
		}

		return true;
	}
}
