using Sirenix.OdinInspector;
using UnityEngine;
using KZLib.KZAttribute;
using UnityEngine.EventSystems;

namespace KZLib
{
	public class TouchMgr : LoadSingletonMB<TouchMgr>
	{
		private bool m_Block = false;

		[VerticalGroup("0",Order = 0),ShowInInspector,LabelText("블락 상태"),KZIsValid("블락 아님","블락 중")]
		public bool IsBlock => !m_Block;

		[VerticalGroup("1",Order = 1),SerializeField,LabelText("터치 이펙트 사용")]
		private bool m_UseTouchEffect = false;

		[VerticalGroup("1",Order = 1),SerializeField,LabelText("터치 이펙트 이름"),ShowIf(nameof(m_UseTouchEffect))]
		private string m_TouchEffectName = null;

		private void Update()
		{
			if(m_Block)
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
				ProcessInputEvent(EventTag.TouchDragPoint,Input.mousePosition,-1);
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
						ProcessInputEvent(EventTag.TouchDownPoint,touch.position,touch.fingerId);
						break;
					case TouchPhase.Moved:
						ProcessInputEvent(EventTag.TouchDragPoint,touch.position,touch.fingerId);
						break;
					case TouchPhase.Ended:
						ProcessInputEvent(EventTag.TouchUpPoint,touch.position,touch.fingerId);
						break;
				}
			}
#endif
		}

		private void ProcessInputEvent(EventTag _eventTag,Vector2 _point,int _pointerId)
		{
			if(!EventSystem.current.IsPointerOverGameObject(_pointerId))
			{
				var worldPoint = CameraMgr.In.CurrentCamera.ScreenToWorldPoint(_point).ToVector2();

				Broadcaster.SendEvent(_eventTag,worldPoint);
			}

			PlayTouchEffect(_point);
		}

		private void PlayTouchEffect(Vector2 _point)
		{
			if(m_UseTouchEffect)
			{
				var point = new Vector2(_point.x-Screen.width/2.0f,_point.y-Screen.height/2.0f)/100.0f;

				EffectMgr.In.PlayEffect(m_TouchEffectName,point,transform);
			}
		}

		public void BlockInput(bool _block)
		{
			m_Block = _block;
		}
	}
}