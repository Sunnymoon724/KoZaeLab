using Cysharp.Threading.Tasks;
using KZLib.Utilities;

namespace KZLib.Networks
{
	public partial class NetworkManager : Singleton<NetworkManager>
	{
		/// <summary>
		/// Validates an in-app purchase receipt with the backend server.
		/// Incomplete: server purchase validation is not implemented yet.
		/// </summary>
		public async UniTask<bool> RequestPurchaseShopProductPaymentAsync(string productId,string platform,string purchaseToken)
		{
			// Incomplete: server purchase validation is not implemented yet.

			await UniTask.Yield();

			return false;
		}
	}
}