using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 셀을 저장하고 알아서 꺼내서 사용함
/// </summary>
[RequireComponent(typeof(GridLayoutGroup))]
public class GridLayoutGroupUI : BaseComponentUI
{
	[VerticalGroup("기본",Order = 0),SerializeField]
	private Transform m_Storage = null;
	[VerticalGroup("기본",Order = 0),SerializeField]
	private SlotUI m_Pivot = null;

	[VerticalGroup("기본",Order = 0),SerializeField]
	private GridLayoutGroup m_GridLayout = null;

	[VerticalGroup("기본",Order = 0),SerializeField,ReadOnly]
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
			throw new NullReferenceException("피벗이 없습니다.");
		}

		if(!m_Storage)
		{
			throw new NullReferenceException("스토리지가 없습니다.");
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