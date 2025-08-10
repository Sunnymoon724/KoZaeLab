#if KZLIB_PLAY_FAB
using PlayFab;

public enum PlayFabLoginOptionType
{
	None,
	GuestLogin,
	GoogleLogin,
	AppleLogin,
}

public record PlayFabPacket(object RequestPacket,NetworkPacket RespondPacket,long Duration);

#endif