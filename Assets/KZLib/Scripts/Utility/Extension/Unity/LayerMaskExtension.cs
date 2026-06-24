using UnityEngine;

/// <summary>
/// Extension methods for <see cref="LayerMask"/> layer membership checks.
/// </summary>
public static class LayerMaskExtension
{
	/// <summary>
	/// Returns whether <paramref name="layer"/> is included in the mask.
	/// </summary>
	public static bool Contains(this LayerMask layerMask,int layer)
	{
		return (layerMask.value & (1 << layer)) != 0;
	}
}