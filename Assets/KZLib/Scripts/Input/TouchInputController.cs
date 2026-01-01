using System.Collections.Generic;
using MessagePipe;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace KZLib
{
	public class TouchInputController : InputController
	{
		private const string c_touchPress = "TouchPress";

		[VerticalGroup("1",Order = 1),SerializeField]
		private bool m_useTouchEffect = false;

		[VerticalGroup("1",Order = 1),SerializeField,ShowIf(nameof(m_useTouchEffect))]
		private string m_touchEffectName = null;

		protected override void SubscribeInputAction()
		{
			var touchPressAction = _TryGetInputAction(c_touchPress);

			if(touchPressAction != null)
			{
				touchPressAction.started += OnTouchedDown;
				touchPressAction.performed += OnTouchedHolding;
				touchPressAction.canceled += OnTouchedUp;
			}
		}

		protected override void UnsubscribeInputAction()
		{
			var touchPressAction = _TryGetInputAction(c_touchPress);

			if(touchPressAction != null)
			{
				touchPressAction.started -= OnTouchedDown;
				touchPressAction.performed -= OnTouchedHolding;
				touchPressAction.canceled -= OnTouchedUp;
			}
		}

		private void OnTouchedDown(InputAction.CallbackContext context)
		{
			_ProcessInputEvent(context,CommonNoticeTag.TouchDownPoint);
		}

		private void OnTouchedHolding(InputAction.CallbackContext context)
		{
			_ProcessInputEvent(context,CommonNoticeTag.TouchHoldingPoint);
		}

		private void OnTouchedUp(InputAction.CallbackContext context)
		{
			_ProcessInputEvent(context,CommonNoticeTag.TouchUpPoint);
		}

		private bool _TryGetPoint(InputAction.CallbackContext context,out Vector2 point)
		{
			if(context.control.device is Pointer pointerDevice)
			{
				point = pointerDevice.position.ReadValue();

				return true;
			}
			else
			{
				LogSvc.System.E("TouchPress action is position value cannot be read from the device.");

				point = Vector2.zero;

				return false;
			}
		}

		private void _ProcessInputEvent(InputAction.CallbackContext context,CommonNoticeTag noticeTag)
		{
			if(!_TryGetPoint(context,out var point))
			{
				return;
			}

			if(_IsPointerOverUIObject(point))
			{
				GlobalMessagePipe.GetPublisher<CommonNoticeTag,Vector2>().Publish(noticeTag,point);
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

		private bool _IsPointerOverUIObject(Vector2 point)
		{
			var eventData = new PointerEventData(EventSystem.current)
			{
				position = point
			};

			var resultList = new List<RaycastResult>();

			EventSystem.current.RaycastAll(eventData,resultList);

			return resultList.Count > 0;
		}

		private void OnPressedBackButton(InputAction.CallbackContext _)
		{
			if(UIManager.HasInstance)
			{
				UIManager.In.PressBackButton();
			}
		}
	}
}