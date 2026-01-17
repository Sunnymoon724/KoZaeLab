#if UNITY_ANDROID || UNITY_IOS
using MessagePipe;
using UnityEngine;
using R3;

[RequireComponent(typeof(RectTransform))]
public class SafeAreaChecker : BaseComponent
{
	[SerializeField]
	private RectTransform m_rectTrans = null;

	protected override void _Initialize()
	{
		base._Initialize();

		m_rectTrans = GetComponent<RectTransform>();

		GlobalMessagePipe.GetSubscriber<CommonNoticeTag,Unit>().Subscribe(CommonNoticeTag.ChangedDeviceResolution,ApplyResolution).RegisterTo(destroyCancellationToken);

		ApplyResolution(Unit.Default);
	}

	private void ApplyResolution(Unit _)
	{
		var width = Screen.width;
		var height = Screen.height;

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

		if(anchorMin.x >= 0 && anchorMin.y >= 0 && anchorMax.x >= 0 && anchorMax.y >= 0)
		{
			m_rectTrans.anchorMin = anchorMin;
			m_rectTrans.anchorMax = anchorMax;
		}
	}

	protected override void Reset()
	{
		base.Reset();
		
		if(!m_rectTrans)
		{
			m_rectTrans = GetComponent<RectTransform>();
		}
	}
}
#endif