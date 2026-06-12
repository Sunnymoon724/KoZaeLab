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

			if(UIManager.HasInstance)
			{
				UIManager.In.Open(CommonUINameTag.DebugOverlayPanel);
			}
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

			if(UIManager.HasInstance)
			{
				UIManager.In.Close(CommonUINameTag.DebugOverlayPanel);
			}
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


		private void _RefreshPeriodic(DebugOverlayPanel debugPanel)
		{
			m_deltaTime += Time.deltaTime;
			m_frameCount++;

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