using UnityEngine;

public static class RayExtension
{
	public static Vector3 CalculateClosestPoint(this Ray ray,Vector3 target)
	{
		var position = target-ray.origin;
		var projection = Vector3.Project(position,ray.direction);

		return position - projection;
	}
}