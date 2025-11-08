#if KZLIB_PLAY_FAB
using System.Collections.Generic;
using System.Diagnostics;
using Cysharp.Threading.Tasks;
using KZLib.KZUtility;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;

namespace KZLib
{
	public partial class PlayFabManager : Singleton<PlayFabManager>
	{
		public async UniTask<PlayFabPacket> ExecuteCloudScriptAsync(string functionName,object parameter)
		{
			var request = new ExecuteCloudScriptRequest()
			{
				FunctionName = functionName,
				FunctionParameter = parameter,
				GeneratePlayStreamEvent = true,
			};

            var source = new UniTaskCompletionSource<PlayFabPacket>();
            var stopwatch = Stopwatch.StartNew();

			PlayFabClientAPI.ExecuteCloudScript(request,(result) =>
			{
				stopwatch.Stop();

				var text = JsonConvert.SerializeObject(result.FunctionResult);
				var packet = JsonConvert.DeserializeObject<NetworkPacket>(text);

				source.TrySetResult(new PlayFabPacket(request,packet,stopwatch.ElapsedMilliseconds));
			},(playFabError) =>
			{
				stopwatch.Stop();
				source.TrySetResult(_CreateErrorPacket(playFabError,stopwatch.ElapsedMilliseconds));
			});

			return await source.Task;
		}
		
		private PlayFabPacket _CreateErrorPacket(PlayFabError playFabError,long duration)
		{
			var code = (int)playFabError.Error;
			var errorText = JsonConvert.SerializeObject(new Dictionary<string,string>()
			{
				{ "Message",playFabError.ErrorMessage },
				{ "Details",JsonConvert.SerializeObject(playFabError.ErrorDetails,Formatting.Indented) }
			});

			return new PlayFabPacket(playFabError,new NetworkPacket(code,errorText,false),duration);
		}
	}
}
#endif