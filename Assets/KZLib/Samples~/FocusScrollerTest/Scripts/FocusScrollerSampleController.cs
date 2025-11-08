using System.Collections.Generic;
using KZLib.KZDevelop;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KZLib.KZSample
{
	public class FocusScrollerSampleController : BaseComponent
	{
		private record SampleData : CellData
		{
			public SampleData(int index) : base($"{index}",null,null,null,null) { }
		}

		[VerticalGroup("0",Order = 0),SerializeField]
		private FocusScroller m_focusScroller = null;

		[SerializeField,HideInInspector]
		private int m_cellCount = 3;

		[SerializeField,HideInInspector]
		private int m_centerIndex = 0;

		[VerticalGroup("1",Order = 1),ShowInInspector]
		private int CellCount
		{
			get => m_cellCount;
			set
			{
				if(m_cellCount == value)
				{
					return;
				}

				m_cellCount = value;
				m_centerIndex = 0;

				if(Application.isPlaying)
				{
					_SetCellList();
				}
			}
		}

		protected int MaxCenter => CellCount-1;

		[VerticalGroup("1",Order = 1),ShowInInspector,PropertyRange(0,nameof(MaxCenter))]
		private int CenterIndex
		{
			get => m_centerIndex;
			set
			{
				if(m_centerIndex == value)
				{
					return;
				}

				m_centerIndex = value;

				if(Application.isPlaying)
				{
					_SetCellList();
				}
			}
		}

		private readonly List<int> m_orderList = new();

		private readonly List<ICellData> m_cellDataList = new();

		private void Start()
		{
			_SetCellList();
		}

		private void _SetCellList()
		{
			var count = CellCount;

			m_cellDataList.Clear();
			m_orderList.Clear();

			for(var i=0;i<count;i++)
			{
				m_cellDataList.Add(new SampleData(i));
				m_orderList.Add(i);
			}

			m_focusScroller.SetCellList(m_cellDataList,CenterIndex);
		}
	}
}