using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using KZLib.KZAttribute;
using System.Linq;
using UnityEngine.Events;

namespace KZLib.KZDevelop
{
	public partial class FocusScroller : BaseComponentUI,IPointerUpHandler,IPointerDownHandler,IBeginDragHandler,IEndDragHandler,IDragHandler,IScrollHandler
	{
		[BoxGroup("General",Order = 0)]
		[VerticalGroup("General/0",Order = 0),SerializeField]
		private RectTransform m_viewport = null;

		[VerticalGroup("General/0",Order = 0),SerializeField]
		private FocusSlotUI m_slotUI = null;

		[VerticalGroup("General/0",Order = 0),SerializeField]
		private bool m_circularMode = true;
		[VerticalGroup("General/0",Order = 0),SerializeField]
		private bool m_orderMode = false;
		[VerticalGroup("General/0",Order = 0),SerializeField,Range(0.01f,0.5f)]
		private float m_slotSpace = 0.1f;

		[VerticalGroup("General/0",Order = 0),SerializeField]
		private bool m_vertical = false;

		[VerticalGroup("General/0",Order = 0),SerializeField]
		private bool m_useDrag = true;

		[VerticalGroup("General/0",Order = 0),SerializeField,KZMinClamp(1)]
		private int m_poolCapacity = 1;

		private readonly List<FocusSlotUI> m_slotUIList = new();
		private readonly List<ICellData> m_cellDataList = new();

		private readonly List<(float,Transform)> m_orderList = new();

		[BoxGroup("Viewer",ShowLabel = false,Order = 99),SerializeField,KZRichText]
		private int m_focusIndex = -1;

		private GameObjectUIPool<FocusSlotUI> m_slotUIPool = null;

		[BoxGroup("Viewer",ShowLabel = false,Order = 99),SerializeField,KZRichText]
		private float m_currentLocation = 0.0f;

		private bool IsCircularMode => m_circularMode && m_cellDataList.Count != 1;

		public event Action<ICellData> OnFocusSet = null;

		public ICellData FocusCellData => m_cellDataList.TryGetValueByIndex(m_focusIndex,out var data) ? data : null;

		protected override void Initialize()
		{
			base.Initialize();

			if(!m_slotUI)
			{
				KZLogType.System.E("Slot is null");

				return;
			}

			var slotUI = m_slotUI as SlotUI;

			if(!m_viewport)
			{
				KZLogType.System.E("Viewport is null");

				return;
			}

			slotUI.gameObject.EnsureActive(false);
			transform.SetUIChild(slotUI.transform);

			m_viewport.pivot = new Vector2(0.0f,1.0f);
			slotUI.UIRectTransform.pivot = new Vector2(0.5f,0.5f);

			m_slotUIPool = new GameObjectUIPool<FocusSlotUI>(m_slotUI,m_viewport,m_poolCapacity);

			m_cellDataList.Clear();
			m_slotUIList.Clear();
		}

		public void SetCellList(List<ICellData> cellDataList,int? index = null)
		{
			var focusIndex = index.HasValue ? Mathf.Clamp(index.Value,0,cellDataList.Count) : 0;

			m_cellDataList.Clear();
			m_cellDataList.AddRange(cellDataList);

			_UpdateLocation(focusIndex,true);
			UpdateIndex(focusIndex);
		}

		public void UpdateIndex(int index)
		{
			m_focusIndex = CommonUtility.LoopClamp(index,m_cellDataList.Count);

			OnFocusSet?.Invoke(m_cellDataList[m_focusIndex]);
		}

		private void _UpdateLocation(float location,bool isForceRefresh)
		{
			m_currentLocation = location;

			var newLocation = location-0.5f/m_slotSpace;
			var firstIndex = Mathf.CeilToInt(newLocation);
			var firstLocation = (Mathf.Ceil(newLocation)-newLocation)*m_slotSpace;

			if(firstLocation+m_slotUIList.Count*m_slotSpace < 1.0f)
			{
				_ResizePool(firstLocation);
			}

			_UpdateSlotList(firstLocation,firstIndex,isForceRefresh);
		}

		private void _ResizePool(float firstLocation)
		{
			var count = Mathf.CeilToInt((1.0f-firstLocation)/m_slotSpace)-m_slotUIList.Count;

			for(var i=0;i<count;i++)
			{
				m_slotUIList.Add(m_slotUIPool.GetOrCreate(m_viewport));
			}
		}

		private void _UpdateSlotList(float firstLocation,int firstIndex,bool isForceRefresh)
		{
			var cellCount = m_cellDataList.Count;
			var slotCount = m_slotUIList.Count;

			m_orderList.Clear();

			for(var i=0;i<slotCount;i++)
			{
				var index = firstIndex+i;
				var location = firstLocation+i*m_slotSpace;
				var slot = m_slotUIList[CommonUtility.LoopClamp(index,slotCount)];

				if(IsCircularMode)
				{
					index = CommonUtility.LoopClamp(index,cellCount);
				}

				if(index < 0 || index >= cellCount || location > 1.0f)
				{
					slot.gameObject.EnsureActive(false);

					continue;
				}

				if(isForceRefresh || !slot.gameObject.activeSelf)
				{
					slot.gameObject.EnsureActive(true);
					slot.name = $"Slot_{i}";
					slot.SetCell(m_cellDataList[index]);
				}

				slot.UpdateLocation(location);

				if(m_orderMode)
				{
					m_orderList.Add((location,slot.transform));
				}
			}

			if(m_orderMode)
			{
				foreach(var pair in m_orderList.OrderByDescending(x=>Math.Abs(x.Item1-0.5f)))
				{
					pair.Item2.SetAsLastSibling();
				}
			}
		}

		protected override void Reset()
		{
			base.Reset();

			if(!m_viewport)
			{
				var viewport = transform.Find("Viewport");

				if(!viewport)
				{
					throw new NullReferenceException("No Viewport");
				}

				m_viewport = viewport.GetComponent<RectTransform>();
			}
		}
	}
}