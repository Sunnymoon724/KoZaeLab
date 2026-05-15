using System;
using UnityEngine;

public static class Collider2DExtension
{
	private static readonly RaycastHit2D[] s_cachedRaycast2DArray = new RaycastHit2D[1];

	public static bool IsOverlapping(this Collider2D collider2D,Collider2D target)
	{
		return collider2D.Distance(target).isOverlapped;
	}

	public static bool IsTouchingNow(this Collider2D collider2D,Collider2D target)
	{
		var colliderDistance = collider2D.Distance(collider2D);

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

	public static bool IsOverlapping(this Collider2D collider2D,LayerMask layerMask,Vector2 offset,ContactFilter2D contactFilter2D,out RaycastHit2D raycastHit)
	{
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