using System;
using System.Collections.Generic;
using UnityEngine;

using Object = UnityEngine.Object;

/// <summary>
/// Utility methods for filtering and selecting 2D physics raycast and overlap results.
/// </summary>
public static partial class KZPhysicsKit
{
	#region ClosestHit
	/// <summary>
	/// Returns the closest RaycastHit2D from the full array, including zero-distance hits.
	/// </summary>
	public static RaycastHit2D ClosestHit2D(RaycastHit2D[] raycastHit2DArray)
	{
		if(raycastHit2DArray == null)
		{
			return default;
		}

		return ClosestHit2D(raycastHit2DArray,raycastHit2DArray.Length);
	}

	/// <summary>
	/// Returns the closest RaycastHit2D from the first count entries, including zero-distance hits.
	/// </summary>
	public static RaycastHit2D ClosestHit2D(RaycastHit2D[] raycastHit2DArray,int count)
	{
		static bool _CheckHit(RaycastHit2D _)
		{
			return true;
		}

		return _ClosestHit2D(raycastHit2DArray,count,_CheckHit);
	}

	/// <summary>
	/// Returns the closest RaycastHit2D from the full array, excluding zero-distance hits.
	/// </summary>
	public static RaycastHit2D ClosestHit2DNonZero(RaycastHit2D[] raycastHit2DArray)
	{
		if(raycastHit2DArray == null)
		{
			return default;
		}

		return ClosestHit2DNonZero(raycastHit2DArray,raycastHit2DArray.Length);
	}

	/// <summary>
	/// Returns the closest RaycastHit2D from the first count entries, excluding zero-distance hits.
	/// </summary>
	public static RaycastHit2D ClosestHit2DNonZero(RaycastHit2D[] raycastHit2DArray,int count)
	{
		static bool _CheckHit(RaycastHit2D raycastHit)
		{
			return raycastHit.distance > 0;
		}

		return _ClosestHit2D(raycastHit2DArray,count,_CheckHit);
	}

	private static RaycastHit2D _ClosestHit2D(RaycastHit2D[] raycastHitArray,int count,Func<RaycastHit2D,bool> onCheckHit)
	{
		static float _GetDistance(RaycastHit2D raycastHit)
		{
			return raycastHit.distance;
		}

		return _ClosestHit(raycastHitArray,count,onCheckHit,_GetDistance);
	}
	#endregion ClosestHit

	/// <summary>
	/// Searches raycast results in reverse order for a hit whose collider matches the filter.
	/// </summary>
	public static bool TryFindCollider2D<T>(RaycastHit2D[] raycastHitArray,int count,T filter,out RaycastHit2D raycastHit2D) where T : Collider2D
	{		
		for(var i=count-1;i>=0;i--)
		{
			if(filter.Equals(raycastHitArray[i].collider))
			{
				raycastHit2D = raycastHitArray[i];

				return true;
			}
		}

		raycastHit2D = default;

		return false;
	}

	/// <summary>
	/// Searches raycast results in reverse order for a hit whose collider GameObject matches the filter.
	/// </summary>
	public static bool CheckGameObject<T>(RaycastHit2D[] _raycastArray,int _length,T _filter,out RaycastHit2D _raycast) where T : Object
	{
		for(var i=_length-1;i>=0;i--)
		{
			var collider = _raycastArray[i].collider;

			if(collider && _filter.Equals(collider.gameObject))
			{
				_raycast = _raycastArray[i];

				return true;
			}
		}

		_raycast = default;

		return false;
	}

	#region ExtractByCollider
	/// <summary>
	/// Returns all Collider2D entries from the array that match the filter collider.
	/// </summary>
	public static List<Collider2D> ExtractCollider2D(Collider2D[] colliderArray,Collider2D filterCollider)
	{
		if(colliderArray == null)
		{
			return new List<Collider2D>();
		}

		return ExtractCollider2D(colliderArray,colliderArray.Length,filterCollider);
	}

	/// <summary>
	/// Returns all Collider2D entries from the first count entries that match the filter collider.
	/// </summary>
	public static List<Collider2D> ExtractCollider2D(Collider2D[] colliderArray,int count,Collider2D filterCollider)
	{
		static Collider2D _GetCollider(Collider2D collider)
		{
			return collider;
		}

		return _ExtractCollider(colliderArray,count,filterCollider,_GetCollider);
	}

	/// <summary>
	/// Returns all RaycastHit2D entries from the array whose collider matches the filter.
	/// </summary>
	public static List<RaycastHit2D> ExtractCollider2D(RaycastHit2D[] raycastHitArray,Collider2D filterCollider)
	{
		if(raycastHitArray == null)
		{
			return new List<RaycastHit2D>();
		}

		return ExtractCollider2D(raycastHitArray,raycastHitArray.Length,filterCollider);
	}

	/// <summary>
	/// Returns all RaycastHit2D entries from the first count entries whose collider matches the filter.
	/// </summary>
	public static List<RaycastHit2D> ExtractCollider2D(RaycastHit2D[] raycastHitArray,int count,Collider2D filterCollider)
	{
		static Collider2D _GetCollider(RaycastHit2D raycastHit)
		{
			return raycastHit.collider;
		}

		return _ExtractCollider(raycastHitArray,count,filterCollider,_GetCollider);
	}
	#endregion ExtractByCollider
}
