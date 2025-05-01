using KZLib.KZData;
using KZLib.KZUtility;
using UnityEngine.Events;

namespace KZLib.KZNetwork
{
	public partial class NetworkMgr : Singleton<NetworkMgr>
	{
		private bool m_disposed = false;

		public event UnityAction<string> OnShowNetworkError = null;

		private EnvironmentType m_currentEnvironment = EnvironmentType.DEV;

		protected override void Initialize()
		{
			var gameCfg = ConfigMgr.In.Access<ConfigData.GameConfig>();

			m_currentEnvironment = gameCfg.CurrentEnvironment;

			// TODO 네트워크 컨피그에서 url도 읽기.
			// AddAPIInfo( eAPI.API_PLAYER_CREATE, "/api/my/player/create", true );

			// 키값 + address / 터치여부등 옵션들 전부 넣고 거기서 처리하기
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