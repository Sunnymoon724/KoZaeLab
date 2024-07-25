using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventSystem))]
public class AdjustDragThreshold : BaseComponent
{
	[SerializeField]
	private EventSystem m_EventSystem = null;

	[SerializeField]
	private bool m_RunOnAwake = true;

	private const int REFERENCE_DPI = 100;
	private const float REFERENCE_PIXEL_DRAG = 5.0f;

	protected override void Initialize()
	{
		base.Initialize();

		if(m_RunOnAwake)
		{
			UpdatePixelDrag(Screen.dpi);
		}
	}

	private void UpdatePixelDrag(float _screenDPI)
	{
		if(!m_EventSystem)
		{
			return;
		}

		m_EventSystem.pixelDragThreshold = Mathf.RoundToInt(_screenDPI/REFERENCE_DPI*REFERENCE_PIXEL_DRAG);
	}

	protected override void Reset()
	{
		base.Reset();

		if(!m_EventSystem) 
		{
			m_EventSystem = GetComponent<EventSystem>();
		}
	}
}