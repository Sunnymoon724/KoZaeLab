#if KZLIB_IN_APP_PURCHASE
/// <summary>
/// Result codes returned by <see cref="InAppPurchaseManager.PurchaseProductAsync"/>.
/// </summary>
public enum InAppPurchaseResultType
{
	Unknown,

	Success,

	AlreadyPurchased,
	NetworkError,
	StoreControllerNotInitialized,
	ProductNotFound,

	UserCancelled,
	PaymentDeclined,
	ProductUnavailable,
}
#endif