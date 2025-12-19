using Cysharp.Threading.Tasks;
using KZLib.KZUtility;

#if KZLIB_PLAY_FAB

using Newtonsoft.Json;
using PlayFab.ClientModels;

#endif

namespace KZLib.KZNetwork
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
				var profileModel = JsonConvert.DeserializeObject<PlayerProfileModel>(packet.RespondPacket.Message); // 필요한 정보 분할은 나중에

				return _GetPlayFabResult("GetPlayerProfileAsync",await PlayFabManager.In.GetPlayerProfileAsync(profileId));
			}

			return await _RequestToServerAsync(_RequestAsync,true,CommonNoticeTag.None);
#else
			await UniTask.Yield();

			return false;
#endif
		}
	}
}