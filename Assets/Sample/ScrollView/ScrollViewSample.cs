using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ScrollViewSample : MonoBehaviour
{
	public class SampleData : CellData
	{
		public int Index { get; }

		public SampleData(int _index)
		{
			Index = _index;
		}
	}

	[SerializeField] private UIReuseScrollView m_ScrollView;
	[SerializeField,ReadOnly] private int Count => m_CellList.Count;

	private List<ICellData> m_CellList = new List<ICellData>();
	
	[SerializeField,Button("첫번째 슬롯 추가")]
	private void OnAddFirstSlot()
	{
		if(Application.isPlaying == false)
		{
			return;
		}

		m_CellList.Insert(0,new SampleData(Count));

		m_ScrollView.SetCellList(m_CellList);
	}

		[SerializeField,Button("마지막 슬롯 추가")]
	private void OnAddLastSlot()
	{
		if(Application.isPlaying == false)
		{
			return;
		}

		m_CellList.Add(new SampleData(Count));

		m_ScrollView.SetCellList(m_CellList);
	}

	[SerializeField,Button("첫번째 슬롯 삭제")]
	private void OnRemoveFirstSlot()
	{
		if(Application.isPlaying == false)
		{
			return;
		}

		m_CellList.RemoveAt(0);

		m_ScrollView.SetCellList(m_CellList);
	}

	[SerializeField,Button("마지막 슬롯 삭제")]
	private void OnRemoveLastSlot()
	{
		if(Application.isPlaying == false)
		{
			return;
		}

		m_CellList.RemoveAt(m_CellList.Count-1);

		m_ScrollView.SetCellList(m_CellList);
	}
}