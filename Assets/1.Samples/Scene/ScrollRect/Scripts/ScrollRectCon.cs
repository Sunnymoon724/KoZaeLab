using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KZLib.KZSample
{
	public class ScrollRectCon : BaseComponent
	{
		public class SampleData : CellData
		{
			public SampleData(int _index) : base(string.Format("{0}",_index),null,null,null) { }
		}

		[SerializeField] private UIScrollRect m_VerticalScrollRect = null;

		[SerializeField] private UIScrollRect m_HorizontalScrollRect = null;

		[ShowInInspector,ReadOnly] private int Count => m_CellList.Count;

		[SerializeField] private int m_CellCount = 10;

		private List<ICellData> m_CellList = new();

		private void Start()
		{
			for(var i=0;i<m_CellCount;i++)
			{
				m_CellList.Add(new SampleData(i));
			}

			if(m_VerticalScrollRect.gameObject.activeInHierarchy)
			{
				m_VerticalScrollRect.SetCellList(m_CellList);
			}

			if(m_HorizontalScrollRect.gameObject.activeInHierarchy)
			{
				m_HorizontalScrollRect.SetCellList(m_CellList);
			}
		}
	}
}