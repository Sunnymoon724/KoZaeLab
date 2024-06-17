using UnityEngine;
using System.Collections;

public static class RayExtension
{
	public static Vector3 GetClosestPoint(this Ray _ray,Vector3 _position)
	{
		var position = _position-_ray.origin;
		var projection = Vector3.Project(position,_ray.direction);

		return position - projection;
	}
}