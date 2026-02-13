using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using KZLib.Development;
using R3;
using Sirenix.OdinInspector;
using System;

namespace UnityEngine.UI
{
	[RequireComponent(typeof(ScrollRect))]
	public class Carousel : BaseComponent,IBeginDragHandler,IEndDragHandler
	{
		[SerializeField]
		ScrollRect m_scrollRect = null;

		[ShowInInspector,ReadOnly]
		private bool IsVertical => m_scrollRect != null && m_scrollRect.vertical;

		[SerializeField]
		private float m_space = 0.0f;

		[SerializeField]
		private Slot m_slot = null;
		[SerializeField]
		private float m_snapDuration = 0.1f;

		[SerializeField]
		private bool m_useAutoScroll = false;
		[SerializeField,ShowIf(nameof(m_useAutoScroll))]
		private float m_autoScrollInterval = 2.0f;
		[SerializeField,ShowIf(nameof(m_useAutoScroll))]
		private float m_autoScrollDuration = 1.0f;

		private float m_slotStep = 0.0f;

		private bool m_initialize = false;
		private GameObjectPoolBinder<Slot,IEntryInfo> m_poolBinder = null;

		private Slot m_centerSlot = null;

		private float LoopBoundary => m_poolBinder == null ? 0.0f : m_slotStep*m_poolBinder.ItemCount*0.5f;

		private readonly Subject<Slot> m_carouselSlotSubject = new();
		public Observable<Slot> OnChangedSlot => m_carouselSlotSubject;

		private readonly Subject<int> m_carouselIndexSubject = new();
		public Observable<int> OnChangedIndex => m_carouselIndexSubject;

		private CancellationTokenSource m_snapTokenSource = null;
		private CancellationTokenSource m_autoScrollTokenSource = null;

		protected override void _Initialize()
		{
			base._Initialize();

			_EnsureInitialized();
		}

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

			m_poolBinder = new GameObjectPoolBinder<Slot,IEntryInfo>(m_slot,m_scrollRect.content,_BindSlot);

			m_initialize = true;
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			m_scrollRect.onValueChanged.AddAction(_RefreshScroll);

			if(m_useAutoScroll)
			{
				_StartAutoScroll();
			}
		}

		protected override void OnDisable()
		{
			base.OnDisable();

			m_scrollRect.onValueChanged.RemoveAction(_RefreshScroll);

			_KillAllTokenSource();
		}

		protected override void _Release()
		{
			base._Release();

			_KillAllTokenSource();
		}

		private void _KillAllTokenSource()
		{
			CommonUtility.KillTokenSource(ref m_snapTokenSource);
			CommonUtility.KillTokenSource(ref m_autoScrollTokenSource);
		}

		public void SetEntryInfoList(List<IEntryInfo> entryInfoList,int index = -1)
		{
			_EnsureInitialized();

			if(!m_poolBinder.TrySetDataList(entryInfoList))
			{
				return;
			}

			var pivot = 0;

			foreach(var slot in m_poolBinder.ItemGroup)
			{
				var location = m_slotStep*pivot++;

				slot.AnchoredPosition = IsVertical ? new Vector2(0.0f,-location) : new Vector2(location,0.0f);
			}

			var focusIndex = index != -1 ? Mathf.Clamp(index,0,entryInfoList.Count) : 0;

			SetTargetIndexImmediate(focusIndex);
		}

		public void Clear()
		{
			_KillAllTokenSource();

			m_poolBinder.Clear();
		}

		public void SetTargetIndexImmediate(int index)
		{
			var target = m_poolBinder.GetItemByIndex(index);

			if(target == null)
			{
				return;
			}

			m_centerSlot = target;

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

			if(m_useAutoScroll)
			{
				_StartAutoScroll();
			}
		}

		private void _RefreshScroll(Vector2 _)
		{
			_RefreshSlotList();
		}

		private void _RefreshSlotList()
		{
			var totalSize = m_poolBinder.ItemCount*m_slotStep;
			var pivot = (IsVertical ? Vector2.up : Vector2.right)*totalSize;

			foreach(var slot in m_poolBinder.ItemGroup)
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
		}

		private float _GetAxisLocation(Vector2 location)
		{
			return IsVertical ? location.y : location.x;
		}

		void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
		{
			m_scrollRect.velocity = Vector2.zero;

			_KillAllTokenSource();
		}

		void IEndDragHandler.OnEndDrag(PointerEventData eventData)
		{
			CommonUtility.RecycleTokenSource(ref m_snapTokenSource);

			m_scrollRect.velocity = Vector2.zero;

			var closestSlot = _FindClosestSlot();

			if(closestSlot == null)
			{
				return;
			}

			_SnapAsync(closestSlot,m_snapDuration,m_snapTokenSource.Token).Forget();
		}

		private async UniTask _SnapAsync(Slot target,float duration,CancellationToken token)
		{
			var content = m_scrollRect.content;
			var viewport = m_scrollRect.viewport;

            var offset = viewport.InverseTransformPoint(viewport.TransformPoint(viewport.rect.center))-viewport.InverseTransformPoint(target.transform.position);

			var delta = IsVertical ? offset.y : offset.x;

			var start = content.anchoredPosition;
            var finish = start+(IsVertical ? new Vector2(0.0f,delta) : new Vector2(delta,0.0f));

			void _ScrollContent(float time)
			{
				content.anchoredPosition = Vector2.Lerp(start,finish,time);
			}

			await CommonUtility.ExecuteProgressAsync(0.0f,1.0f,duration,_ScrollContent,false,null,token);

			m_centerSlot = target;

			m_carouselSlotSubject.OnNext(target);
			m_carouselIndexSubject.OnNext(m_poolBinder.FindIndex(target));

			if(m_useAutoScroll)
			{
				_StartAutoScroll();
			}
		}

		private Slot _FindClosestSlot()
		{
			if(m_poolBinder.ItemCount == 0)
			{
				return null;
			}

			var viewport = m_scrollRect.viewport;
			var center = IsVertical ? viewport.position.y : viewport.position.x;

			var closest = m_poolBinder.GetItemByIndex(0);
			var minimum = float.MaxValue;

			foreach(var slot in m_poolBinder.ItemGroup)
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

		private void _StartAutoScroll()
		{
			if(m_poolBinder != null && m_poolBinder.ItemCount < 2)
			{
				return;
			}

			CommonUtility.RecycleTokenSource(ref m_autoScrollTokenSource);

			if(m_centerSlot == null)
			{
				return;
			}

			_AutoScrollAsync(m_autoScrollTokenSource.Token).Forget();
		}

		private async UniTaskVoid _AutoScrollAsync(CancellationToken token)
		{
			async UniTask _PlayTaskAsync()
			{
				await UniTask.Delay(TimeSpan.FromSeconds(m_autoScrollInterval), cancellationToken: token);

				var nextSlot = m_poolBinder.GetNearItem(m_centerSlot,1,true);

				CommonUtility.RecycleTokenSource(ref m_snapTokenSource);

				await _SnapAsync(nextSlot,m_autoScrollDuration,m_snapTokenSource.Token);
			}

			await CommonUtility.LoopUniTaskAsync(_PlayTaskAsync,-1,token).SuppressCancellationThrow();
		}

		protected override void Reset()
		{
			base.Reset();

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