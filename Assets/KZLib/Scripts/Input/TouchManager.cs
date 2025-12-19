using Sirenix.OdinInspector;
using UnityEngine;
using KZLib.KZAttribute;
using KZLib.KZUtility;
using UnityEngine.EventSystems;
using MessagePipe;

namespace KZLib
{
	/// <summary>
	/// Touch Event is Down, Holding(Drag), Up
	/// </summary>
	public class TouchManager : LoadSingletonMB<TouchManager>
	{
		private bool m_block = false;

		[VerticalGroup("0",Order = 0),ShowInInspector,KZIsValid("No","Yes")]
		public bool IsBlock => !m_block;

		[VerticalGroup("1",Order = 1),SerializeField]
		private bool m_useTouchEffect = false;

		[VerticalGroup("1",Order = 1),SerializeField,ShowIf(nameof(m_useTouchEffect))]
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
				_ProcessInputEvent(CommonNoticeTag.TouchDownPoint,Input.mousePosition,-1);
			}
			else if(Input.GetMouseButton(0))
			{
				_ProcessInputEvent(CommonNoticeTag.TouchHoldingPoint,Input.mousePosition,-1);
			}
			else if(Input.GetMouseButtonUp(0))
			{
				_ProcessInputEvent(CommonNoticeTag.TouchUpPoint,Input.mousePosition,-1);
			}
#elif UNITY_IOS || UNITY_ANDROID
			if(Input.touchCount > 0)
			{
				var touch = Input.GetTouch(0);

				switch (touch.phase)
				{
					case TouchPhase.Began:
					{
						_ProcessInputEvent(CommonNoticeTag.TouchDownPoint,touch.position,touch.fingerId);
						break;
					}
					case TouchPhase.Moved:
					case TouchPhase.Stationary:
					{
						_ProcessInputEvent(CommonNoticeTag.TouchHoldingPoint,touch.position,touch.fingerId);
						break;
					}
					case TouchPhase.Ended:
					{
						_ProcessInputEvent(CommonNoticeTag.TouchUpPoint,touch.position,touch.fingerId);
						break;
					}
				}
			}
#endif
		}

		private void _ProcessInputEvent(CommonNoticeTag noticeTag,Vector2 point,int pointerId)
		{
			if(EventSystem.current != null && !EventSystem.current.IsPointerOverGameObject(pointerId))
			{
				var worldPoint = CameraManager.In.CurrentCamera.ScreenToWorldPoint(point).ToVector2();

				GlobalMessagePipe.GetPublisher<CommonNoticeTag,Vector2>().Publish(noticeTag,worldPoint);
			}

			_PlayTouchEffect(point);
		}

		private void _PlayTouchEffect(Vector2 point)
		{
			if(m_useTouchEffect)
			{
				var touchPoint = new Vector2(point.x-Screen.width/2.0f,point.y-Screen.height/2.0f)/100.0f;

				EffectManager.In.PlayEffect(m_touchEffectName,touchPoint,transform);
			}
		}

		public void BlockInput(bool isBlocked)
		{
			m_block = isBlocked;
		}
	}
}