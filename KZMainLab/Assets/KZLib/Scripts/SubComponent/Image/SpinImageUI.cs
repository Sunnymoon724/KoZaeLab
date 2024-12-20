using KZLib;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpinImageUI : BaseImageUI,IDragHandler
{
	[SerializeField] private Transform m_Target = null;
	[SerializeField] private float m_Speed = 1.0f;
	[SerializeField] private bool m_LockVertical = false;

	public NewAction onDragHandler = new();

	public void SetTarget(Transform _target)
	{
		m_Target = _target;
	}

	void IDragHandler.OnDrag(PointerEventData _data)
	{
		if(!m_Target)
		{
			return;
		}

		var delta = _data.delta;

		if(m_LockVertical)
		{
			delta.y = 0.0f;
		}

		m_Target.localRotation = Quaternion.Euler(0.0f,-0.5f*delta.x*m_Speed,-0.5f*delta.y*m_Speed)*m_Target.localRotation;

		onDragHandler?.Invoke();
	}
}