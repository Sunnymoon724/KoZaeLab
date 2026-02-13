#if KZLIB_PLAY_FAB
using System;
using Cysharp.Threading.Tasks;
using KZLib.Utilities;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.SharedModels;

namespace KZLib
{
	public partial class PlayFabManager : Singleton<PlayFabManager>
	{
		public async UniTask<PlayFabPacketInfo> GetPlayerProfileAsync(string playFabId)
		{
			void _SendPacket(PlayFabRequestCommon commonRequest,Action<PlayFabResultCommon> onSendResult,Action<PlayFabError> onSendError)
			{
				var profileRequest = commonRequest as GetPlayerProfileRequest;

				PlayFabClientAPI.GetPlayerProfile(profileRequest,onSendResult,onSendError);
			}

			NetworkPacketInfo _CreatePacket(PlayFabResultCommon commonResult)
			{
				var profileResult = commonResult as GetPlayerProfileResult;

				var message = JsonConvert.SerializeObject(profileResult.PlayerProfile);

				return new NetworkPacketInfo(0,message,false);
			}

			var request = new GetPlayerProfileRequest()
			{
				PlayFabId = playFabId,
				ProfileConstraints = new PlayerProfileViewConstraints() { ShowDisplayName = true }
			};

			return await _ExecuteAsync(request,_SendPacket,_CreatePacket);
		}
	}
}
#endif