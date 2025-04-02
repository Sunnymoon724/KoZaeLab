using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KZLib.KZSample
{
	public class ScrollRectUICon : BaseComponent
	{
		private record SampleData : CellData
		{
			public SampleData(int _index) : base($"{_index}",null,null,null,null) { }
		}

		[VerticalGroup("0",Order = 0),SerializeField]
		private ReuseScrollRectUI m_VerticalScrollRect = null;

		[VerticalGroup("0",Order = 0),SerializeField]
		private ReuseScrollRectUI m_HorizontalScrollRect = null;

		[SerializeField,HideInInspector]
		private int m_CellCount = 10;

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

				if(Application.isPlaying)
				{
					_SetCellList();
				}
			}
		}

		[VerticalGroup("2",Order = 2),SerializeField,Range(0.0f,1.0f)]
		private float m_Duration = 0.5f;
		[VerticalGroup("2",Order = 2),SerializeField,LabelText("Number"),ValueDropdown(nameof(m_OrderList))]
		private int m_Order = 0;

		private readonly List<int> m_OrderList = new();

		[HorizontalGroup("3",Order = 3),Button("Number Move To Top",ButtonSizes.Medium)]
		protected void OnMoveToTop()
		{
			if(m_VerticalScrollRect.gameObject.activeInHierarchy)
			{
				m_VerticalScrollRect.ScrollToTop(m_Order,m_Duration);
			}

			if(m_HorizontalScrollRect.gameObject.activeInHierarchy)
			{
				m_HorizontalScrollRect.ScrollToTop(m_Order,m_Duration);
			}
		}

		[HorizontalGroup("3",Order = 3),Button("Number Move To Center",ButtonSizes.Medium)]
		protected void OnMoveToCenter()
		{
			if(m_VerticalScrollRect.gameObject.activeInHierarchy)
			{
				m_VerticalScrollRect.ScrollToCenter(m_Order,m_Duration);
			}

			if(m_HorizontalScrollRect.gameObject.activeInHierarchy)
			{
				m_HorizontalScrollRect.ScrollToCenter(m_Order,m_Duration);
			}
		}

		[HorizontalGroup("3",Order = 3),Button("Number Move To Bottom",ButtonSizes.Medium)]
		protected void OnMoveToBottom()
		{
			if(m_VerticalScrollRect.gameObject.activeInHierarchy)
			{
				m_VerticalScrollRect.ScrollToBottom(m_Order,m_Duration);
			}

			if(m_HorizontalScrollRect.gameObject.activeInHierarchy)
			{
				m_HorizontalScrollRect.ScrollToBottom(m_Order,m_Duration);
			}
		}

		private readonly List<ICellData> m_CellList = new();

		private void Start()
		{
			_SetCellList();
		}

		private void _SetCellList()
		{
			var count = CellCount;

			m_CellList.Clear();
			m_OrderList.Clear();

			for(var i=0;i<count;i++)
			{
				m_CellList.Add(new SampleData(i));
				m_OrderList.Add(i);
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