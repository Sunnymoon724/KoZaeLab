using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventSystem))]
public class AdjustDragThreshold : MonoBehaviour
{
	[SerializeField]
	private EventSystem m_eventSystem = null;

	[SerializeField]
	private bool m_runOnAwake = true;

	private const int c_reference_dpi = 100;
	private const float c_reference_pixel_drag = 5.0f;

	private void Awake()
	{
		if(m_runOnAwake)
		{
			_SetPixelDrag(Screen.dpi);
		}
	}

	private void _SetPixelDrag(float screen_dpi)
	{
		if(!m_eventSystem)
		{
			LogChannel.System.E("EventSystem is null");

			return;
		}

		m_eventSystem.pixelDragThreshold = Mathf.RoundToInt(screen_dpi/c_reference_dpi*c_reference_pixel_drag);
	}

	private void Reset()
	{
		if(!m_eventSystem) 
		{
			m_eventSystem = GetComponent<EventSystem>();
		}
	}
}