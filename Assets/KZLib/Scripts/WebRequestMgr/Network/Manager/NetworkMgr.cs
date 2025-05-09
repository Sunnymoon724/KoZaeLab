using System;
using KZLib.KZData;
using KZLib.KZUtility;

namespace KZLib.KZNetwork
{
	public partial class NetworkMgr : Singleton<NetworkMgr>
	{
		private bool m_disposed = false;

		public event Action<string> OnShowNetworkError = null;

		private EnvironmentType m_currentEnvironment = EnvironmentType.DEV;

		protected override void Initialize()
		{
			var gameCfg = ConfigMgr.In.Access<ConfigData.GameConfig>();

			m_currentEnvironment = gameCfg.CurrentEnvironment;
		}

		protected override void Release(bool disposing)
		{
			if(m_disposed)
			{
				return;
			}

			if(disposing)
			{
				OnShowNetworkError = null;
			}

			m_disposed = true;

			base.Release(disposing);
		}

		public void SendNetworkError(string message)
		{
			OnShowNetworkError?.Invoke(message);
		}
	}
}