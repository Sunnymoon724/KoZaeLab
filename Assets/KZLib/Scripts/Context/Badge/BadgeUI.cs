using System;
using System.Collections.Generic;
using KZLib;
using MessagePipe;
using UnityEngine;

public class BadgeUI : MonoBehaviour
{
	[SerializeField]
	private List<GameObject> m_badgeList = new();
	[SerializeField]
	private BadgeTag m_badgeTag = BadgeTag.NONE;

	private IDisposable m_subscription = null;

	private void OnEnable()
	{
		if(m_badgeTag == BadgeTag.NONE)
		{
			return;
		}

		var currentCount = ContextManager.In.GetBadgeCount(m_badgeTag);
		_RefreshBadge(currentCount);

		m_subscription?.Dispose();
		m_subscription = GlobalMessagePipe.GetSubscriber<BadgeTag,int>().Subscribe(m_badgeTag,_RefreshBadge);
	}

	private void OnDisable()
	{
		m_subscription?.Dispose();
	}

	private void _RefreshBadge(int currentCount)
	{
		var hasBadge = currentCount > 0;

		for(int i=0;i<m_badgeList.Count;i++)
		{
			var badge = m_badgeList[i];

			if(badge)
			{
				badge.SetActive(hasBadge);
			}
		}
	}
}