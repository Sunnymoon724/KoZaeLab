#if KZLIB_PLAY_FAB
using System.Diagnostics;
using Cysharp.Threading.Tasks;
using KZLib.KZUtility;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;

namespace KZLib
{
	public partial class PlayFabMgr : Singleton<PlayFabMgr>
	{
		public async UniTask<PlayFabPacket> GetPlayerProfileAsync(string playFabId)
		{
			var request = new GetPlayerProfileRequest()
			{
				PlayFabId = playFabId,
				ProfileConstraints = new PlayerProfileViewConstraints() { ShowDisplayName = true }
			};

			var source = new UniTaskCompletionSource<PlayFabPacket>();

			var stopwatch = Stopwatch.StartNew();

			PlayFabClientAPI.GetPlayerProfile(request,(result) =>
			{
				stopwatch.Stop();

				var message = JsonConvert.SerializeObject(result.PlayerProfile);

				source.TrySetResult(new PlayFabPacket(request,new NetworkPacket(0,message,false),stopwatch.ElapsedMilliseconds));
			},(playFabError) =>
			{
				stopwatch.Stop();
				source.TrySetResult(_CreateErrorPacket(playFabError,stopwatch.ElapsedMilliseconds));
			});

			return await source.Task;
		}
	}
}
#endif