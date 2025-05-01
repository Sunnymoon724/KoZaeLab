using Cysharp.Threading.Tasks;
using KZLib.KZUtility;

namespace KZLib.KZNetwork
{
	public partial class NetworkMgr : Singleton<NetworkMgr>
	{
		private async UniTask<ResponseInfo> _SendWebRequest(BaseWebRequest webRequest)
		{
			if(webRequest == null)
			{
				LogTag.Network.E("WebRequest is null");

				return null;
			}

			var responseInfo = await webRequest.SendAsync();

			if(responseInfo.Result)
			{
				LogTag.Network.I($"{webRequest.Name} is Success");
			}
			else
			{
				LogTag.Network.E($"{webRequest.Name} is Failed");

				OnShowNetworkError?.Invoke(responseInfo.Error);
			}

			return responseInfo;
		}
	}
}