using UnityEngine;

/// <summary>
/// Extension methods for <see cref="Ray"/> closest-point calculations.
/// </summary>
public static class RayExtension
{
	/// <summary>
	/// Returns the point on the ray closest to the given target position.
	/// </summary>
	public static Vector3 CalculateClosestPoint(this Ray ray,Vector3 target)
	{
		var projection = Vector3.Project(target-ray.origin,ray.direction);

		return ray.origin+projection;
	}
}