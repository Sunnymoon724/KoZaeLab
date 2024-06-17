using Sirenix.OdinInspector;
using UnityEngine;
using KZLib.KZAttribute;
using KZLib.KZDevelop;
using UnityEngine.EventSystems;

namespace KZLib
{
	public class TouchMgr : LoadSingletonMB<TouchMgr>
	{
		private bool m_Block = false;

#pragma warning disable IDE0051
		[VerticalGroup("0",Order = 0),ShowInInspector,LabelText("블락 상태"),KZIsValid("블락 아님","블락 중")]
		private bool IsBlock => !m_Block;
#pragma warning restore IDE0051

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
				var point = Input.mousePosition.ToVector2();

				if(!EventSystem.current.IsPointerOverGameObject(-1))
				{
					Broadcaster.SendEvent(EventTag.TouchPoint,point);
				}

				PlayTouchEffect(point);
			}

#elif UNITY_IOS || UNITY_ANDROID
			if(Input.touchCount > 0)
			{
				var touch = Input.GetTouch(0);

				if(touch.phase == TouchPhase.Began)
				{
					if(!EventSystem.current.IsPointerOverGameObject(touch.fingerId))
					{
						Broadcaster.SendEvent(EventTag.TouchPoint,touch.position);
					}

					PlayTouchEffect(touch.position);
				}

				
			}
#endif
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