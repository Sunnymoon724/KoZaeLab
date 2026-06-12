using System;
using UnityEngine;

/// <summary>
/// Utility methods for releasing unused assets and inspecting managed memory usage.
/// </summary>
public static class KZMemoryKit
{
	/// <summary>
	/// Unloads unused Unity assets and forces a garbage collection pass.
	/// </summary>
	public static void ClearUnloadedAssetMemory()
	{
		Resources.UnloadUnusedAssets();

		GC.Collect();
	}

	/// <summary>
	/// Returns the total managed memory in bytes after a full GC collection.
	/// </summary>
	public static long MemoryCheck()
	{
		return GC.GetTotalMemory(true);
	}
}
