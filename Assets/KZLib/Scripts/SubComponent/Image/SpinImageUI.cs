using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SpinImageUI : BaseImageUI,IDragHandler
{
	[SerializeField] private Transform m_target = null;
	[SerializeField] private float m_speed = 1.0f;
	[SerializeField] private bool m_lockVertical = false;

	public event UnityAction OnImageSpin;

	public void SetTarget(Transform target)
	{
		m_target = target;
	}

	void IDragHandler.OnDrag(PointerEventData eventData)
	{
		if(!m_target)
		{
			return;
		}

		var delta = eventData.delta;

		if(m_lockVertical)
		{
			delta.y = 0.0f;
		}

		m_target.localRotation = Quaternion.Euler(0.0f,-0.5f*delta.x*m_speed,-0.5f*delta.y*m_speed)*m_target.localRotation;

		OnImageSpin?.Invoke();
	}
}