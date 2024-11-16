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

	[FoldoutGroup("Sorting Layer",Order = -20),ShowInInspector,LabelText("Local Order"),KZRichText]
	public int LocalSortingLayerOrder => m_SortingLayerOrder;
	[FoldoutGroup("Sorting Layer",Order = -20),ShowInInspector,LabelText("Parent Order"),KZRichText,ShowIf(nameof(IsParentSortingLayerExist))]
	protected int ParentSortingLayerOrder => IsParentSortingLayerExist ? m_ParentSortingLayer.SortingLayerOrder : DEFAULT_SORTING_LAYER_ORDER;

    [FoldoutGroup("Sorting Layer",Order = -20),ShowInInspector,LabelText("Current Order"),KZRichText,PropertyTooltip("My Order + Parent Order")]
    public int SortingLayerOrder => IsParentSortingLayerExist ? m_ParentSortingLayer.SortingLayerOrder+m_SortingLayerOrder : DEFAULT_SORTING_LAYER_ORDER+m_SortingLayerOrder;
    [FoldoutGroup("Sorting Layer",Order = -20),ShowInInspector,LabelText("Order Range"),KZRichText]
	protected string LayerRange_Display => $"{m_SortingLayerOrderMin} - {m_SortingLayerOrderMax}";

	[FoldoutGroup("Sorting Layer",Order = -20),ShowInInspector,LabelText("Layer Count"),KZRichText]
	protected string LayerCount_Display => $"{m_SortingLayerCount}";

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

		foreach(var layer in layerArray)
		{
			if(m_SortingLayerOrderMin > layer.SortingLayerOrder)
			{
				m_SortingLayerOrderMin = layer.SortingLayerOrder;
			}

			if(m_SortingLayerOrderMax < layer.SortingLayerOrder)
			{
				m_SortingLayerOrderMax = layer.SortingLayerOrder;
			}
		}

		m_SortingLayerCount = layerArray.Length;
	}
	
	public void SetParentSortingLayer(SortingLayerBase _parent,int? _sortingLayerOrder = null)
	{
		m_ParentSortingLayer = _parent;

		SetSortingLayerOrder(_sortingLayerOrder ?? SORTING_INTERVAL);
	}

	public virtual void SetSortingLayerOrder(int _sortingLayerOrder)
	{
		m_SortingLayerOrder = _sortingLayerOrder;
	}
}