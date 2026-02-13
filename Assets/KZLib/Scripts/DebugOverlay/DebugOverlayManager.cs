using KZLib.Utilities;
using UnityEngine;

namespace KZLib
{
	[SingletonConfig(AutoCreate = true,DontDestroy = true)]
	public class DebugOverlayManager : SingletonMB<DebugOverlayManager>
	{
		private const float c_updatePeriod = 0.25f;

		private float m_deltaTime = 0.0f;
		private int m_frameCount = 0;

		private DebugOverlayPanel m_debugPanel;

		private DebugOverlayPanel DebugPanel
		{
			get
			{
				if(!m_debugPanel)
				{
					m_debugPanel = UIManager.In.Find(CommonUINameTag.DebugOverlayPanel) as DebugOverlayPanel;
				}

				return m_debugPanel;
			}
		}

		protected override void _Release()
		{
			base._Release();

			if(UIManager.HasInstance)
			{
				UIManager.In.Close(CommonUINameTag.DebugOverlayPanel);
			}
		}

		public void ShowOverlay()
		{
			if(enabled && _IsOpenedPanel())
			{
				return;
			}

			enabled = true;

			UIManager.In.Open(CommonUINameTag.DebugOverlayPanel);
		}
		
		private bool _IsOpenedPanel()
		{
			return UIManager.HasInstance && UIManager.In.IsOpened(CommonUINameTag.DebugOverlayPanel);
		}

		public void ToggleOverlay()
		{
			if(enabled)
			{
				HideOverlay();
			}
			else
			{
				ShowOverlay();
			}
		}

		public void HideOverlay()
		{
			if(!enabled && !_IsOpenedPanel())
			{
				return;
			}

			enabled = false;

			UIManager.In.Close(CommonUINameTag.DebugOverlayPanel);
		}

		private void Update()
		{
			var debugPanel = DebugPanel; 

			if(!debugPanel)
			{
				return;
			}

			_RefreshImmediate(debugPanel);
			_RefreshPeriodic(debugPanel);
		}

		private void _RefreshImmediate(DebugOverlayPanel debugPanel)
		{
			debugPanel.RefreshImmediate();
		}


		/// <summary>
		/// 일정 주기(c_periodicUpdatePeriod)마다 평균을 내어 갱신해야 하는 정보를 처리합니다.
		/// (예: FPS, 평균 프레임 시간)
		/// </summary>
		private void _RefreshPeriodic(DebugOverlayPanel debugPanel)
		{
			// 1. FPS 카운터 누적
			m_deltaTime += Time.deltaTime;
			m_frameCount++;

			// 2. 갱신 주기가 되었는지 확인
			if (m_deltaTime >= c_updatePeriod)
			{
				var frameRate = Mathf.RoundToInt(m_frameCount/m_deltaTime);
				var frameTime = m_deltaTime/m_frameCount*1000.0f;

				debugPanel.RefreshPeriodic(frameRate,frameTime);

				m_deltaTime = 0.0f;
				m_frameCount = 0;
			}
		}
	}
}