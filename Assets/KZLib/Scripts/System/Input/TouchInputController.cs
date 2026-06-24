using System.Collections.Generic;
using MessagePipe;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using KZLib.Effects;
using UnityEngine.InputSystem;
using KZLib.Utilities;

public partial class CommonNoticeTag : CustomTag
{
	/// <summary>Screen-space touch phases from <see cref="KZLib.Inputs.TouchInputController"/> (UI hits excluded). Payload: <see cref="UnityEngine.Vector2"/>.</summary>
	public static readonly CommonNoticeTag TouchDownPoint				= new(nameof(TouchDownPoint));
	/// <summary>Input System <c>performed</c>; omitted when it fires on the same frame as Down.</summary>
	public static readonly CommonNoticeTag TouchPressPoint				= new(nameof(TouchPressPoint));
	public static readonly CommonNoticeTag TouchUpPoint					= new(nameof(TouchUpPoint));
}

namespace KZLib.Inputs
{
	/// <summary>
	/// Touch / mouse input via the <c>TouchPress</c> action.
	/// Publishes screen positions on <see cref="CommonNoticeTag"/> (Down / Press / Up) for game-layer listeners.
	/// Events are suppressed over UI; optional FX spawns in world space at the touch point.
	/// </summary>
	public class TouchInputController : InputController
	{
		private const string c_touchPress = "TouchPress";

		private readonly List<RaycastResult> m_raycastResultList = new();

		/// <summary>Skips <see cref="CommonNoticeTag.TouchPressPoint"/> when <c>performed</c> fires on the same frame as <c>started</c>.</summary>
		private int m_lastDownFrame = -1;

		/// <summary>Camera distance passed to <see cref="Camera.ScreenToWorldPoint"/> for touch FX placement.</summary>
		private const float c_touchEffectDistance = 100.0f;

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
				touchPressAction.performed += OnTouchedPress;
				touchPressAction.canceled += OnTouchedUp;
			}
		}

		protected override void UnsubscribeInputAction()
		{
			var touchPressAction = _TryGetInputAction(c_touchPress);

			if(touchPressAction != null)
			{
				touchPressAction.started -= OnTouchedDown;
				touchPressAction.performed -= OnTouchedPress;
				touchPressAction.canceled -= OnTouchedUp;
			}
		}

		private void OnTouchedDown(InputAction.CallbackContext context)
		{
			m_lastDownFrame = Time.frameCount;

			_ProcessInputEvent(context,CommonNoticeTag.TouchDownPoint);
		}

		private void OnTouchedPress(InputAction.CallbackContext context)
		{
			if(Time.frameCount == m_lastDownFrame)
			{
				return;
			}

			_ProcessInputEvent(context,CommonNoticeTag.TouchPressPoint);
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
				LogChannel.Input.E("TouchPress position cannot be read from the device.");

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
				return;
			}

			GlobalMessagePipe.GetPublisher<CommonNoticeTag,Vector2>().Publish(noticeTag,point);

			_PlayTouchEffect(point);
		}

		private void _PlayTouchEffect(Vector2 screenPoint)
		{
			if(!m_useTouchEffect)
			{
				return;
			}

			var camera = CameraManager.HasInstance ? CameraManager.In.CurrentCamera : Camera.main;

			if(!camera)
			{
				return;
			}

			var worldPoint = camera.ScreenToWorldPoint(new Vector3(screenPoint.x,screenPoint.y,c_touchEffectDistance));
			worldPoint.z = 0.0f;

			EffectManager.In.PlayEffect(m_touchEffectName,worldPoint);
		}

		private bool _IsPointerOverUIObject(Vector2 point)
		{
			if(EventSystem.current == null)
			{
				return false;
			}

			var eventData = new PointerEventData(EventSystem.current)
			{
				position = point
			};

			m_raycastResultList.Clear();

			EventSystem.current.RaycastAll(eventData,m_raycastResultList);

			return m_raycastResultList.Count > 0;
		}
	}
}
