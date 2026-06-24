using R3;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Drag handler that rotates a target <see cref="Transform"/> from pointer delta. Emits <see cref="OnSpinedImage"/> on each drag frame.
/// </summary>
public class SpinImage : BaseImage,IDragHandler
{
	[SerializeField]
	private Transform m_target = null;
	[SerializeField]
	private float m_speed = 1.0f;
	[SerializeField]
	private bool m_lockVertical = false;

	private readonly Subject<Unit> m_imageSubject = new();
	public Observable<Unit> OnSpinedImage => m_imageSubject;

	protected override void _Release()
	{
		m_imageSubject.Dispose();

		base._Release();
	}

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

		m_imageSubject.OnNext(Unit.Default);
	}
}
