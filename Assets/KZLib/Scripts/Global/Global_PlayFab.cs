#if KZLIB_PLAY_FAB

public enum PlayFabLoginOptionType
{
	None,
	GuestLogin,
	GoogleLogin,
	AppleLogin,
}

public record PlayFabPacketInfo(object RequestPacket,NetworkPacketInfo RespondPacket,long Duration);

#endif