#if UNITY_ANDROID || UNITY_IOS
using System;
using MessagePipe;
using UnityEngine;
using R3;

[RequireComponent(typeof(RectTransform))]
public class SafeAreaChecker : BaseComponentUI
{
	private IDisposable m_subscription = null;

	protected override void Initialize()
	{
		base.Initialize();

		m_subscription = GlobalMessagePipe.GetSubscriber<CommonNoticeTag,Unit>().Subscribe(CommonNoticeTag.ChangedDeviceResolution,ApplyResolution);

		ApplyResolution(Unit.Default);
	}
	
	protected override void Release()
	{
		base.Release();

		m_subscription?.Dispose();
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
			m_rectTransform.anchorMin = anchorMin;
			m_rectTransform.anchorMax = anchorMax;
		}
	}

}
#endif