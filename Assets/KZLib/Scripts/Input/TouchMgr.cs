using Sirenix.OdinInspector;
using UnityEngine;
using KZLib.KZAttribute;
using KZLib.KZUtility;
using UnityEngine.EventSystems;

namespace KZLib
{
	/// <summary>
	/// Touch Event is Down, Holding(Drag), Up
	/// </summary>
	public class TouchMgr : LoadSingletonMB<TouchMgr>
	{
		private bool m_block = false;

		[VerticalGroup("0",Order = 0),ShowInInspector,LabelText("Is Block"),KZIsValid("No","Yes")]
		public bool IsBlock => !m_block;

		[VerticalGroup("1",Order = 1),SerializeField,LabelText("Use Touch Effect")]
		private bool m_useTouchEffect = false;

		[VerticalGroup("1",Order = 1),SerializeField,LabelText("Touch Effect Name"),ShowIf(nameof(m_useTouchEffect))]
		private string m_touchEffectName = null;

		private void Update()
		{
			if(m_block)
			{
				return;
			}

#if UNITY_EDITOR || UNITY_STANDALONE
			if(Input.GetMouseButtonDown(0))
			{
				ProcessInputEvent(EventTag.TouchDownPoint,Input.mousePosition,-1);
			}
			else if(Input.GetMouseButton(0))
			{
				ProcessInputEvent(EventTag.TouchHoldingPoint,Input.mousePosition,-1);
			}
			else if(Input.GetMouseButtonUp(0))
			{
				ProcessInputEvent(EventTag.TouchUpPoint,Input.mousePosition,-1);
			}
#elif UNITY_IOS || UNITY_ANDROID
			if(Input.touchCount > 0)
			{
				var touch = Input.GetTouch(0);

				switch (touch.phase)
				{
					case TouchPhase.Began:
					{
						ProcessInputEvent(EventTag.TouchDownPoint,touch.position,touch.fingerId);
						break;
					}
					case TouchPhase.Moved:
					case TouchPhase.Stationary:
					{
						ProcessInputEvent(EventTag.TouchHoldingPoint,touch.position,touch.fingerId);
						break;
					}
					case TouchPhase.Ended:
					{
						ProcessInputEvent(EventTag.TouchUpPoint,touch.position,touch.fingerId);
						break;
					}
				}
			}
#endif
		}

		private void ProcessInputEvent(EventTag eventTag,Vector2 point,int pointerId)
		{
			if(EventSystem.current != null && !EventSystem.current.IsPointerOverGameObject(pointerId))
			{
				var worldPoint = CameraMgr.In.CurrentCamera.ScreenToWorldPoint(point).ToVector2();

				EventMgr.In.SendEvent(eventTag,worldPoint);
			}

			PlayTouchEffect(point);
		}

		private void PlayTouchEffect(Vector2 point)
		{
			if(m_useTouchEffect)
			{
				var touchPoint = new Vector2(point.x-Screen.width/2.0f,point.y-Screen.height/2.0f)/100.0f;

				EffectMgr.In.PlayEffect(m_touchEffectName,touchPoint,transform);
			}
		}

		public void BlockInput(bool isBlocked)
		{
			m_block = isBlocked;
		}
	}
}