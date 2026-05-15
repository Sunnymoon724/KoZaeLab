using System;
using UnityEngine;

public static class Rigidbody2DExtension
{
	private static readonly RaycastHit2D[] s_cachedRaycast2DArray = new RaycastHit2D[1];

	public static bool IsOverlapping(this Rigidbody2D rigidbody2D,Collider2D collider2D)
	{
		return rigidbody2D.Distance(collider2D).isOverlapped;
	}

	public static bool IsTouchingNow(this Rigidbody2D rigidbody2D,Collider2D collider2D)
	{
		var colliderDistance = rigidbody2D.Distance(collider2D);

		if(colliderDistance.isOverlapped)
		{
			return true;
		}

		return colliderDistance.distance.ApproximatelyZero();
	}

	public static bool IsOverlapping(this Rigidbody2D rigidbody2D,LayerMask layerMask,Vector2 offset)
	{
		return IsOverlapping(rigidbody2D,layerMask,offset,default);
	}

	public static bool IsOverlapping(this Rigidbody2D rigidbody2D,LayerMask layerMask,Vector2 offset,ContactFilter2D contactFilter2D)
	{
		return IsOverlapping(rigidbody2D,layerMask,offset,contactFilter2D,out _);
	}

	public static bool IsOverlapping(this Rigidbody2D rigidbody2D,LayerMask layerMask,Vector2 offset,out RaycastHit2D raycastHit)
	{
		return IsOverlapping(rigidbody2D,layerMask,offset,default,out raycastHit);
	}

	public static bool IsOverlapping(this Rigidbody2D rigidbody2D,LayerMask layerMask,Vector2 offset,ContactFilter2D contactFilter2D,out RaycastHit2D raycastHit)
	{
		contactFilter2D.SetLayerMask(layerMask | contactFilter2D.layerMask);

		var distance = offset.magnitude;
		var direction = offset.normalized;
		var buffer = s_cachedRaycast2DArray;

		var count = rigidbody2D.Cast(direction,contactFilter2D,buffer,distance);
		var isHit = count > 0;

		raycastHit = isHit ? buffer[0] : default;

		Array.Clear(buffer,0,count);

		return isHit;
	}

	public static void ChangeDirection(this Rigidbody2D rigidbody2D,Vector2 direction)
	{
		rigidbody2D.linearVelocity = direction*rigidbody2D.linearVelocity.magnitude;
	}
}