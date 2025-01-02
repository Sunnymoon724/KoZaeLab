using System.Collections.Generic;
using HudPanel;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HudPanel
{
	public interface IHudUI
	{
		void Refresh(float _deltaTime,int _frameCount);

		bool IsActive { get; }
	}
}

public class HudPanelUI : WindowUI2D
{
	public override UITag Tag => UITag.HudPanelUI;

	private float m_DeltaTime = 0.0f;
	private int m_FrameCount = 0;

	[SerializeField,ListDrawerSettings(ShowFoldout = false)]
	private List<IHudUI> m_HudUIList = new();

	protected override void Update()
	{
		base.Update();

		m_DeltaTime += Time.deltaTime;
		m_FrameCount++;

		if(m_DeltaTime > Global.FRAME_UPDATE_PERIOD)
		{
			foreach(var hud in m_HudUIList)
			{
				if(hud.IsActive)
				{
					hud.Refresh(m_DeltaTime,m_FrameCount);
				}
			}

			m_DeltaTime = 0.0f;
			m_FrameCount = 0;
		}
	}
}