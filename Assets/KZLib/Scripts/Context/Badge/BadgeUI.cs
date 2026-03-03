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
		if(m_badgeTag != BadgeTag.NONE)
		{
			m_subscription = GlobalMessagePipe.GetSubscriber<BadgeTag,bool>().Subscribe(m_badgeTag,_RefreshBadge);

			var badgeCtx = ContextManager.In.Access<IBadgeContext>();

			_RefreshBadge(badgeCtx.HasBadge(m_badgeTag));
		}
	}

	private void OnDisable()
	{
		m_subscription?.Dispose();
	}

	private void _RefreshBadge(bool hasBadge)
	{
		var badge = hasBadge;

		for(int i=0;i<m_badgeList.Count;i++)
		{
			m_badgeList[i].SetActive( badge );
		}
	}
}