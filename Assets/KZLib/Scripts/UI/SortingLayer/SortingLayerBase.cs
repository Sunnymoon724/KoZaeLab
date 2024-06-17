using KZLib.KZAttribute;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class SortingLayerBase : BaseComponentUI
{
	protected const string DEFAULT_LAYER = "Default";

	private const int DEFAULT_SORTING_LAYER_ORDER = 1000;
	protected const int SORTING_INTERVAL = 5;

	[SerializeField,HideInInspector]
	protected int m_SortingLayerOrder = 0;
	
	protected SortingLayerBase m_ParentSortingLayer = null;
	private int m_SortingLayerCount = 0;

	private int m_SortingLayerOrderMin = 0;
	private int m_SortingLayerOrderMax = 0;

	private bool IsParentSortingLayerExist => m_ParentSortingLayer;

	[FoldoutGroup("소팅레이어",Order = -20),ShowInInspector,LabelText("로컬 순서"),KZRichText]
	public int LocalSortingLayerOrder => m_SortingLayerOrder;
	[FoldoutGroup("소팅레이어",Order = -20),ShowInInspector,LabelText("부모 순서"),KZRichText,ShowIf(nameof(IsParentSortingLayerExist))]
	protected int ParentSortingLayerOrder => IsParentSortingLayerExist ? m_ParentSortingLayer.SortingLayerOrder : DEFAULT_SORTING_LAYER_ORDER;

    [FoldoutGroup("소팅레이어",Order = -20),ShowInInspector,LabelText("실제 순서"),KZRichText,PropertyTooltip("내 순서 + 부모 순서")]
    public int SortingLayerOrder => IsParentSortingLayerExist ? m_ParentSortingLayer.SortingLayerOrder+m_SortingLayerOrder : DEFAULT_SORTING_LAYER_ORDER+m_SortingLayerOrder;
    [FoldoutGroup("소팅레이어",Order = -20),ShowInInspector,LabelText("순서 범위"),KZRichText]
	protected string LayerRange_Display => string.Format("{0} - {1}",m_SortingLayerOrderMin,m_SortingLayerOrderMax);

	[FoldoutGroup("소팅레이어",Order = -20),ShowInInspector,LabelText("자식 포함 갯수"),KZRichText]
	protected string LayerCount_Display => string.Format("{0} 개",m_SortingLayerCount);

	protected override void Reset()
	{
		base.Reset();

		m_SortingLayerCount = 0;
		
		if(!IsParentSortingLayerExist)
		{
			m_ParentSortingLayer = transform.parent ? transform.parent.GetComponentInParent<SortingLayerBase>() : null;
		}

		if(m_SortingLayerCount != 0)
		{
			return;
		}

		var layerArray = GetComponentsInChildren<SortingLayerBase>(true);

		if(m_SortingLayerCount == layerArray.Length)
		{
			return;
		}

		m_SortingLayerOrderMin = SortingLayerOrder;
		m_SortingLayerOrderMax = SortingLayerOrder;

		for(var i=0;i<layerArray.Length;i++)
		{
			if(m_SortingLayerOrderMin > layerArray[i].SortingLayerOrder)
			{
				m_SortingLayerOrderMin = layerArray[i].SortingLayerOrder;
			}

			if(m_SortingLayerOrderMax < layerArray[i].SortingLayerOrder)
			{
				m_SortingLayerOrderMax = layerArray[i].SortingLayerOrder;
			}
		}

		m_SortingLayerCount = layerArray.Length;
	}
	
	public void SetParentSortingLayer(SortingLayerBase _parent,int? _sortingLayerOrder = null)
	{
		m_ParentSortingLayer = _parent;

		SetSortingLayerOrder(_sortingLayerOrder.HasValue ? _sortingLayerOrder.Value : SORTING_INTERVAL);
	}

	public virtual void SetSortingLayerOrder(int _sortingLayerOrder)
	{
		m_SortingLayerOrder = _sortingLayerOrder;
	}
}