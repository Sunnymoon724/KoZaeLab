using System;
using UnityEngine;

public static class KZMemoryKit
{
	public static void ClearUnloadedAssetMemory()
	{
		Resources.UnloadUnusedAssets();

		GC.Collect();
	}

	public static long MemoryCheck()
	{
		return GC.GetTotalMemory(true);
	}

	public static string GetDownloadSpeed(long tick)
	{
		var size = tick/1024.0d;

		return size > 0.0d ? $"{size / 1024.0d:f2} MB/s" : $"{tick} B/s";
	}
}