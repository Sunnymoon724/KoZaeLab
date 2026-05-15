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
		public async UniTask<PlayFabPacketInfo> ExecuteCloudScriptAsync(string functionName,object parameter)
		{
			void _SendPacket(PlayFabRequestCommon commonRequest,Action<PlayFabResultCommon> onSendResult,Action<PlayFabError> onSendError)
			{
				var cloudRequest = commonRequest as ExecuteCloudScriptRequest;

				PlayFabClientAPI.ExecuteCloudScript(cloudRequest,onSendResult,onSendError);
			}

			NetworkPacketInfo _CreatePacket(PlayFabResultCommon commonResult)
			{
				var cloudResult = commonResult as ExecuteCloudScriptResult;
				var resultText = JsonConvert.SerializeObject(cloudResult.FunctionResult);

				return JsonConvert.DeserializeObject<NetworkPacketInfo>(resultText);
			}

			var request = new ExecuteCloudScriptRequest()
			{
				FunctionName = functionName,
				FunctionParameter = parameter,
				GeneratePlayStreamEvent = true,
			};

			return await _ExecuteAsync(request,_SendPacket,_CreatePacket);
		}
	}
}
#endif