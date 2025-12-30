using Cysharp.Threading.Tasks;
using KZLib.KZUtility;

namespace KZLib.KZNetwork
{
	public partial class NetworkManager : Singleton<NetworkManager>
	{
		public async UniTask<bool> RequestPurchaseShopProductPaymentAsync(string productId,string platform,string purchaseToken)
		{
			
			await UniTask.Yield();

			return true;
		}
	}
}