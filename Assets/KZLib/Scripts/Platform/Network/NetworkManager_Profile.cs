using Cysharp.Threading.Tasks;
using KZLib.Utilities;

namespace KZLib.Networks
{
	public partial class NetworkManager : Singleton<NetworkManager>
	{
		public async UniTask<bool> GetPlayerProfileAsync(string sessionTicket)
		{
#if KZLIB_PLAY_FAB
			async UniTask<bool> _RequestAsync()
			{
				var profileId = m_accountCredential.AccountId;
				var packet = await PlayFabManager.In.GetPlayerProfileAsync(profileId);

				// Incomplete: player profile parsing and mapping will be implemented with the server profile flow.

				return _GetPlayFabResult("GetPlayerProfileAsync",packet);
			}

			return await _RequestToServerAsync(_RequestAsync,true,CommonNoticeTag.None);
#else
			await UniTask.Yield();

			return false;
#endif
		}
	}
}