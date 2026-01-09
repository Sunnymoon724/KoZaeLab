using System;
using R3;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	public partial class FocusScroller : BaseComponentUI,IPointerUpHandler,IPointerDownHandler,IBeginDragHandler,IEndDragHandler,IDragHandler,IScrollHandler
	{
		#region Scroll Info
		private record ScrollInfo
		{
			public bool IsEnable { get; private set; }
			public bool IsElastic { get; private set; }
			public float Duration { get; private set; }
			public AnimationCurve Curve { get; private set; }
			public float StartTime { get; private set; }
			public float EndLocation { get; private set; }

			private Action m_onComplete = null;

			public void Set(Action onComplete)
			{
				_Set(true,true,0.0f,EaseType.OutCubic,0.0f,0.0f,onComplete);
			}

			public void Set(float duration,EaseType easeType,float endLocation,Action onComplete)
			{
				_Set(true,false,duration,easeType,Time.unscaledTime,endLocation,onComplete);
			}

			public void Reset()
			{
				_Set();
			}

			public void Complete()
			{
				m_onComplete?.Invoke();
				Reset();
			}

			private void _Set(bool enable = false,bool elastic = false,float duration = 0.0f,EaseType easeType = EaseType.OutCubic,float startTime = 0.0f,float endLocation = 0.0f,Action onComplete = null)
			{
				IsEnable = enable;
				IsElastic = elastic;
				Duration = duration;
				Curve = CommonUtility.GetEaseCurve(easeType);
				StartTime = startTime;
				EndLocation = endLocation;
				m_onComplete = onComplete;
			}
		}
		#endregion Scroll Info

		[Space(10)]
		[SerializeField]
		private float m_elasticity = 0.1f;
		[SerializeField]
		private float m_sensitivity = 1.0f;
		[SerializeField]
		private float m_decelerationRate = 0.03f;
		[SerializeField]
		private bool m_inertia = true;

		[SerializeField]
		private bool m_magnetMode = false;
		[SerializeField,ShowIf(nameof(m_magnetMode))]
		private float m_magnetVelocity = 0.5f;
		[SerializeField,ShowIf(nameof(m_magnetMode))]
		private float m_magnetDuration = 0.3f;
		[SerializeField,ShowIf(nameof(m_magnetMode))]
		private EaseType m_magnetEasing = EaseType.InOutCubic;

		private bool m_dragging = true;

		private bool m_hold = false;
		private bool m_scrolling = false;
		private float m_velocity = 0.0f;

		public float CurrentLocation
		{
			get => m_currentLocation;
			set
			{
				m_scrollInfo.Reset();
				m_velocity = 0.0f;
				m_dragging = false;

				_RefreshLocation(value,false);
			}
		}

		private readonly ScrollInfo m_scrollInfo = new();

		private float m_prevPosition = 0.0f;

		private Vector2 m_beginDragPoint = Vector2.zero;
		private float m_scrollStartLocation = 0.0f;

		private float ViewportSize => m_vertical ? m_viewport.rect.height : m_viewport.rect.width;

		private readonly Subject<Unit> m_dragStartSubject = new();
		public Observable<Unit> OnStartedDrag => m_dragStartSubject;

		private readonly Subject<float> m_dragEndSubject = new();
		public Observable<float> OnFinishedDrag => m_dragEndSubject;

		void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
		{
			if(!m_useDrag || eventData.button != PointerEventData.InputButton.Left)
			{
				return;
			}

			m_hold = true;
			m_velocity = 0.0f;
			m_scrollInfo.Reset();
		}

		void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
		{
			if(!m_useDrag || eventData.button != PointerEventData.InputButton.Left)
			{
				return;
			}

			if(m_hold && m_magnetMode)
			{
				var location = Mathf.RoundToInt(CurrentLocation);

				ScrollTo(location,m_magnetDuration,m_magnetEasing);
			}

			m_hold = false;
		}

		void IScrollHandler.OnScroll(PointerEventData eventData)
		{
			if(!m_useDrag)
			{
				return;
			}

			var delta = eventData.scrollDelta;

			delta.y *= -1.0f;

			var scrollDelta = m_vertical ? Mathf.Abs(delta.x) > Mathf.Abs(delta.y) ? delta.x : delta.y : Mathf.Abs(delta.y) > Mathf.Abs(delta.x) ? delta.y : delta.x;

			if(eventData.IsScrolling())
			{
				m_scrolling = true;
			}

			var location = CurrentLocation + scrollDelta/ViewportSize*m_sensitivity;

			if(m_scrollInfo.IsEnable)
			{
				m_scrollInfo.Reset();
			}

			_RefreshLocation(location,false);
		}

		void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
		{
			if(!m_useDrag || eventData.button != PointerEventData.InputButton.Left)
			{
				return;
			}

			m_hold = false;

			RectTransformUtility.ScreenPointToLocalPointInRectangle(m_viewport,eventData.position,eventData.pressEventCamera,out m_beginDragPoint);

			m_scrollStartLocation = CurrentLocation;
			m_dragging = true;
			m_scrollInfo.Reset();

			m_dragStartSubject.OnNext(Unit.Default);
		}

		void IDragHandler.OnDrag(PointerEventData eventData)
		{
			if(!m_useDrag || eventData.button != PointerEventData.InputButton.Left || !m_dragging)
			{
				return;
			}

			if(!RectTransformUtility.ScreenPointToLocalPointInRectangle(m_viewport,eventData.position,eventData.pressEventCamera,out var point))
			{
				return;
			}

			var delta = point - m_beginDragPoint;
			var location = (m_vertical ? delta.y : -delta.x)/ViewportSize*m_sensitivity+m_scrollStartLocation;

			var offset = _GetOffset(location);
			location += offset;

			if(!IsCircularMode)
			{
				if(offset != 0.0f)
				{
					location -= _RubberDelta(offset,m_sensitivity);
				}
			}

			_RefreshLocation(location,false);
		}

		void IEndDragHandler.OnEndDrag(PointerEventData eventData)
		{
			if(!m_useDrag || eventData.button != PointerEventData.InputButton.Left)
			{
				return;
			}

			m_dragging = false;
		}

		private void Update()
		{
			var deltaTime = Time.unscaledDeltaTime;
			var offset = _GetOffset(CurrentLocation);
			var isMoving = m_dragging || m_scrolling;

			if(m_scrollInfo.IsEnable)
			{
				var location = 0.0f;

				if(m_scrollInfo.IsElastic)
				{
					location = Mathf.SmoothDamp(CurrentLocation,CurrentLocation+offset,ref m_velocity,m_elasticity,Mathf.Infinity,deltaTime);

					if(Mathf.Abs(m_velocity) < 0.001f)
					{
						location = Mathf.Clamp(Mathf.RoundToInt(location),0,m_entryInfoList.Count-1);
						m_velocity = 0.0f;
						m_scrollInfo.Complete();
					}
				}
				else
				{
					var alpha = Mathf.Clamp01((Time.unscaledTime-m_scrollInfo.StartTime)/Mathf.Max(m_scrollInfo.Duration,float.Epsilon));

					location = Mathf.LerpUnclamped(m_scrollStartLocation,m_scrollInfo.EndLocation,m_scrollInfo.Curve.Evaluate(alpha));

					if(alpha.Approximately(1.0f))
					{
						m_scrollInfo.Complete();
					}
				}

				_RefreshLocation(location,false);
			}
			else if(!isMoving && (!offset.ApproximatelyZero() || !m_velocity.ApproximatelyZero()))
			{
				var location = CurrentLocation;

				if(!IsCircularMode && !offset.ApproximatelyZero())
				{
					m_scrollInfo.Set(_RefreshAll);
				}
				else if(m_inertia)
				{
					m_velocity *= Mathf.Pow(m_decelerationRate,deltaTime);

					if(Mathf.Abs(m_velocity) < 0.001f)
					{
						m_velocity = 0.0f;
					}

					location += m_velocity*deltaTime;

					if(m_magnetMode && Mathf.Abs(m_velocity) < m_magnetVelocity)
					{
						ScrollTo(Mathf.RoundToInt(CurrentLocation),m_magnetDuration,m_magnetEasing);

						m_dragEndSubject.OnNext(m_magnetDuration);
					}
				}
				else
				{
					m_velocity = 0.0f;

					if(!m_magnetMode)
					{
						m_dragEndSubject.OnNext(0.0f);
					}
				}

				if(!m_velocity.ApproximatelyZero())
				{
					_RefreshLocation(location,false);
				}
			}

			if(!m_scrollInfo.IsEnable && isMoving && m_inertia)
			{
				var velocity = (CurrentLocation-m_prevPosition)/deltaTime;

				m_velocity = Mathf.Lerp(m_velocity,velocity,deltaTime*10.0f);
			}

			m_prevPosition = CurrentLocation;
			m_scrolling = false;
		}

		public void ScrollTo(float location,float duration,EaseType easeType)
		{
			if(duration <= 0.0f)
			{
				CurrentLocation = CommonUtility.LoopClamp(location,m_entryInfoList.Count);

				return;
			}

			var endLocation = CurrentLocation+_CalculateLoopedMovementAmount(CurrentLocation,location);

			m_scrollInfo.Set(duration,easeType,endLocation,_RefreshAll);

			m_velocity = 0.0f;
			m_scrollStartLocation = CurrentLocation;
		}

		public void JumpTo(int index)
		{
			if(m_entryInfoList.ContainsIndex(index))
			{
				RefreshIndex(index);
				CurrentLocation = index;
			}
		}

		private float _RubberDelta(float stretching,float size)
		{
			return (1.0f-1.0f/(Mathf.Abs(stretching)*0.55f/size+1))*size*Mathf.Sign(stretching);
		}

		private float _CalculateLoopedMovementAmount(float start,float finish)
		{
			var count = m_entryInfoList.Count;

			if(!IsCircularMode)
			{
				return Mathf.Clamp(finish,0,count-1)-start;
			}

			var amount = CommonUtility.LoopClamp(finish,count)-CommonUtility.LoopClamp(start,count);

			if(Mathf.Abs(amount) > count*0.5f)
			{
				amount = Mathf.Sign(-amount)*(count-Mathf.Abs(amount));
			}

			return amount;
		}

		private float _GetOffset(float location)
		{
			if(IsCircularMode)
			{
				return 0.0f;
			}

			if(location < 0.0f)
			{
				return -location;
			}

			var count = m_entryInfoList.Count-1;

			if(location > count)
			{
				return count-location;
			}

			return 0.0f;
		}

		private void _RefreshAll()
		{
			RefreshIndex(Mathf.RoundToInt(CurrentLocation));
		}
    }
}