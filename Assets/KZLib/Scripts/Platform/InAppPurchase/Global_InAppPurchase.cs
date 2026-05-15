#if KZLIB_IN_APP_PURCHASE
using System.ComponentModel;
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

namespace System.Runtime.CompilerServices
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal class IsExternalInit { }
}
#endif