using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Extends Unity <see cref="ContentSizeFitter"/>.
/// Applies preferred/min size from the base fitter, then clamps so this rect does not exceed the parent's available space.
/// </summary>
/// <remarks>
/// Layout rules:
/// <list type="bullet">
/// <item><description>No parent <see cref="LayoutGroup"/>: clamp to the parent's full rect on that axis.</description></item>
/// <item><description><see cref="VerticalLayoutGroup"/> child: horizontal clamp uses inner width (parent minus padding).</description></item>
/// <item><description><see cref="HorizontalLayoutGroup"/> child: vertical clamp uses inner height (parent minus padding).</description></item>
/// <item><description>Flow axis (HLG horizontal / VLG vertical): also subtracts active siblings' preferred sizes and spacing.</description></item>
/// <item><description><see cref="GridLayoutGroup"/> parent: clamp is skipped; cell size is owned by the grid.</description></item>
/// </list>
/// Does not control text mesh overflow; configure TMP/Text overflow separately.
/// </remarks>
public class EnchantedContentSizeFitter : ContentSizeFitter
{
	private RectTransform m_currentRootRect = null;
	private RectTransform m_parentRootRect = null;

	protected override void Awake()
	{
		base.Awake();

		m_currentRootRect = transform.GetComponent<RectTransform>();

		_ResolveParentRectTransform();
	}

	protected override void OnTransformParentChanged()
	{
		base.OnTransformParentChanged();

		_ResolveParentRectTransform();
		SetDirty();
	}

	/// <summary>Runs base horizontal fit, then clamps width if <see cref="m_HorizontalFit"/> is not <see cref="FitMode.Unconstrained"/>.</summary>
	public override void SetLayoutHorizontal()
	{
		base.SetLayoutHorizontal();

		if(m_HorizontalFit == FitMode.Unconstrained)
		{
			return;
		}

		_ClampAxis(RectTransform.Axis.Horizontal);
	}

	/// <summary>Runs base vertical fit, then clamps height if <see cref="m_VerticalFit"/> is not <see cref="FitMode.Unconstrained"/>.</summary>
	public override void SetLayoutVertical()
	{
		base.SetLayoutVertical();

		if(m_VerticalFit == FitMode.Unconstrained)
		{
			return;
		}

		_ClampAxis(RectTransform.Axis.Vertical);
	}

	/// <summary>Shrinks this rect on <paramref name="axis"/> when it exceeds the computed parent budget.</summary>
	private void _ClampAxis(RectTransform.Axis axis)
	{
		_ResolveParentRectTransform();

		if(!m_parentRootRect)
		{
			return;
		}

		var maxSize = _GetMaxAvailableSize(axis);

		// Skip when parent is not ready (0) or clamp does not apply (e.g. GridLayoutGroup).
		if(float.IsPositiveInfinity(maxSize) || maxSize <= 0.0f)
		{
			return;
		}

		var currentSize = axis == RectTransform.Axis.Horizontal ? m_currentRootRect.rect.width : m_currentRootRect.rect.height;

		if(currentSize <= maxSize)
		{
			return;
		}

		m_currentRootRect.SetSizeWithCurrentAnchors(axis,maxSize);
	}

	/// <summary>Returns the maximum size this rect may use on <paramref name="axis"/> inside the immediate parent.</summary>
	private float _GetMaxAvailableSize(RectTransform.Axis axis)
	{
		var isHorizontal = axis == RectTransform.Axis.Horizontal;
		var maxSize = isHorizontal ? m_parentRootRect.rect.width : m_parentRootRect.rect.height;

		if(maxSize <= 0.0f)
		{
			return maxSize;
		}

		if(!m_parentRootRect.TryGetComponent<LayoutGroup>(out var layoutGroup))
		{
			return maxSize;
		}

		if(layoutGroup is GridLayoutGroup)
		{
			return float.PositiveInfinity;
		}

		var padding = layoutGroup.padding;
		maxSize -= isHorizontal ? padding.horizontal : padding.vertical;

		// On the layout flow axis, siblings share the same row/column budget.
		if(layoutGroup is HorizontalLayoutGroup horizontalLayoutGroup && isHorizontal)
		{
			maxSize = _SubtractSiblingLayoutSizes(maxSize,horizontalLayoutGroup,axis,horizontalLayoutGroup.spacing);
		}
		else if(layoutGroup is VerticalLayoutGroup verticalLayoutGroup && !isHorizontal)
		{
			maxSize = _SubtractSiblingLayoutSizes(maxSize,verticalLayoutGroup,axis,verticalLayoutGroup.spacing);
		}

		return Mathf.Max(0.0f,maxSize);
	}

	/// <summary>Subtracts other active direct children's preferred sizes and inter-child spacing from <paramref name="available"/>.</summary>
	private float _SubtractSiblingLayoutSizes(float available,LayoutGroup layoutGroup,RectTransform.Axis axis,float spacing)
	{
		var parentTransform = layoutGroup.transform;
		var activeChildCount = 0;

		for(var i=0;i<parentTransform.childCount;i++)
		{
			var child = parentTransform.GetChild(i) as RectTransform;

			if(!child || !child.gameObject.activeInHierarchy)
			{
				continue;
			}

			activeChildCount++;

			if(child == m_currentRootRect)
			{
				continue;
			}

			available -= axis == RectTransform.Axis.Horizontal ? LayoutUtility.GetPreferredWidth(child) : LayoutUtility.GetPreferredHeight(child);
		}

		if(activeChildCount > 1)
		{
			available -= spacing*(activeChildCount-1);
		}

		return available;
	}

	/// <summary>Caches the immediate parent's <see cref="RectTransform"/>.</summary>
	private void _ResolveParentRectTransform()
	{
		var parentTransform = transform.parent;

		m_parentRootRect = parentTransform ? parentTransform.GetComponent<RectTransform>() : null;
	}

	protected override void OnValidate()
	{
		base.OnValidate();

#if UNITY_EDITOR
		if(!m_parentRootRect && transform.parent)
		{
			m_parentRootRect = transform.parent.GetComponent<RectTransform>();
		}

		if(m_parentRootRect && m_parentRootRect.TryGetComponent<GridLayoutGroup>(out _))
		{
			Debug.LogWarning($"{nameof(EnchantedContentSizeFitter)} under {nameof(GridLayoutGroup)} does not clamp; cell size is controlled by the grid.",this);
		}
#endif
	}
}