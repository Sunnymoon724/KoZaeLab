#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
using MessagePipe;
using UnityEngine;
using R3;

/// <summary>
/// Fits this <see cref="RectTransform"/> to <see cref="Screen.safeArea"/> (notch, home indicator, etc.).
/// Listens for <see cref="CommonNoticeTag.ChangedDeviceResolution"/> published by <see cref="KZLib.ResolutionMonitor"/>.
/// Attach to a full-screen stretch root under Canvas; child UI should be parented below this object.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class SafeAreaChecker : MonoBehaviour
{
	[SerializeField]
	private RectTransform m_rootRect = null;

	private void Awake()
	{
		if(!m_rootRect)
		{
			m_rootRect = GetComponent<RectTransform>();
		}

		GlobalMessagePipe.GetSubscriber<CommonNoticeTag,Unit>().Subscribe(CommonNoticeTag.ChangedDeviceResolution,ApplyResolution).RegisterTo(destroyCancellationToken);

		ApplyResolution(Unit.Default);
	}

	/// <summary>Maps pixel safe area to normalized anchors and resets offsets so the rect fills that region.</summary>
	private void ApplyResolution(Unit _)
	{
		if(!m_rootRect)
		{
			return;
		}

		var width = Screen.width;
		var height = Screen.height;

		// Screen metrics can be zero briefly at startup; ResolutionMonitor will publish again once valid.
		if(width <= 0 || height <= 0)
		{
			return;
		}

		var safeArea = Screen.safeArea;
		var anchorMin = safeArea.position;
		var anchorMax = anchorMin + safeArea.size;

		anchorMin.x /= width;
		anchorMax.x /= width;
		anchorMin.y /= height;
		anchorMax.y /= height;

		// Skip invalid normalized rectangles instead of applying a broken layout.
		if(anchorMin.x < 0.0f || anchorMin.y < 0.0f || anchorMax.x > 1.0f || anchorMax.y > 1.0f || anchorMax.x <= anchorMin.x || anchorMax.y <= anchorMin.y)
		{
			return;
		}

		m_rootRect.anchorMin = anchorMin;
		m_rootRect.anchorMax = anchorMax;
		// Clear stretch offsets so anchors alone define the safe region.
		m_rootRect.offsetMin = Vector2.zero;
		m_rootRect.offsetMax = Vector2.zero;
	}

	private void Reset()
	{
		if(!m_rootRect)
		{
			m_rootRect = GetComponent<RectTransform>();
		}
	}
}
#endif