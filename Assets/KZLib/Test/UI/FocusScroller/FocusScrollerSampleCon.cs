using System.Collections.Generic;
using KZLib.Develop;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KZLib.KZSample
{
	public class FocusScrollerSampleCon : BaseComponent
	{
		private record SampleData : CellData
		{
			public SampleData(int _index) : base($"{_index}",null,null,null,null) { }
		}

		[VerticalGroup("0",Order = 0),SerializeField]
		private FocusScroller m_Scroller = null;

		[SerializeField,HideInInspector]
		private int m_CellCount = 3;

		[SerializeField,HideInInspector]
		private int m_CenterIndex = 0;

		[VerticalGroup("1",Order = 1),ShowInInspector]
		private int CellCount
		{
			get => m_CellCount;
			set
			{
				if(m_CellCount == value)
				{
					return;
				}

				m_CellCount = value;
				m_CenterIndex = 0;

				if(Application.isPlaying)
				{
					SetCellList();
				}
			}
		}

		protected int MaxCenter => CellCount-1;

		[VerticalGroup("1",Order = 1),ShowInInspector,PropertyRange(0,"$MaxCenter")]
		private int CenterIndex
		{
			get => m_CenterIndex;
			set
			{
				if(m_CenterIndex == value)
				{
					return;
				}

				m_CenterIndex = value;

				if(Application.isPlaying)
				{
					SetCellList();
				}
			}
		}

		private readonly List<int> m_OrderList = new();

		private readonly List<ICellData> m_CellList = new();

		private void Start()
		{
			SetCellList();
		}

		private void SetCellList()
		{
			var count = CellCount;

			m_CellList.Clear();
			m_OrderList.Clear();

			for(var i=0;i<count;i++)
			{
				m_CellList.Add(new SampleData(i));
				m_OrderList.Add(i);
			}

			m_Scroller.SetCellList(m_CellList,CenterIndex);
		}
	}
}