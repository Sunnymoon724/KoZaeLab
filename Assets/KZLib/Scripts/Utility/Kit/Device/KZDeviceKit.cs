using UnityEngine;

/// <summary>
/// Utility methods for querying device battery state via Unity SystemInfo.
/// </summary>
public static class KZDeviceKit
{
	/// <summary>
	/// Returns the current battery level as a value between 0.0 and 1.0.
	/// </summary>
	public static float GetBatteryAmount() => SystemInfo.batteryLevel;

	/// <summary>
	/// Returns the current battery charging status.
	/// </summary>
	public static BatteryStatus GetBatteryStatus() => SystemInfo.batteryStatus;
}
