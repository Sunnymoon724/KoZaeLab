using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KZLib.KZSample
{
	public class ReuseScrollRectCon : BaseComponent
	{
		private record SampleData : CellData
		{
			public SampleData(int index) : base($"{index}",null,null,null,null) { }
		}

		[VerticalGroup("0",Order = 0),SerializeField]
		private ReuseScrollRectUI m_verticalScrollRect = null;

		[VerticalGroup("0",Order = 0),SerializeField]
		private ReuseScrollRectUI m_horizontalScrollRect = null;

		[SerializeField,HideInInspector]
		private int m_cellCount = 10;

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

				if(Application.isPlaying)
				{
					_SetCellList();
				}
			}
		}

		[VerticalGroup("2",Order = 2),SerializeField,Range(0.0f,1.0f)]
		private float m_duration = 0.5f;
		[VerticalGroup("2",Order = 2),SerializeField,ValueDropdown(nameof(m_orderList))]
		private int m_order = 0;

		private readonly List<int> m_orderList = new();

		[HorizontalGroup("3",Order = 3),Button("Number Move To Top",ButtonSizes.Medium)]
		protected void OnMoveToTop()
		{
			if(m_verticalScrollRect.gameObject.activeInHierarchy)
			{
				m_verticalScrollRect.ScrollToTop(m_order,m_duration);
			}

			if(m_horizontalScrollRect.gameObject.activeInHierarchy)
			{
				m_horizontalScrollRect.ScrollToTop(m_order,m_duration);
			}
		}

		[HorizontalGroup("3",Order = 3),Button("Number Move To Center",ButtonSizes.Medium)]
		protected void OnMoveToCenter()
		{
			if(m_verticalScrollRect.gameObject.activeInHierarchy)
			{
				m_verticalScrollRect.ScrollToCenter(m_order,m_duration);
			}

			if(m_horizontalScrollRect.gameObject.activeInHierarchy)
			{
				m_horizontalScrollRect.ScrollToCenter(m_order,m_duration);
			}
		}

		[HorizontalGroup("3",Order = 3),Button("Number Move To Bottom",ButtonSizes.Medium)]
		protected void OnMoveToBottom()
		{
			if(m_verticalScrollRect.gameObject.activeInHierarchy)
			{
				m_verticalScrollRect.ScrollToBottom(m_order,m_duration);
			}

			if(m_horizontalScrollRect.gameObject.activeInHierarchy)
			{
				m_horizontalScrollRect.ScrollToBottom(m_order,m_duration);
			}
		}

		private readonly List<ICellData> m_cellList = new();

		private void Start()
		{
			_SetCellList();
		}

		private void _SetCellList()
		{
			var count = CellCount;

			m_cellList.Clear();
			m_orderList.Clear();

			for(var i=0;i<count;i++)
			{
				m_cellList.Add(new SampleData(i));
				m_orderList.Add(i);
			}

			if(m_verticalScrollRect.gameObject.activeInHierarchy)
			{
				m_verticalScrollRect.SetCellList(m_cellList);
			}

			if(m_horizontalScrollRect.gameObject.activeInHierarchy)
			{
				m_horizontalScrollRect.SetCellList(m_cellList);
			}
		}
	}
}