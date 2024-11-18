using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Save the cell and retrieve it as needed.
/// </summary>
[RequireComponent(typeof(GridLayoutGroup))]
public class GridLayoutGroupUI : BaseComponentUI
{
	[VerticalGroup("General",Order = 0),SerializeField]
	private Transform m_Storage = null;
	[VerticalGroup("General",Order = 0),SerializeField]
	private SlotUI m_Pivot = null;

	[VerticalGroup("General",Order = 0),SerializeField]
	private GridLayoutGroup m_GridLayout = null;

	[VerticalGroup("General",Order = 0),SerializeField,ReadOnly]
	private List<SlotUI> m_SlotList = new();

	private readonly List<ICellData> m_CellList = new();

	private GameObjectUIPool<SlotUI> m_ObjectPool = null;

	private bool m_Initialize = false;

	protected override void Initialize()
	{
		if(m_Initialize)
		{
			return;
		}

		base.Initialize();

		if(!m_Pivot)
		{
			LogTag.UI.E("Pivot is null");

			return;
		}

		if(!m_Storage)
		{
			LogTag.UI.E("Storage is null");

			return;
		}

		m_Pivot.gameObject.SetActiveSelf(false);
		m_Storage.SetUIChild(m_Pivot.transform);

		m_ObjectPool = new GameObjectUIPool<SlotUI>(m_Pivot,m_Storage);

		m_CellList.Clear();
		m_SlotList.Clear();

		m_Initialize = true;
	}

	public void SetCellList(List<ICellData> _cellList)
	{
		Initialize();

		m_CellList.Clear();
		m_CellList.AddRange(_cellList);

		if(m_CellList.Count < m_SlotList.Count)
		{
			RemoveSlot(m_CellList.Count);
		}
		else if(m_CellList.Count > m_SlotList.Count)
		{
			var count = m_CellList.Count-m_SlotList.Count;
			
			for(var i=0;i<count;i++)
			{
				m_SlotList.Add(AddSlot());
			}
		}

		for(var i=0;i<m_SlotList.Count;i++)
		{
			m_SlotList[i].SetCell(m_CellList[i]);
		}
	}

	private SlotUI AddSlot()
	{
		var slot = m_ObjectPool.Get(m_GridLayout.transform);

		slot.gameObject.SetActive(true);

		return slot;
	}

	private void RemoveSlot(int _count)
	{
		for(var i=_count;i<m_SlotList.Count;i++)
		{
			m_ObjectPool.Put(m_SlotList[i]);
		}

		m_SlotList.RemoveRange(_count,m_SlotList.Count-_count);
	}

	protected override void Reset()
	{
		base.Reset();

		if(!m_Storage)
		{
			m_Storage = transform.parent;
		}

		if(!m_GridLayout)
		{
			m_GridLayout = GetComponent<GridLayoutGroup>();
		}
	}
}