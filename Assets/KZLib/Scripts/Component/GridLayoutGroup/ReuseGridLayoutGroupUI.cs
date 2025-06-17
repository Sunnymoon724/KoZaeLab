using System.Collections.Generic;
using KZLib.KZAttribute;
using KZLib.KZDevelop;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class ReuseGridLayoutGroupUI : BaseComponentUI
{
	[SerializeField]
	private Transform m_storage = null;
	[SerializeField]
	private SlotUI m_pivot = null;

	[SerializeField]
	private GridLayoutGroup m_gridLayoutGroup = null;

	[SerializeField,KZMinClamp(1)]
	private int m_poolCapacity = 1;

	[SerializeField,ReadOnly]
	private List<SlotUI> m_slotList = new();

	private readonly List<ICellData> m_cellDataList = new();

	private GameObjectUIPool<SlotUI> m_slotUIPool = null;

	protected override void Initialize()
	{
		base.Initialize();

		if(!m_pivot)
		{
			Logger.UI.E("Pivot is null");

			return;
		}

		if(!m_storage)
		{
			Logger.UI.E("Storage is null");

			return;
		}

		m_pivot.gameObject.EnsureActive(false);
		m_storage.SetUIChild(m_pivot.transform);

		m_slotUIPool = new GameObjectUIPool<SlotUI>(m_pivot,m_storage,m_poolCapacity);

		m_cellDataList.Clear();
		m_slotList.Clear();
	}

	public void SetCellList(List<ICellData> cellDataList)
	{
		m_cellDataList.Clear();
		m_cellDataList.AddRange(cellDataList);

		if(m_cellDataList.Count < m_slotList.Count)
		{
			_RecycleExtraSlot(m_cellDataList.Count);
		}
		else if(m_cellDataList.Count > m_slotList.Count)
		{
			var count = m_cellDataList.Count-m_slotList.Count;
			
			for(var i=0;i<count;i++)
			{
				m_slotList.Add(_GetOrCreateSlot());
			}
		}

		for(var i=0;i<m_slotList.Count;i++)
		{
			m_slotList[i].SetCell(m_cellDataList[i]);
		}
	}

	private SlotUI _GetOrCreateSlot()
	{
		var slot = m_slotUIPool.GetOrCreate(m_gridLayoutGroup.transform);

		slot.gameObject.SetActive(true);

		return slot;
	}

	private void _RecycleExtraSlot(int count)
	{
		for(var i=count;i<m_slotList.Count;i++)
		{
			m_slotUIPool.Put(m_slotList[i]);
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