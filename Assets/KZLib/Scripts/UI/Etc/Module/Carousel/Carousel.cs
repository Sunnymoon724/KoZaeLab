using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.UI;
using KZLib.Utilities;

namespace KZLib.UI
{
	/// <summary>
	/// Infinite-loop carousel backed by a <see cref="RosterMapper{Slot,IEntryInfo}"/> over <see cref="Slot"/> cells.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Slots are laid out manually on the scroll content and re-wrapped when they move beyond
	/// <see cref="LoopBoundary"/>. Drag end snaps to the nearest slot; optional auto-scroll advances
	/// with <see cref="RosterMapper{Slot,IEntryInfo}.GetNearItem"/>.
	/// </para>
	/// <para>
	/// Creates one active slot per roster entry (not viewport-virtualized). For long lists, prefer
	/// <see cref="FocusScroller"/> or <c>ReuseScrollRect</c>.
	/// </para>
	/// <para>
	/// Assign a <see cref="CarouselSlot"/> or <see cref="FocusSlot"/> pivot when center-only button
	/// interactability is required; base <see cref="Slot"/> cells skip <see cref="IFocusSlot"/> updates.
	/// </para>
	/// <para>
	/// <see cref="OnChangedSlot"/> and <see cref="OnChangedIndex"/> fire after snap or immediate focus changes.
	/// <see cref="Clear"/> emits <see cref="OnChangedIndex"/> with <c>-1</c> only.
	/// </para>
	/// </remarks>
	[RequireComponent(typeof(ScrollRect))]
	public class Carousel : MonoBehaviour,IBeginDragHandler,IEndDragHandler
	{
		[SerializeField]
		private ScrollRect m_scrollRect = null;

		[ShowInInspector,ReadOnly]
		private bool IsVertical => m_scrollRect != null && m_scrollRect.vertical;

		/// <summary>Gap between consecutive slots along the scroll axis, in pixels.</summary>
		[SerializeField]
		private float m_space = 0.0f;

		/// <summary>Template slot cloned by the internal roster mapper.</summary>
		[SerializeField]
		private Slot m_slot = null;

		/// <summary>Duration of the snap animation after drag end.</summary>
		[SerializeField]
		private float m_snapDuration = 0.1f;

		[SerializeField]
		private bool m_useAutoScroll = false;
		/// <summary>Idle time between auto-scroll steps.</summary>
		[SerializeField,ShowIf(nameof(m_useAutoScroll))]
		private float m_autoScrollInterval = 2.0f;
		/// <summary>Duration of each auto-scroll snap animation.</summary>
		[SerializeField,ShowIf(nameof(m_useAutoScroll))]
		private float m_autoScrollDuration = 1.0f;

		/// <summary>Scroll stride along the active axis: slot size plus <see cref="m_space"/>.</summary>
		private float m_slotStep = 0.0f;

		private bool m_initialize = false;

		/// <summary>Pools carousel slots and binds each entry to <see cref="IEntryInfo"/>.</summary>
		private RosterMapper<Slot,IEntryInfo> m_rosterMapper = null;

		/// <summary>Slot currently centered in the viewport after snap or immediate focus.</summary>
		private Slot m_centerSlot = null;

		/// <summary>Half-width of the wrapped slot strip; slots outside this range are repositioned.</summary>
		private float LoopBoundary => m_rosterMapper == null ? 0.0f : m_slotStep*m_rosterMapper.ItemCount*0.5f;

		private readonly Subject<Slot> m_carouselSlotSubject = new();

		/// <summary>Emits the centered slot after snap or immediate focus.</summary>
		public Observable<Slot> OnChangedSlot => m_carouselSlotSubject;

		private readonly Subject<int> m_carouselIndexSubject = new();

		/// <summary>Emits the roster index of the centered slot.</summary>
		public Observable<int> OnChangedIndex => m_carouselIndexSubject;

		/// <summary>Number of active roster entries.</summary>
		public int ItemCount => m_rosterMapper?.ItemCount ?? 0;

		/// <summary>Roster index of the centered slot, or <c>-1</c> when none is focused.</summary>
		public int CenterIndex => m_centerSlot == null || m_rosterMapper == null ? -1 : m_rosterMapper.FindIndex(m_centerSlot);

		private CancellationTokenSource m_snapTokenSource = null;
		private CancellationTokenSource m_autoScrollTokenSource = null;

		private void Awake()
		{
			_EnsureInitialized();
		}

		/// <summary>Creates the roster mapper, measures slot stride, and configures the scroll rect once.</summary>
		private void _EnsureInitialized()
		{
			if(m_initialize)
			{
				return;
			}

			SetScrollRect();

			var slotSize = m_slot.GetSlotSize(IsVertical);

			m_slotStep = slotSize+m_space;

			static void _BindSlot(Slot slot,IEntryInfo entryInfo)
			{
				slot.SetEntryInfo(entryInfo);
			}

			m_rosterMapper = new RosterMapper<Slot,IEntryInfo>(m_slot,m_scrollRect.content,_BindSlot);

			m_initialize = true;
		}

		private void OnEnable()
		{
			m_scrollRect.onValueChanged.AddAction(_RefreshScroll);

			if(m_useAutoScroll)
			{
				_StartAutoScroll();
			}
		}

		private void OnDisable()
		{
			m_scrollRect.onValueChanged.RemoveAction(_RefreshScroll);

			_KillAllTokenSource();
		}

		private void OnDestroy()
		{
			_KillAllTokenSource();

			m_rosterMapper?.Dispose();
			m_carouselSlotSubject.Dispose();
			m_carouselIndexSubject.Dispose();
		}

		private void _KillAllTokenSource()
		{
			KZExternalKit.KillTokenSource(ref m_snapTokenSource);
			KZExternalKit.KillTokenSource(ref m_autoScrollTokenSource);
		}

		/// <summary>Binds <paramref name="entryInfoList"/>, lays out slots, and focuses <paramref name="index"/> when it is not <c>-1</c>.</summary>
		/// <param name="index">Roster index to center. <c>-1</c> centers index <c>0</c>. An empty list calls <see cref="Clear"/>.</param>
		public void SetEntryInfoList(List<IEntryInfo> entryInfoList,int index = -1)
		{
			_EnsureInitialized();

			if(entryInfoList.IsNullOrEmpty())
			{
				Clear();

				return;
			}

			if(!m_rosterMapper.TrySetDataList(entryInfoList))
			{
				return;
			}

			var pivot = 0;

			foreach(var slot in m_rosterMapper.ItemGroup)
			{
				var location = m_slotStep*pivot++;

				slot.AnchoredPosition = IsVertical ? new Vector2(0.0f,-location) : new Vector2(location,0.0f);
			}

			var focusIndex = index != -1 ? Mathf.Clamp(index,0,entryInfoList.Count-1) : 0;

			SetTargetIndexImmediate(focusIndex);
		}

		/// <summary>Cancels async scroll work, returns every slot to the pool, and emits <see cref="OnChangedIndex"/> with <c>-1</c>.</summary>
		public void Clear()
		{
			_KillAllTokenSource();

			if(m_rosterMapper == null)
			{
				m_centerSlot = null;

				return;
			}

			m_rosterMapper.Clear();
			m_centerSlot = null;
			m_carouselIndexSubject.OnNext(-1);
		}

		/// <summary>Moves content so <paramref name="index"/> is centered without animation.</summary>
		public void SetTargetIndexImmediate(int index)
		{
			var target = m_rosterMapper.GetItemByIndex(index);

			if(target == null)
			{
				return;
			}

			var content = m_scrollRect.content;

			if(IsVertical)
			{
				content.anchoredPosition = new Vector2(0.0f,index*m_slotStep);
			}
			else
			{
				content.anchoredPosition = new Vector2(-(index*m_slotStep),0.0f);
			}

			_RefreshSlotList();

			_NotifyFocusChanged(target);
		}

		/// <summary>Focuses <paramref name="index"/> with optional snap animation.</summary>
		/// <param name="duration">Snap duration in seconds. Non-positive values use <see cref="SetTargetIndexImmediate"/>.</param>
		public void SetTargetIndex(int index,float duration = -1.0f)
		{
			if(duration <= 0.0f)
			{
				SetTargetIndexImmediate(index);

				return;
			}

			var target = m_rosterMapper.GetItemByIndex(index);

			if(target == null)
			{
				return;
			}

			KZExternalKit.RecycleTokenSource(ref m_snapTokenSource);

			_SnapAsync(target,duration,m_snapTokenSource.Token).Forget();
		}

		private void _RefreshScroll(Vector2 _)
		{
			_RefreshSlotList();
		}

		/// <summary>Repositions slots that have scrolled beyond the loop boundary, then refreshes focus slots.</summary>
		private void _RefreshSlotList()
		{
			var totalSize = m_rosterMapper.ItemCount*m_slotStep;
			var pivot = (IsVertical ? Vector2.up : Vector2.right)*totalSize;

			foreach(var slot in m_rosterMapper.ItemGroup)
			{
				var location = _GetAxisLocation(slot.AnchoredPosition)+_GetAxisLocation(m_scrollRect.content.anchoredPosition);

				if(location < -LoopBoundary)
				{
					slot.AnchoredPosition += pivot;
				}
				else if(location > LoopBoundary)
				{
					slot.AnchoredPosition -= pivot;
				}
			}

			_RefreshSlotLocations();
		}

		/// <summary>
		/// Updates <see cref="IFocusSlot"/> views with a normalized position along the scroll axis.
		/// <c>0.5</c> means centered in the viewport.
		/// </summary>
		private void _RefreshSlotLocations()
		{
			if(m_slotStep <= 0.0f || m_rosterMapper.ItemCount == 0)
			{
				return;
			}

			var viewport = m_scrollRect.viewport;
			var center = IsVertical ? viewport.position.y : viewport.position.x;
			var locationScale = m_slotStep*2.0f;

			foreach(var slot in m_rosterMapper.ItemGroup)
			{
				if(slot is not IFocusSlot focusSlot)
				{
					continue;
				}

				var position = IsVertical ? slot.transform.position.y : slot.transform.position.x;
				var location = 0.5f-(position-center)/locationScale;

				focusSlot.RefreshLocation(location);
			}
		}

		/// <summary>Stores focus, emits observables, and restarts auto-scroll when the centered slot changes.</summary>
		private void _NotifyFocusChanged(Slot target)
		{
			if(m_centerSlot == target)
			{
				return;
			}

			m_centerSlot = target;

			m_carouselSlotSubject.OnNext(target);
			m_carouselIndexSubject.OnNext(m_rosterMapper.FindIndex(target));

			if(m_useAutoScroll)
			{
				_StartAutoScroll();
			}
		}

		private float _GetAxisLocation(Vector2 location)
		{
			return IsVertical ? location.y : location.x;
		}

		/// <summary>Stops in-flight snap and auto-scroll work when the user starts dragging.</summary>
		void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
		{
			m_scrollRect.velocity = Vector2.zero;

			_KillAllTokenSource();
		}

		void IEndDragHandler.OnEndDrag(PointerEventData eventData)
		{
			KZExternalKit.RecycleTokenSource(ref m_snapTokenSource);

			m_scrollRect.velocity = Vector2.zero;

			var closestSlot = _FindClosestSlot();

			if(closestSlot == null)
			{
				return;
			}

			_SnapAsync(closestSlot,m_snapDuration,m_snapTokenSource.Token).Forget();
		}

		/// <summary>Snap-scrolls content so <paramref name="target"/> sits at the viewport center.</summary>
		private async UniTask _SnapAsync(Slot target,float duration,CancellationToken token)
		{
			var content = m_scrollRect.content;
			var viewport = m_scrollRect.viewport;
			var viewportCenter = viewport.rect.center;
			var targetLocal = viewport.InverseTransformPoint(target.transform.position);
			var delta = IsVertical ? viewportCenter.y-targetLocal.y : viewportCenter.x-targetLocal.x;

			var start = content.anchoredPosition;
			var finish = start+(IsVertical ? new Vector2(0.0f,delta) : new Vector2(delta,0.0f));

			void _ScrollContent(float time)
			{
				content.anchoredPosition = Vector2.Lerp(start,finish,time);
			}

			await KZExternalKit.ExecuteProgressAsync(0.0f,1.0f,duration,_ScrollContent,false,null,token);

			_NotifyFocusChanged(target);
		}

		/// <summary>Returns the slot whose world position is closest to the viewport center.</summary>
		private Slot _FindClosestSlot()
		{
			if(m_rosterMapper.ItemCount == 0)
			{
				return null;
			}

			var viewport = m_scrollRect.viewport;
			var center = IsVertical ? viewport.position.y : viewport.position.x;

			var closest = m_rosterMapper.GetItemByIndex(0);
			var minimum = float.MaxValue;

			foreach(var slot in m_rosterMapper.ItemGroup)
			{
				var position = IsVertical ? slot.transform.position.y : slot.transform.position.x;
				var distance = Mathf.Abs(position-center);

				if(distance < minimum)
				{
					minimum = distance;
					closest = slot;
				}
			}

			return closest;
		}

		/// <summary>Starts or restarts the auto-scroll loop after focus settles.</summary>
		private void _StartAutoScroll()
		{
			if(m_rosterMapper != null && m_rosterMapper.ItemCount < 2)
			{
				return;
			}

			KZExternalKit.RecycleTokenSource(ref m_autoScrollTokenSource);

			if(m_centerSlot == null)
			{
				return;
			}

			_AutoScrollAsync(m_autoScrollTokenSource.Token).Forget();
		}

		/// <summary>Waits <see cref="m_autoScrollInterval"/>, then snaps to the next roster slot in order.</summary>
		private async UniTaskVoid _AutoScrollAsync(CancellationToken token)
		{
			async UniTask _PlayTaskAsync()
			{
				await UniTask.Delay(TimeSpan.FromSeconds(m_autoScrollInterval), cancellationToken: token);

				var nextSlot = m_rosterMapper.GetNearItem(m_centerSlot,1,true);

				if(nextSlot == null)
				{
					return;
				}

				KZExternalKit.RecycleTokenSource(ref m_snapTokenSource);

				await _SnapAsync(nextSlot,m_autoScrollDuration,m_snapTokenSource.Token);
			}

			await KZExternalKit.LoopUniTaskAsync(_PlayTaskAsync,-1,token).SuppressCancellationThrow();
		}

		/// <summary>Editor helper that wires the default Viewport/Content hierarchy on the scroll rect.</summary>
		private void Reset()
		{
			if(!m_scrollRect)
			{
				m_scrollRect = GetComponent<ScrollRect>();
			}

			if(!m_scrollRect.viewport)
			{
				m_scrollRect.viewport = m_scrollRect.transform.Find("Viewport").GetComponent<RectTransform>();
			}

			if(!m_scrollRect.content)
			{
				m_scrollRect.content = m_scrollRect.transform.Find("Viewport/Content").GetComponent<RectTransform>();
			}
		}

		/// <summary>Configures scroll rect anchors and movement for manual infinite-loop layout.</summary>
		public void SetScrollRect()
		{
			m_scrollRect.movementType = ScrollRect.MovementType.Unrestricted;

			var content = m_scrollRect.content;
			var viewport = m_scrollRect.viewport;

			viewport.pivot = new Vector2(0.5f,0.5f);
			viewport.ExpandAnchorSize();

			if(IsVertical)
			{
				content.pivot = new Vector2(0.0f,0.5f);
				content.anchorMin = new Vector2(0.0f,0.0f);
				content.anchorMax = new Vector2(0.0f,1.0f);
			}
			else
			{
				content.pivot = new Vector2(0.5f,1.0f);
				content.anchorMin = new Vector2(0.0f,1.0f);
				content.anchorMax = new Vector2(1.0f,1.0f);
			}
		}
	}
}