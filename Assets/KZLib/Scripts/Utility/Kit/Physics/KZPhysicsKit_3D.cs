using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Utility methods for filtering and selecting 3D physics raycast and overlap results.
/// </summary>
public static partial class KZPhysicsKit
{
	#region ClosestHit
	/// <summary>
	/// Returns the closest RaycastHit from the full array, including zero-distance hits.
	/// </summary>
	public static RaycastHit ClosestHit3D(RaycastHit[] raycastHitArray)
	{
		if(raycastHitArray == null)
		{
			return default;
		}

		return ClosestHit3D(raycastHitArray,raycastHitArray.Length);
	}

	/// <summary>
	/// Returns the closest RaycastHit from the first count entries, including zero-distance hits.
	/// </summary>
	public static RaycastHit ClosestHit3D(RaycastHit[] raycastHitArray,int count)
	{
		static bool _CheckHit(RaycastHit _)
		{
			return true;
		}

		return _ClosestHit3D(raycastHitArray,count,_CheckHit);
	}

	/// <summary>
	/// Returns the closest RaycastHit from the full array, excluding zero-distance hits.
	/// </summary>
	public static RaycastHit ClosestHit3DNonZero(RaycastHit[] raycastHitArray)
	{
		if(raycastHitArray == null)
		{
			return default;
		}

		return ClosestHit3DNonZero(raycastHitArray,raycastHitArray.Length);
	}

	/// <summary>
	/// Returns the closest RaycastHit from the first count entries, excluding zero-distance hits.
	/// </summary>
	public static RaycastHit ClosestHit3DNonZero(RaycastHit[] raycastHitArray,int count)
	{
		static bool _CheckHit(RaycastHit raycastHit)
		{
			return raycastHit.distance > 0;
		}

		return _ClosestHit3D(raycastHitArray,count,_CheckHit);
	}

	private static RaycastHit _ClosestHit3D(RaycastHit[] raycastHitArray,int count,Func<RaycastHit,bool> onCheckHit)
	{
		static float _GetDistance(RaycastHit raycastHit)
		{
			return raycastHit.distance;
		}

		return _ClosestHit(raycastHitArray,count,onCheckHit,_GetDistance);
	}
	#endregion ClosestHit

	#region ExtractByCollider
	/// <summary>
	/// Returns all Collider entries from the array that match the filter collider.
	/// </summary>
	public static List<Collider> ExtractCollider3D(Collider[] colliderArray,Collider filterCollider)
	{
		if(colliderArray == null)
		{
			return new List<Collider>();
		}

		return ExtractCollider3D(colliderArray,colliderArray.Length,filterCollider);
	}

	/// <summary>
	/// Returns all Collider entries from the first count entries that match the filter collider.
	/// </summary>
	public static List<Collider> ExtractCollider3D(Collider[] colliderArray,int count,Collider filterCollider)
	{
		static Collider _GetCollider(Collider collider)
		{
			return collider;
		}

		return _ExtractCollider(colliderArray,count,filterCollider,_GetCollider);
	}

	/// <summary>
	/// Returns all RaycastHit entries from the array whose collider matches the filter.
	/// </summary>
	public static List<RaycastHit> ExtractCollider3D(RaycastHit[] raycastHitArray,Collider filterCollider)
	{
		if(raycastHitArray == null)
		{
			return new List<RaycastHit>();
		}

		return ExtractCollider3D(raycastHitArray,raycastHitArray.Length,filterCollider);
	}

	/// <summary>
	/// Returns all RaycastHit entries from the first count entries whose collider matches the filter.
	/// </summary>
	public static List<RaycastHit> ExtractCollider3D(RaycastHit[] raycastHitArray,int count,Collider filterCollider)
	{
		static Collider _GetCollider(RaycastHit raycastHit)
		{
			return raycastHit.collider;
		}

		return _ExtractCollider(raycastHitArray,count,filterCollider,_GetCollider);
	}
	#endregion ExtractByCollider
}
