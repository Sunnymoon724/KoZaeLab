#if KZLIB_PLAY_FAB
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cysharp.Threading.Tasks;
using KZLib.Utilities;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.SharedModels;

namespace KZLib
{
	public partial class PlayFabManager : Singleton<PlayFabManager>
	{
		private PlayFabManager() { }

		protected override void _Release(bool disposing)
		{
			if(disposing)
			{
				
			}

			base._Release(disposing);
		}

		private async UniTask<PlayFabPacketInfo> _ExecuteAsync(PlayFabRequestCommon request,Action<PlayFabRequestCommon,Action<PlayFabResultCommon>,Action<PlayFabError>> onSendPacket,Func<PlayFabResultCommon,NetworkPacketInfo> onCreateNetworkPacket)
		{
            var source = new UniTaskCompletionSource<PlayFabPacketInfo>();
            var stopwatch = Stopwatch.StartNew();

			void _SendResult(PlayFabResultCommon result)
			{
				stopwatch.Stop();
				source.TrySetResult(new PlayFabPacketInfo(request,onCreateNetworkPacket(result),stopwatch.ElapsedMilliseconds));
			}

			void _SendError(PlayFabError playFabError)
			{
				stopwatch.Stop();
				source.TrySetResult(_CreateErrorPacket(playFabError,stopwatch.ElapsedMilliseconds));
			}

			onSendPacket(request,_SendResult,_SendError);

			return await source.Task;
		}

		private PlayFabPacketInfo _CreateErrorPacket(PlayFabError playFabError,long duration)
		{
			var code = (int) playFabError.Error;
			var errorText = JsonConvert.SerializeObject(new Dictionary<string,string>()
			{
				{ "Message",playFabError.ErrorMessage },
				{ "Details",JsonConvert.SerializeObject(playFabError.ErrorDetails,Formatting.Indented) }
			});

			return new PlayFabPacketInfo(playFabError,new NetworkPacketInfo(code,errorText,false),duration);
		}
	}
}
#endif