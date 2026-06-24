using MessagePipe;
using UnityEngine;
using UnityEngine.EventSystems;
using R3;

/// <summary>
/// Scales <see cref="EventSystem.pixelDragThreshold"/> by <see cref="Screen.dpi"/> so drag distance feels consistent across devices.
/// Listens for <see cref="CommonNoticeTag.ChangedDeviceResolution"/> published by <see cref="KZLib.ResolutionMonitor"/>.
/// Attach to the same GameObject as <see cref="EventSystem"/>.
/// </summary>
[RequireComponent(typeof(EventSystem))]
public class AdjustDragThreshold : MonoBehaviour
{
	[SerializeField]
	private EventSystem m_eventSystem = null;

	/// <summary>When false, drag threshold is left at the EventSystem default.</summary>
	[SerializeField]
	private bool m_runOnAwake = true;

	/// <summary>Baseline DPI; also used as fallback when <see cref="Screen.dpi"/> is unknown.</summary>
	private const int c_referenceDpi = 100;
	/// <summary>Drag threshold in pixels at <see cref="c_referenceDpi"/> (~0.05 inch of finger movement).</summary>
	private const float c_referencePixelDrag = 5.0f;
	/// <summary>Prevents a zero threshold from turning every touch into a drag.</summary>
	private const int c_minPixelDrag = 1;
	/// <summary>Caps drag distance on abnormally high reported DPI values.</summary>
	private const int c_maxPixelDrag = 50;

	private void Awake()
	{
		if(!m_eventSystem)
		{
			m_eventSystem = GetComponent<EventSystem>();
		}

		if(!m_runOnAwake)
		{
			return;
		}

		GlobalMessagePipe.GetSubscriber<CommonNoticeTag,Unit>().Subscribe(CommonNoticeTag.ChangedDeviceResolution,ApplyDragThreshold).RegisterTo(destroyCancellationToken);

		ApplyDragThreshold(Unit.Default);
	}

	/// <summary>Maps DPI to pixel drag threshold, using reference DPI when <see cref="Screen.dpi"/> is unknown.</summary>
	private void ApplyDragThreshold(Unit _)
	{
		if(!m_eventSystem)
		{
			LogChannel.UI.E("EventSystem is null");

			return;
		}

		var screenDpi = Screen.dpi;

		// Editor and some desktop platforms report 0; use reference DPI to avoid a zero threshold.
		if(screenDpi <= 0.0f)
		{
			screenDpi = c_referenceDpi;
		}

		var pixelDrag = Mathf.RoundToInt(screenDpi/c_referenceDpi*c_referencePixelDrag);

		// Keep threshold within a sane pixel range after DPI scaling.
		m_eventSystem.pixelDragThreshold = Mathf.Clamp(pixelDrag,c_minPixelDrag,c_maxPixelDrag);
	}

	private void Reset()
	{
		if(!m_eventSystem)
		{
			m_eventSystem = GetComponent<EventSystem>();
		}
	}
}