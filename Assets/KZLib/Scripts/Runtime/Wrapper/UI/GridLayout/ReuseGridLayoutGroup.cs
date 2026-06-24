using System.Collections.Generic;
using KZLib.Attributes;
using KZLib.UI;
using KZLib.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Virtualized grid that pools <see cref="Slot"/> instances for a list of <see cref="IEntryInfo"/>.
/// </summary>
[RequireComponent(typeof(GridLayoutGroup))]
public class ReuseGridLayoutGroup : BaseComponent
{
	[SerializeField]
	private Transform m_storage = null;
	[SerializeField]
	private Slot m_pivot = null;

	[SerializeField]
	private GridLayoutGroup m_gridLayoutGroup = null;

	[SerializeField,KZMinClamp(1)]
	private int m_poolCapacity = 1;

	[SerializeField,ReadOnly]
	private List<Slot> m_slotList = new();

	private readonly List<IEntryInfo> m_entryInfoList = new();

	private GameObjectPawnPool<Slot> m_slotPool = null;

	/// <summary><c>true</c> after <see cref="_Initialize"/> creates the slot pool.</summary>
	public bool IsReady => m_slotPool != null;

	protected override void _Initialize()
	{
		base._Initialize();

		if(!m_pivot || !m_storage || !m_gridLayoutGroup)
		{
			LogChannel.UI.W("ReuseGridLayoutGroup: pivot, storage, or gridLayoutGroup is not assigned.");

			return;
		}

		m_slotPool = new GameObjectPawnPool<Slot>(m_pivot,m_storage,m_poolCapacity,false);

		m_pivot.gameObject.EnsureActive(false);
		m_storage.SetChild(m_pivot.transform,false);

		m_entryInfoList.Clear();
		m_slotList.Clear();
	}

	/// <summary>Rebinds visible slots to <paramref name="entryInfoList"/>, growing or recycling the pool as needed.</summary>
	public void SetEntryInfoList(List<IEntryInfo> entryInfoList)
	{
		if(m_slotPool == null || entryInfoList == null)
		{
			return;
		}

		m_entryInfoList.Clear();
		m_entryInfoList.AddRange(entryInfoList);

		if(m_entryInfoList.Count < m_slotList.Count)
		{
			_RecycleExtraSlot(m_entryInfoList.Count);
		}
		else if(m_entryInfoList.Count > m_slotList.Count)
		{
			var count = m_entryInfoList.Count-m_slotList.Count;
			
			for(var i=0;i<count;i++)
			{
				m_slotList.Add(_GetOrCreateSlot());
			}
		}

		for(var i=0;i<m_slotList.Count;i++)
		{
			m_slotList[i].SetEntryInfo(m_entryInfoList[i]);
		}
	}

	private Slot _GetOrCreateSlot()
	{
		var slot = m_slotPool.GetOrCreate(m_gridLayoutGroup.transform);

		slot.gameObject.SetActive(true);

		return slot;
	}

	private void _RecycleExtraSlot(int count)
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