#if KZLIB_PLAY_FAB
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cysharp.Threading.Tasks;
using KZLib.KZUtility;
using PlayFab;
using PlayFab.ClientModels;

namespace KZLib
{
	public partial class PlayFabMgr : Singleton<PlayFabMgr>
	{
		public async UniTask<string> GetMyDataAsync(string key)
		{
			m_userDataResult ??= await _GetUserDataAsync(m_myPlayFabId,"GetMyUserDataAsync");

			return m_userDataResult.Data.TryGetValue(key,out var data) ? data.Value : string.Empty;
		}

		public async UniTask<GetUserDataResult> GetUserDataAsync(string playFabId)
		{
			return await _GetUserDataAsync(playFabId,"GetUserDataAsync");
		}

		private async UniTask<GetUserDataResult> _GetUserDataAsync(string playFabId,string requestMethodName)
		{
			var request = new GetUserDataRequest()
			{
				PlayFabId = playFabId,
			};

			var source = new UniTaskCompletionSource<GetUserDataResult>();

			var stopwatch = Stopwatch.StartNew();

			PlayFabClientAPI.GetUserData(request,(result) =>
			{
				stopwatch.Stop();

				_WriteResult(requestMethodName,request,result,stopwatch.ElapsedMilliseconds);

				source.TrySetResult(result);
			},(playFabError) =>
			{
				stopwatch.Stop();

				_WriteError(requestMethodName,request,playFabError,stopwatch.ElapsedMilliseconds);

				source.TrySetException(new Exception(playFabError.ErrorMessage));
			});

			return await source.Task;
		}

		public async UniTask<UpdateUserDataResult> UpdateUserDataAsync(string key,string value)
		{
			return await UpdateUserDataAsync(new Dictionary<string,string>() { { key,value } });
		}

		public async UniTask<UpdateUserDataResult> UpdateUserDataAsync(Dictionary<string,string> dataDict)
		{
			var requestMethodName = "UpdateUserDataAsync";

			var request = new UpdateUserDataRequest()
			{
				Data = dataDict,
			};

			var source = new UniTaskCompletionSource<UpdateUserDataResult>();

			var stopwatch = Stopwatch.StartNew();

			PlayFabClientAPI.UpdateUserData(request,(result) =>
			{
				stopwatch.Stop();

				_WriteResult(requestMethodName,request,result,stopwatch.ElapsedMilliseconds);

				source.TrySetResult(result);
			},(playFabError) =>
			{
				stopwatch.Stop();

				_WriteError(requestMethodName,request,playFabError,stopwatch.ElapsedMilliseconds);

				source.TrySetException(new Exception(playFabError.ErrorMessage));
			});

			return await source.Task;
		}
	}
}
#endif