using Cysharp.Threading.Tasks;
using KZLib.KZUtility;

#if KZLIB_PLAY_FAB

using Newtonsoft.Json;
using PlayFab.ClientModels;

#endif

namespace KZLib.KZNetwork
{
	public partial class NetworkMgr : Singleton<NetworkMgr>
	{
		public async UniTask<bool> GetPlayerProfileAsync(string sessionTicket)
		{
#if KZLIB_PLAY_FAB
			return await _RequestToServerAsync( async () =>
			{
				var profileId = m_accountCredential.AccountId;

				var packet = await PlayFabMgr.In.GetPlayerProfileAsync(profileId);
				var profileModel = JsonConvert.DeserializeObject<PlayerProfileModel>(packet.RespondPacket.Message); // 필요한 정보 분할은 나중에

				return _GetPlayFabResult("GetPlayerProfileAsync",await PlayFabMgr.In.GetPlayerProfileAsync(profileId));
			},true,string.Empty);
#else
			await UniTask.Yield();

			return false;
#endif
		}
	}
}