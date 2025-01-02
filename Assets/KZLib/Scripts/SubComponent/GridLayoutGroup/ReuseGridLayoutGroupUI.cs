using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class ReuseGridLayoutGroupUI : BaseComponentUI
{
	[VerticalGroup("General",Order = 0),SerializeField]
	private Transform m_storage = null;
	[VerticalGroup("General",Order = 0),SerializeField]
	private SlotUI m_pivot = null;

	[VerticalGroup("General",Order = 0),SerializeField]
	private GridLayoutGroup m_gridLayoutGroup = null;

	[VerticalGroup("General",Order = 0),SerializeField,ReadOnly]
	private List<SlotUI> m_slotList = new();

	private readonly List<ICellData> m_cellDataList = new();

	private GameObjectUIPool<SlotUI> m_slotPool = null;

	protected override void Initialize()
	{
		base.Initialize();

		if(!m_pivot)
		{
			LogTag.UI.E("Pivot is null");

			return;
		}

		if(!m_storage)
		{
			LogTag.UI.E("Storage is null");

			return;
		}

		m_pivot.gameObject.SetActiveIfDifferent(false);
		m_storage.SetUIChild(m_pivot.transform);

		m_slotPool = new GameObjectUIPool<SlotUI>(m_pivot,m_storage);

		m_cellDataList.Clear();
		m_slotList.Clear();
	}

	public void SetCellList(List<ICellData> cellDataList)
	{
		m_cellDataList.Clear();
		m_cellDataList.AddRange(cellDataList);

		if(m_cellDataList.Count < m_slotList.Count)
		{
			RecycleExtraSlot(m_cellDataList.Count);
		}
		else if(m_cellDataList.Count > m_slotList.Count)
		{
			var count = m_cellDataList.Count-m_slotList.Count;
			
			for(var i=0;i<count;i++)
			{
				m_slotList.Add(GetOrCreateSlot());
			}
		}

		for(var i=0;i<m_slotList.Count;i++)
		{
			m_slotList[i].SetCell(m_cellDataList[i]);
		}
	}

	private SlotUI GetOrCreateSlot()
	{
		var slot = m_slotPool.GetOrCreate(m_gridLayoutGroup.transform);

		slot.gameObject.SetActive(true);

		return slot;
	}

	private void RecycleExtraSlot(int count)
	{
		for(var i=count;i<m_slotList.Count;i++)
		{
			m_slotPool.Put(m_slotList[i]);
		}

		m_slotList.RemoveRange(count,m_slotList.Count-count);
	}

	protected override void Reset()
	{
		base.Reset();

		if(!m_storage)
		{
			m_storage = transform.parent;
		}

		if(!m_gridLayoutGroup)
		{
			m_gridLayoutGroup = GetComponent<GridLayoutGroup>();
		}
	}
}