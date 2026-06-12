#if KZLIB_IN_APP_PURCHASE
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