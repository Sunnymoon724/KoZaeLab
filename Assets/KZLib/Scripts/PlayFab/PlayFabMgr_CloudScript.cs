#if KZLIB_PLAY_FAB
using System;
using System.Diagnostics;
using Cysharp.Threading.Tasks;
using KZLib.KZUtility;
using PlayFab;
using PlayFab.ClientModels;

namespace KZLib
{
	public partial class PlayFabMgr : Singleton<PlayFabMgr>
	{
		public async UniTask<ExecuteCloudScriptResult> ExecuteCloudScriptAsync(string functionName,object parameter)
		{
			var requestMethodName = "ExecuteCloudScriptAsync";

			var request = new ExecuteCloudScriptRequest()
			{
				FunctionName = functionName,
				FunctionParameter = parameter,
				GeneratePlayStreamEvent = true,
			};

			var source = new UniTaskCompletionSource<ExecuteCloudScriptResult>();

			var stopwatch = Stopwatch.StartNew();

			PlayFabClientAPI.ExecuteCloudScript(request,(result) =>
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