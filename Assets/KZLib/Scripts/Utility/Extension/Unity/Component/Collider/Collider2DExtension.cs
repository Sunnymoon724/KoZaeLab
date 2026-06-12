using System;
using UnityEngine;

/// <summary>
/// Extension methods for <see cref="Collider2D"/> overlap and touch detection.
/// </summary>
public static class Collider2DExtension
{
	private static readonly RaycastHit2D[] s_cachedRaycast2DArray = new RaycastHit2D[1];

	public static bool IsOverlapping(this Collider2D collider2D,Collider2D target)
	{
		if(!collider2D || !target)
		{
			return false;
		}

		return collider2D.Distance(target).isOverlapped;
	}

	/// <summary>
	/// Returns true when colliders overlap or their surfaces are touching (zero separation distance).
	/// </summary>
	public static bool IsTouchingNow(this Collider2D collider2D,Collider2D target)
	{
		if(!collider2D || !target)
		{
			return false;
		}

		var colliderDistance = collider2D.Distance(target);

		if(colliderDistance.isOverlapped)
		{
			return true;
		}

		return colliderDistance.distance.ApproximatelyZero();
	}

	public static bool IsOverlapping(this Collider2D collider2D,LayerMask layerMask,Vector2 offset)
	{
		return IsOverlapping(collider2D,layerMask,offset,default);
	}

	public static bool IsOverlapping(this Collider2D collider2D,LayerMask layerMask,Vector2 offset,ContactFilter2D contactFilter2D)
	{
		return IsOverlapping(collider2D,layerMask,offset,contactFilter2D,out _);
	}

	public static bool IsOverlapping(this Collider2D collider2D,LayerMask layerMask,Vector2 offset,out RaycastHit2D raycastHit)
	{
		return IsOverlapping(collider2D,layerMask,offset,default,out raycastHit);
	}

	/// <summary>
	/// Casts from this collider along <paramref name="offset"/> and returns whether another collider is hit on the combined layer mask.
	/// </summary>
	public static bool IsOverlapping(this Collider2D collider2D,LayerMask layerMask,Vector2 offset,ContactFilter2D contactFilter2D,out RaycastHit2D raycastHit)
	{
		raycastHit = default;

		if(!collider2D)
		{
			return false;
		}

		contactFilter2D.SetLayerMask(layerMask | contactFilter2D.layerMask);

		var distance = offset.magnitude;
		var direction = offset.normalized;
		var buffer = s_cachedRaycast2DArray;

		var count = collider2D.Cast(direction,contactFilter2D,buffer,distance);
		var isHit = count > 0;

		raycastHit = isHit ? buffer[0] : default;

		Array.Clear(buffer,0,count);

		return isHit;
	}
}
