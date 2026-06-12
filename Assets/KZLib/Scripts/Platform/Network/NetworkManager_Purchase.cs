using Cysharp.Threading.Tasks;
using KZLib.Utilities;

namespace KZLib.Networks
{
	public partial class NetworkManager : Singleton<NetworkManager>
	{
		public async UniTask<bool> RequestPurchaseShopProductPaymentAsync(string productId,string platform,string purchaseToken)
		{
			// Incomplete: server purchase validation is not implemented yet.

			await UniTask.Yield();

			return false;
		}
	}
}