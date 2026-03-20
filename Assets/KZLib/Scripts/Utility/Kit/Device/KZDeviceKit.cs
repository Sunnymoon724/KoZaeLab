using UnityEngine;

public static class KZDeviceKit
{
	public static float GetBatteryAmount() => SystemInfo.batteryLevel;
	public static BatteryStatus GetBatteryStatus() => SystemInfo.batteryStatus;
}