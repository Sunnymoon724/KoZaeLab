// using KZLib.Attributes;
// using Sirenix.OdinInspector;
// using UnityEngine;

// public abstract class SortingLayerBase : BaseComponentUI
// {
// 	private const int c_defaultSortingLayerOrder = 1000;
// 	protected const int c_sortingInterval = 5;

// 	[SerializeField,HideInInspector]
// 	protected int m_sortingLayerOrder = 0;
	
// 	protected SortingLayerBase m_parentSortingLayer = null;
// 	private int m_sortingLayerCount = 0;

// 	private int m_sortingLayerOrderMin = 0;
// 	private int m_sortingLayerOrderMax = 0;

// 	private bool IsParentSortingLayerExist => m_parentSortingLayer;

// 	[FoldoutGroup("Sorting Layer",Order = -20),ShowInInspector,LabelText("Local Order"),KZRichText]
// 	public int LocalSortingLayerOrder => m_sortingLayerOrder;
// 	[FoldoutGroup("Sorting Layer",Order = -20),ShowInInspector,LabelText("Parent Order"),KZRichText,ShowIf(nameof(IsParentSortingLayerExist))]
// 	protected int ParentSortingLayerOrder => IsParentSortingLayerExist ? m_parentSortingLayer.SortingLayerOrder : c_defaultSortingLayerOrder;

// 	[FoldoutGroup("Sorting Layer",Order = -20),ShowInInspector,LabelText("Current Order"),KZRichText,PropertyTooltip("My Order + Parent Order")]
// 	public int SortingLayerOrder => IsParentSortingLayerExist ? m_parentSortingLayer.SortingLayerOrder+m_sortingLayerOrder : c_defaultSortingLayerOrder+m_sortingLayerOrder;
// 	[FoldoutGroup("Sorting Layer",Order = -20),ShowInInspector,LabelText("Order Range"),KZRichText]
// 	protected string LayerRange_Display => $"{m_sortingLayerOrderMin} - {m_sortingLayerOrderMax}";

// 	[FoldoutGroup("Sorting Layer",Order = -20),ShowInInspector,LabelText("Layer Count"),KZRichText]
// 	protected int SortingLayerCount => m_sortingLayerCount;

// 	protected override void Reset()
// 	{
// 		base.Reset();

// 		m_sortingLayerCount = 0;

// 		if(!IsParentSortingLayerExist)
// 		{
// 			m_parentSortingLayer = transform.parent ? transform.parent.GetComponentInParent<SortingLayerBase>() : null;
// 		}

// 		if(m_sortingLayerCount != 0)
// 		{
// 			return;
// 		}

// 		var layerArray = GetComponentsInChildren<SortingLayerBase>(true);

// 		if(m_sortingLayerCount == layerArray.Length)
// 		{
// 			return;
// 		}

// 		m_sortingLayerOrderMin = SortingLayerOrder;
// 		m_sortingLayerOrderMax = SortingLayerOrder;

// 		for(var i=0;i<layerArray.Length;i++)
// 		{
// 			if(m_sortingLayerOrderMin > layerArray[i].SortingLayerOrder)
// 			{
// 				m_sortingLayerOrderMin = layerArray[i].SortingLayerOrder;
// 			}

// 			if(m_sortingLayerOrderMax < layerArray[i].SortingLayerOrder)
// 			{
// 				m_sortingLayerOrderMax = layerArray[i].SortingLayerOrder;
// 			}
// 		}

// 		m_sortingLayerCount = layerArray.Length;
// 	}

// 	public void SetParentSortingLayer(SortingLayerBase parentSortingLayer,int? sortingLayerOrder = null)
// 	{
// 		m_parentSortingLayer = parentSortingLayer;

// 		SetSortingLayerOrder(sortingLayerOrder ?? c_sortingInterval);
// 	}

// 	public virtual void SetSortingLayerOrder(int sortingLayerOrder)
// 	{
// 		m_sortingLayerOrder = sortingLayerOrder;
// 	}
// }