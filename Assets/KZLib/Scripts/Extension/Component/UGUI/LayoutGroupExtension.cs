using UnityEngine;
using UnityEngine.UI;

public static class LayoutGroupExtension
{
	public static void ForceRebuild<TComponent>(this TComponent _layoutGroup,bool _recursive = true) where TComponent : Component,ILayoutController
	{
		if(_recursive)
		{
			RecursiveLayoutRebuild((RectTransform)_layoutGroup.transform);
		}
		else
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)_layoutGroup.transform);
		}
	}
	
	public static void MarkForRebuild<TComponent>(this TComponent _layoutGroup,bool _recursive = true) where TComponent : Component,ILayoutController
	{
		if(_recursive)
		{
			RecursiveMarkForRebuild((RectTransform)_layoutGroup.transform);
		}
		else
		{
			LayoutRebuilder.MarkLayoutForRebuild((RectTransform)_layoutGroup.transform);
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

		var controller = _transform.GetComponent<ILayoutController>();

		if(controller != null)
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(_transform);
		}
	}
}