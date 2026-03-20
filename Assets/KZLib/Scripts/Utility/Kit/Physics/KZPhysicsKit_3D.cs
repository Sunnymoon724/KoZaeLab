using System;
using System.Collections.Generic;
using UnityEngine;

public static partial class KZPhysicsKit
{
	#region ClosestHit
	public static RaycastHit ClosestHit3D(RaycastHit[] raycastHitArray)
	{
		return ClosestHit3D(raycastHitArray,raycastHitArray.Length);
	}

	public static RaycastHit ClosestHit3D(RaycastHit[] raycastHitArray,int count)
	{
		static bool _CheckHit(RaycastHit _)
		{
			return true;
		}

		return _ClosestHit3D(raycastHitArray,count,_CheckHit);
	}

	public static RaycastHit ClosestHit3DNonZero(RaycastHit[] raycastHitArray)
	{
		return ClosestHit3DNonZero(raycastHitArray,raycastHitArray.Length);
	}

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
	public static List<Collider> ExtractCollider3D(Collider[] colliderArray,Collider filterCollider)
	{
		return ExtractCollider3D(colliderArray,colliderArray.Length,filterCollider);
	}

	public static List<Collider> ExtractCollider3D(Collider[] colliderArray,int count,Collider filterCollider)
	{
		static Collider _GetCollider(Collider collider)
		{
			return collider;
		}

		return _ExtractCollider(colliderArray,count,filterCollider,_GetCollider);
	}

	public static List<RaycastHit> ExtractCollider3D(RaycastHit[] raycastHitArray,Collider filterCollider)
	{
		return ExtractCollider3D(raycastHitArray,raycastHitArray.Length,filterCollider);
	}

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