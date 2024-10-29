using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace KZLib.KZDevelop
{
	public partial class FocusScroller : BaseComponentUI,IPointerUpHandler,IPointerDownHandler,IBeginDragHandler,IEndDragHandler,IDragHandler,IScrollHandler
	{
		#region Scroll Data
		private class ScrollData
		{
			public bool Enable { get; private set; }
			public bool Elastic { get; private set; }
			public float Duration { get; private set; }
			public AnimationCurve Curve { get; private set; }
			public float StartTime { get; private set; }
			public float EndLocation { get; private set; }

			private Action m_OnComplete = null;

			public void SetData(Action _onComplete)
			{
				SetDataInner(true,true,0.0f,EaseType.OutCubic,0.0f,0.0f,_onComplete);
			}

			public void SetData(float _duration,EaseType _type,float _endLocation,Action _onComplete)
			{
				SetDataInner(true,false,_duration,_type,Time.unscaledTime,_endLocation,_onComplete);
			}

			public void Reset()
			{
				SetDataInner();
			}

			public void Complete()
            {
                m_OnComplete?.Invoke();
                Reset();
            }

			private void SetDataInner(bool _enable = false,bool _elastic = false,float _duration = 0.0f,EaseType _type = EaseType.OutCubic,float _startTime = 0.0f,float _endLocation = 0.0f,Action _onComplete = null)
			{
				Enable = _enable;
				Elastic = _elastic;
				Duration = _duration;
				Curve = MathUtility.GetEaseCurve(_type);
				StartTime = _startTime;
				EndLocation = _endLocation;
				m_OnComplete = _onComplete;
			}
		}
		#endregion Scroll Data

		[FoldoutGroup("Movement",Order = 10)]
		[VerticalGroup("Movement/0",Order = 0),SerializeField,LabelText("Elasticity")]
		private float m_Elasticity = 0.1f;
		[VerticalGroup("Movement/0",Order = 0),SerializeField,LabelText("Sensitivity")]
		private float m_sensitivity = 1.0f;
		[VerticalGroup("Movement/0",Order = 0),SerializeField,LabelText("Deceleration Rate")]
		private float m_DecelerationRate = 0.03f;
		[VerticalGroup("Movement/0",Order = 0),SerializeField,LabelText("Inertia")]
		private bool m_Inertia = true;

		[PropertySpace(10)]

		[VerticalGroup("Movement/1",Order = 1),SerializeField,LabelText("Magnet Mode")]
		private bool m_MagnetMode = false;
		[VerticalGroup("Movement/1",Order = 1),SerializeField,LabelText("Velocity"),ShowIf(nameof(m_MagnetMode))]
		private float m_MagnetVelocity = 0.5f;
		[VerticalGroup("Movement/1",Order = 1),SerializeField,LabelText("Duration"),ShowIf(nameof(m_MagnetMode))]
		private float m_MagnetDuration = 0.3f;
		[VerticalGroup("Movement/1",Order = 1),SerializeField,LabelText("Easing Type"),ShowIf(nameof(m_MagnetMode))]
		private EaseType m_MagnetEasing = EaseType.InOutCubic;

		private bool m_Dragging = true;

		private bool m_hold = false;
		private bool m_scrolling = false;
		private float m_Velocity = 0.0f;

		public float Location
		{
			get => m_CurrentLocation;
			set
			{
				m_scrollData.Reset();
				m_Velocity = 0.0f;
				m_Dragging = false;

				UpdateLocation(value,false);
			}
		}

		private readonly ScrollData m_scrollData = new();

		private float m_PrevPosition = 0.0f;

		private Vector2 m_BeginDragPoint = Vector2.zero;
		private float m_scrollStartLocation = 0.0f;

		private float ViewportSize => m_Vertical ? m_Viewport.rect.height : m_Viewport.rect.width;

		private Action m_OnDragStart = null;

		public event Action OnDragStart
		{
			add { m_OnDragStart -= value; m_OnDragStart += value; }
			remove { m_OnDragStart -= value; }
		}

		private Action<float> m_OnDragEnd = null;

		public event Action<float> OnDragEnd
		{
			add { m_OnDragEnd -= value; m_OnDragEnd += value; }
			remove { m_OnDragEnd -= value; }
		}

		public void OnPointerDown(PointerEventData _data)
		{
			if(!m_UseDrag || _data.button != PointerEventData.InputButton.Left)
			{
				return;
			}

			m_hold = true;
			m_Velocity = 0.0f;
			m_scrollData.Reset();
		}

		public void OnPointerUp(PointerEventData _data)
		{
			if(!m_UseDrag || _data.button != PointerEventData.InputButton.Left)
			{
				return;
			}

			if(m_hold && m_MagnetMode)
			{
				var location = Mathf.RoundToInt(m_CurrentLocation);

				ScrollTo(location,m_MagnetDuration,m_MagnetEasing);
			}

			m_hold = false;
		}

		public void OnScroll(PointerEventData _data)
		{
			if(!m_UseDrag)
			{
				return;
			}

			var delta = _data.scrollDelta;

			delta.y *= -1.0f;

			var scrollDelta = m_Vertical ? Mathf.Abs(delta.x) > Mathf.Abs(delta.y) ? delta.x : delta.y : Mathf.Abs(delta.y) > Mathf.Abs(delta.x) ? delta.y : delta.x;

			if(_data.IsScrolling())
			{
				m_scrolling = true;
			}

			var location = m_CurrentLocation + scrollDelta/ViewportSize*m_sensitivity;

			if(m_scrollData.Enable)
			{
				m_scrollData.Reset();
			}

			UpdateLocation(location,false);
		}

		public void OnBeginDrag(PointerEventData _data)
		{
			if(!m_UseDrag || _data.button != PointerEventData.InputButton.Left)
			{
				return;
			}

			m_hold = false;

			RectTransformUtility.ScreenPointToLocalPointInRectangle(m_Viewport,_data.position,_data.pressEventCamera,out m_BeginDragPoint);

			m_scrollStartLocation = m_CurrentLocation;
			m_Dragging = true;
			m_scrollData.Reset();

			m_OnDragStart?.Invoke();
		}

		public void OnDrag(PointerEventData _data)
		{
			if(!m_UseDrag || _data.button != PointerEventData.InputButton.Left || !m_Dragging)
			{
				return;
			}

			if(!RectTransformUtility.ScreenPointToLocalPointInRectangle(m_Viewport,_data.position,_data.pressEventCamera,out var point))
			{
				return;
			}

			var delta = point - m_BeginDragPoint;
			var location = (m_Vertical ? delta.y : -delta.x)/ViewportSize*m_sensitivity+m_scrollStartLocation;

			var offset = GetOffset(location);
			location += offset;

			if(!IsCircularMode)
			{
				if(offset != 0.0f)
				{
					location -= RubberDelta(offset,m_sensitivity);
				}
			}

			UpdateLocation(location,false);
		}

		public void OnEndDrag(PointerEventData _data)
		{
			if(!m_UseDrag || _data.button != PointerEventData.InputButton.Left)
			{
				return;
			}

			m_Dragging = false;
		}

		private void Update()
		{
			var deltaTime = Time.unscaledDeltaTime;
			var offset = GetOffset(m_CurrentLocation);
			var isMoving = m_Dragging || m_scrolling;

			if(m_scrollData.Enable)
			{
				var location = 0.0f;

				if(m_scrollData.Elastic)
				{
					location = Mathf.SmoothDamp(m_CurrentLocation,m_CurrentLocation+offset,ref m_Velocity,m_Elasticity,Mathf.Infinity,deltaTime);

					if(Mathf.Abs(m_Velocity) < 0.001f)
					{
						location = Mathf.Clamp(Mathf.RoundToInt(location),0,m_CellList.Count-1);
						m_Velocity = 0.0f;
						m_scrollData.Complete();
					}
				}
				else
				{
					var alpha = Mathf.Clamp01((Time.unscaledTime-m_scrollData.StartTime)/Mathf.Max(m_scrollData.Duration,float.Epsilon));

					location = Mathf.LerpUnclamped(m_scrollStartLocation,m_scrollData.EndLocation,m_scrollData.Curve.Evaluate(alpha));

					if(alpha.Approximately(1.0f))
					{
						m_scrollData.Complete();
					}
				}

				UpdateLocation(location,false);
			}
			else if(!isMoving && (!offset.ApproximatelyZero() || !m_Velocity.ApproximatelyZero()))
			{
				var location = m_CurrentLocation;

				if(!IsCircularMode && !offset.ApproximatelyZero())
				{
					m_scrollData.SetData(()=>
					{
						UpdateIndex(Mathf.RoundToInt(m_CurrentLocation));
					});

					m_OnDragEnd?.Invoke(m_FadeInDuration);
				}
				else if(m_Inertia)
				{
					m_Velocity *= Mathf.Pow(m_DecelerationRate,deltaTime);

					if(Mathf.Abs(m_Velocity) < 0.001f)
					{
						m_Velocity = 0.0f;
					}

					location += m_Velocity*deltaTime;

					if(m_MagnetMode && Mathf.Abs(m_Velocity) < m_MagnetVelocity)
					{
						ScrollTo(Mathf.RoundToInt(m_CurrentLocation),m_MagnetDuration,m_MagnetEasing);

						m_OnDragEnd?.Invoke(m_MagnetDuration);
					}
				}
				else
				{
					m_Velocity = 0.0f;

					if(!m_MagnetMode)
					{
						m_OnDragEnd?.Invoke(0.0f);
					}
				}

				if(!m_Velocity.ApproximatelyZero())
				{
					UpdateLocation(location,false);
				}
			}

			if(!m_scrollData.Enable && isMoving && m_Inertia)
			{
				var velocity = (m_CurrentLocation-m_PrevPosition)/deltaTime;

				m_Velocity = Mathf.Lerp(m_Velocity,velocity,deltaTime*10.0f);
			}

			m_PrevPosition = m_CurrentLocation;
			m_scrolling = false;
		}

		public void ScrollTo(float _location,float _duration,EaseType _type)
		{
			if(_duration <= 0.0f)
			{
				Location = MathUtility.LoopClamp(_location,m_CellList.Count);

				return;
			}

			var endLocation = m_CurrentLocation+CalculateMovementAmount(m_CurrentLocation,_location);

			m_scrollData.SetData(_duration,_type,endLocation,()=>
			{
				UpdateIndex(Mathf.RoundToInt(m_CurrentLocation));
			});

			m_Velocity = 0.0f;
			m_scrollStartLocation = m_CurrentLocation;
		}

		public void JumpTo(int _index)
		{
			if(m_CellList.ContainsIndex(_index))
			{
				UpdateIndex(_index);
				Location = _index;
			}
		}

		private float RubberDelta(float _stretching,float _size)
		{
			return (1.0f-1.0f/(Mathf.Abs(_stretching)*0.55f/_size+1))*_size*Mathf.Sign(_stretching);
		}

		private float CalculateMovementAmount(float _source,float _destination)
		{
			var count = m_CellList.Count;

			if(!IsCircularMode)
			{
				return Mathf.Clamp(_destination,0,count-1)-_source;
			}

			var amount = MathUtility.LoopClamp(_destination,count)-MathUtility.LoopClamp(_source,count);

			if(Mathf.Abs(amount) > count*0.5f)
			{
				amount = Mathf.Sign(-amount)*(count-Mathf.Abs(amount));
			}

			return amount;
		}

		private float GetOffset(float _location)
		{
			if(IsCircularMode)
			{
				return 0.0f;
			}

			if(_location < 0.0f)
			{
				return -_location;
			}

			var count = m_CellList.Count-1;

			if(_location > count)
			{
				return count-_location;
			}

			return 0.0f;
		}
    }
}