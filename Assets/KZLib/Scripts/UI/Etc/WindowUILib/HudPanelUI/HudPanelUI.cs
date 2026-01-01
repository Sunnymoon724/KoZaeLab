using System.Collections.Generic;
using HudPanel;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HudPanel
{
	public interface IHudUI
	{
		void Refresh(float deltaTime,int frameCount);

		bool IsActive { get; }
	}
}

public class HudPanelUI : BasePanelUI
{
	private const float c_frameUpdatePeriod = 0.25f;

	private float m_deltaTime = 0.0f;
	private int m_frameCount = 0;

	[SerializeField,ListDrawerSettings(ShowFoldout = false)]
	private List<IHudUI> m_hudUIList = new();

	private void Update()
	{
		m_deltaTime += Time.deltaTime;
		m_frameCount++;

		if(m_deltaTime > c_frameUpdatePeriod)
		{
			for(var i=0;i<m_hudUIList.Count;i++)
			{
				var hudUI = m_hudUIList[i];

				if(hudUI.IsActive)
				{
					hudUI.Refresh(m_deltaTime,m_frameCount);
				}
			}

			m_deltaTime = 0.0f;
			m_frameCount = 0;
		}
	}
}