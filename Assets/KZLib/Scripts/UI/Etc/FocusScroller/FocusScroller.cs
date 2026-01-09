using System;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using KZLib.KZAttribute;
using R3;
using KZLib.KZDevelop;

namespace UnityEngine.UI
{
	public partial class FocusScroller : BaseComponentUI,IPointerUpHandler,IPointerDownHandler,IBeginDragHandler,IEndDragHandler,IDragHandler,IScrollHandler
	{
		[SerializeField]
		private RectTransform m_viewport = null;

		[SerializeField]
		private FocusSlot m_slot = null;

		[SerializeField]
		private bool m_circularMode = true;
		[SerializeField]
		private bool m_orderMode = false;
		[SerializeField,Range(0.01f,0.5f)]
		private float m_slotSpace = 0.1f;

		[SerializeField]
		private bool m_vertical = false;

		[SerializeField]
		private bool m_useDrag = true;

		[SerializeField,KZMinClamp(1)]
		private int m_poolCapacity = 1;

		private readonly List<FocusSlot> m_slotList = new();
		private readonly List<IEntryInfo> m_entryInfoList = new();

		private readonly List<OrderSortInfo> m_orderList = new();

		[Space(10)]
		[BoxGroup("0",ShowLabel = false,Order = 99),SerializeField,KZRichText]
		private int m_focusIndex = Global.INVALID_INDEX;

		[BoxGroup("0",ShowLabel = false,Order = 99),SerializeField,KZRichText]
		private float m_currentLocation = 0.0f;

		private GameObjectPool<FocusSlot> m_slotPool = null;

		private readonly Subject<IEntryInfo> m_focusSubject = new();
		public Observable<IEntryInfo> OnChangedFocus => m_focusSubject;

		public IEntryInfo FocusEntryInfo => m_entryInfoList.TryGetValueByIndex(m_focusIndex,out var info) ? info : null;
		private bool IsCircularMode => m_circularMode && m_entryInfoList.Count != 1;

		protected override void Initialize()
		{
			base.Initialize();

			m_slotPool = new GameObjectPool<FocusSlot>(m_slot,m_viewport,m_poolCapacity,false);

			var slot = m_slot as Slot;

			slot.gameObject.EnsureActive(false);
			m_viewport.SetChild(slot.transform,false);

			m_viewport.pivot = new Vector2(0.0f,1.0f);
			slot.CurrentRect.pivot = new Vector2(0.5f,0.5f);

			m_entryInfoList.Clear();
			m_slotList.Clear();
		}

		public void SetEntryInfoList(List<IEntryInfo> entryInfoList,int index = -1)
		{
			var focusIndex = index != -1 ? Mathf.Clamp(index,0,entryInfoList.Count) : 0;

			m_entryInfoList.Clear();

			if(!entryInfoList.IsNullOrEmpty())
			{
				m_entryInfoList.AddRange(entryInfoList);
			}

			_RefreshLocation(focusIndex,true);
			RefreshIndex(focusIndex);
		}

		public void RefreshIndex(int index)
		{
			m_focusIndex = CommonUtility.LoopClamp(index,m_entryInfoList.Count);

			m_focusSubject.OnNext(m_entryInfoList[m_focusIndex]);
		}

		private void _RefreshLocation(float location,bool isForceRefresh)
		{
			m_currentLocation = location;

			var newLocation = location-0.5f/m_slotSpace;
			var firstIndex = Mathf.CeilToInt(newLocation);
			var firstLocation = (Mathf.Ceil(newLocation)-newLocation)*m_slotSpace;

			if(firstLocation+m_slotList.Count*m_slotSpace < 1.0f)
			{
				_ResizePool(firstLocation);
			}

			_RefreshSlotList(firstLocation,firstIndex,isForceRefresh);
		}

		private void _ResizePool(float firstLocation)
		{
			var count = Mathf.CeilToInt((1.0f-firstLocation)/m_slotSpace)-m_slotList.Count;

			for(var i=0;i<count;i++)
			{
				m_slotList.Add(m_slotPool.GetOrCreate(m_viewport));
			}
		}

		private void _RefreshSlotList(float firstLocation,int firstIndex,bool isForceRefresh)
		{
			var entryInfoCount = m_entryInfoList.Count;
			var slotCount = m_slotList.Count;

			m_orderList.Clear();

			for(var i=0;i<slotCount;i++)
			{
				var index = firstIndex+i;
				var location = firstLocation+i*m_slotSpace;
				var slot = m_slotList[CommonUtility.LoopClamp(index,slotCount)];

				if(IsCircularMode)
				{
					index = CommonUtility.LoopClamp(index,entryInfoCount);
				}

				if(index < 0 || index >= entryInfoCount || location > 1.0f)
				{
					slot.gameObject.EnsureActive(false);

					continue;
				}

				if(isForceRefresh || !slot.gameObject.activeSelf)
				{
					slot.gameObject.EnsureActive(true);
					slot.name = $"Slot_{i}";
					slot.SetEntryInfo(m_entryInfoList[index]);
				}

				slot.RefreshLocation(location);

				if(m_orderMode)
				{
					m_orderList.Add(new OrderSortInfo(location,slot.transform));
				}
			}

			if(m_orderMode)
			{
				static int _Sort(OrderSortInfo infoA,OrderSortInfo infoB)
				{
					var valueA = Math.Abs(infoA.Location-0.5f);
					var valueB = Math.Abs(infoB.Location-0.5f);

					return valueB.CompareTo(valueA); 
				}

				m_orderList.Sort(_Sort);

				for(var i=0;i<m_orderList.Count;i++)
				{
					m_orderList[i].Target.SetAsLastSibling();
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

		private record OrderSortInfo(float Location,Transform Target);
	}
}