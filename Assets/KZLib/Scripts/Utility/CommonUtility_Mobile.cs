using UnityEngine;

public static partial class CommonUtility
{
	public static float GetBatteryAmount() => SystemInfo.batteryLevel;
	public static BatteryStatus GetBatteryStatus() => SystemInfo.batteryStatus;
}