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

public class HudPanelUI : WindowUI2D
{
	private const float c_frame_update_period = 0.25f;

	public override UITag Tag => UITag.HudPanelUI;

	private float m_deltaTime = 0.0f;
	private int m_frameCount = 0;

	[SerializeField,ListDrawerSettings(ShowFoldout = false)]
	private List<IHudUI> m_hudUIList = new();

	protected override void Update()
	{
		base.Update();

		m_deltaTime += Time.deltaTime;
		m_frameCount++;

		if(m_deltaTime > c_frame_update_period)
		{
			foreach(var hudUI in m_hudUIList)
			{
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