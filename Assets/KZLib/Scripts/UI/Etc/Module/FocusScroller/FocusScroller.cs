using System;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using KZLib.Attributes;
using R3;
using UnityEngine;
using KZLib.Utilities;

namespace KZLib.UI
{
	/// <summary>
	/// Focus-based scroller that virtualizes <see cref="FocusSlot"/> instances inside a viewport using a normalized
	/// scroll location (<see cref="CurrentLocation"/>).
	/// </summary>
	/// <remarks>
	/// <para>
	/// Unlike <see cref="Carousel"/>, which drives a <see cref="UnityEngine.UI.ScrollRect"/>, this component owns
	/// drag, wheel, inertia, and snap logic directly. Only enough pooled slots to fill the viewport are active;
	/// each visible slot receives <see cref="FocusSlot.RefreshLocation"/> while scrolling.
	/// </para>
	/// <para>
	/// <see cref="OnChangedFocus"/> fires when the snapped focus index changes (after scroll completion,
	/// <see cref="JumpTo"/>, or <see cref="SetEntryInfoList"/>). It emits <c>null</c> when the entry list is cleared.
	/// During scroll, visual focus is conveyed through slot location only.
	/// </para>
	/// <para>
	/// When using <see cref="AnchorFocusSlot"/>, set its vertical axis flag to match <c>m_vertical</c> on this component.
	/// Re-bind data by calling <see cref="SetEntryInfoList"/>; in-place entry changes at the same index are not detected automatically.
	/// </para>
	/// </remarks>
	public partial class FocusScroller : MonoBehaviour,IPointerUpHandler,IPointerDownHandler,IBeginDragHandler,IEndDragHandler,IDragHandler,IScrollHandler
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

		[SerializeField,KZMinClamp(1)]
		private int m_poolCapacity = 1;

		private readonly List<FocusSlot> m_slotList = new();
		private readonly List<IEntryInfo> m_entryInfoList = new();

		private readonly List<OrderSortInfo> m_orderList = new();

		/// <summary>Tracks the entry index bound to each pooled slot to avoid stale data on pool reuse.</summary>
		private readonly Dictionary<FocusSlot,int> m_boundEntryIndexMap = new();

		[Space(10)]
		[BoxGroup("0",ShowLabel = false,Order = 99),SerializeField,KZRichText]
		private int m_focusIndex = Global.InvalidIndex;

		[BoxGroup("0",ShowLabel = false,Order = 99),SerializeField,KZRichText]
		private float m_currentLocation = 0.0f;

		private GameObjectPawnPool<FocusSlot> m_slotPool = null;

		private readonly Subject<IEntryInfo> m_focusSubject = new();

		/// <summary>Emits the focused entry after snap, jump, or list refresh; emits <c>null</c> when the list is empty.</summary>
		public Observable<IEntryInfo> OnChangedFocus => m_focusSubject;

		/// <summary>Currently focused entry, or <c>null</c> when none.</summary>
		public IEntryInfo FocusEntryInfo => m_entryInfoList.TryGetValueByIndex(m_focusIndex,out var info) ? info : null;

		/// <summary><c>true</c> when circular wrapping is active (requires more than one entry).</summary>
		private bool IsCircularMode => m_circularMode && m_entryInfoList.Count > 1;

		private void Awake()
		{
			m_slotPool = new GameObjectPawnPool<FocusSlot>(m_slot,m_viewport,m_poolCapacity,false);

			var slotRootRect = m_slot.GetComponent<RectTransform>();

			slotRootRect.gameObject.EnsureActive(false);
			m_viewport.SetChild(slotRootRect.transform,false);

			m_viewport.pivot = new Vector2(0.0f,1.0f);
			slotRootRect.pivot = new Vector2(0.5f,0.5f);

			m_entryInfoList.Clear();
			m_slotList.Clear();
			m_boundEntryIndexMap.Clear();
		}

		/// <summary>
		/// Replaces all entries and resets the viewport. <paramref name="index"/> selects the initial focus
		/// (clamped to the list range; defaults to <c>0</c> when <c>-1</c>).
		/// </summary>
		public void SetEntryInfoList(List<IEntryInfo> entryInfoList,int index = -1)
		{
			m_entryInfoList.Clear();
			m_boundEntryIndexMap.Clear();

			if(!entryInfoList.IsNullOrEmpty())
			{
				m_entryInfoList.AddRange(entryInfoList);
			}

			var count = m_entryInfoList.Count;
			var focusIndex = count == 0 ? 0 : index != -1 ? Mathf.Clamp(index,0,count-1) : 0;

			_RefreshLocation(focusIndex,true);
			RefreshIndex(focusIndex);
		}

		/// <summary>
		/// Updates the focus index and notifies <see cref="OnChangedFocus"/> without moving scroll location.
		/// </summary>
		public void RefreshIndex(int index)
		{
			if(m_entryInfoList.Count == 0)
			{
				m_focusIndex = Global.InvalidIndex;
				m_focusSubject.OnNext(null);

				return;
			}

			m_focusIndex = KZMathKit.LoopClamp(index,m_entryInfoList.Count);

			m_focusSubject.OnNext(m_entryInfoList[m_focusIndex]);
		}

		/// <summary>Maps scroll location to pooled slots and rebinds entries when the visible window shifts.</summary>
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
				var slot = m_slotList[KZMathKit.LoopClamp(index,slotCount)];

				if(IsCircularMode)
				{
					index = KZMathKit.LoopClamp(index,entryInfoCount);
				}

				if(index < 0 || index >= entryInfoCount || location > 1.0f)
				{
					slot.gameObject.EnsureActive(false);
					m_boundEntryIndexMap.Remove(slot);

					continue;
				}

				var needsBind = isForceRefresh
					|| !slot.gameObject.activeSelf
					|| !m_boundEntryIndexMap.TryGetValue(slot,out var boundIndex)
					|| boundIndex != index;

				if(needsBind)
				{
					slot.gameObject.EnsureActive(true);
					slot.name = $"Slot_{i}";
					slot.SetEntryInfo(m_entryInfoList[index]);
					m_boundEntryIndexMap[slot] = index;
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

		private void Reset()
		{
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