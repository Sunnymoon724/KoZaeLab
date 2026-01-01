using System;
using System.Collections.Generic;
using KZLib;
using MessagePipe;
using UnityEngine;

public class RedDotUI : BaseComponentUI
{
	[SerializeField]
	private List<GameObject> m_redDotList = new();
	[SerializeField]
	private RedDotTag m_redDotTag = RedDotTag.NONE;

	private IDisposable m_subscription = null;

	protected override void OnEnable()
	{
		base.OnEnable();

		if(m_redDotTag != RedDotTag.NONE)
		{
			m_subscription = GlobalMessagePipe.GetSubscriber<RedDotTag,bool>().Subscribe(m_redDotTag,_RefreshRedDot);

			var redDotCtx = ContextManager.In.Access<IRedDotContext>();

			_RefreshRedDot(redDotCtx.HasRedDot(m_redDotTag));
		}
	}

	protected override void OnDisable()
	{
		base.OnDisable();

		m_subscription?.Dispose();
	}

	private void _RefreshRedDot(bool hasRedDot)
	{
		var redDot = hasRedDot;

		for(int i=0;i<m_redDotList.Count;i++)
		{
			m_redDotList[i].SetActive( redDot );
		}
	}
}