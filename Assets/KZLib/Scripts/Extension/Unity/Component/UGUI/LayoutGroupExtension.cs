using UnityEngine;
using UnityEngine.UI;

public static class LayoutGroupExtension
{
	public static void ForceRebuild(this LayoutGroup _layoutGroup,bool _recursive = true)
	{
		if(_recursive)
		{
			RecursiveLayoutRebuild(_layoutGroup.transform as RectTransform);
		}
		else
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(_layoutGroup.transform as RectTransform);
		}
	}
	
	public static void MarkForRebuild(this LayoutGroup _layoutGroup,bool _recursive = true)
	{
		if(_recursive)
		{
			RecursiveMarkForRebuild(_layoutGroup.transform as RectTransform);
		}
		else
		{
			LayoutRebuilder.MarkLayoutForRebuild(_layoutGroup.transform as RectTransform);
		}
	}

	private static void RecursiveMarkForRebuild(RectTransform _transform)
	{
		for(var i=0;i<_transform.childCount;i++)
		{
			var rect = _transform.GetChild(i) as RectTransform;

			if(rect)
			{
				RecursiveMarkForRebuild(rect);
			}
		}

		var controller = _transform.GetComponent<ILayoutController>();

		if(controller != null)
		{
			LayoutRebuilder.MarkLayoutForRebuild(_transform);
		}
	}
	
	private static void RecursiveLayoutRebuild(RectTransform _transform)
	{
		for(var i=0;i<_transform.childCount;i++)
		{
			var rect = _transform.GetChild(i) as RectTransform;

			if(rect)
			{
				RecursiveLayoutRebuild(rect);
			}
		}

		
		if(_transform.TryGetComponent<ILayoutController>(out var _))
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(_transform);
		}
	}
}