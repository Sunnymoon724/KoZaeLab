using System;
using System.Collections.Generic;
using UnityEngine;

using Object = UnityEngine.Object;

public static partial class CommonUtility
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


	public static RaycastHit2D ClosestHit2D(RaycastHit2D[] raycastHit2DArray)
	{
		return ClosestHit2D(raycastHit2DArray,raycastHit2DArray.Length);
	}
	public static RaycastHit2D ClosestHit2D(RaycastHit2D[] raycastHit2DArray,int count)
	{
		static bool _CheckHit(RaycastHit2D _)
		{
			return true;
		}

		return _ClosestHit2D(raycastHit2DArray,count,_CheckHit);
	}
	public static RaycastHit2D ClosestHit2DNonZero(RaycastHit2D[] raycastHit2DArray)
	{
		return ClosestHit2DNonZero(raycastHit2DArray,raycastHit2DArray.Length);
	}
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


	private static TRaycastHit _ClosestHit<TRaycastHit>(TRaycastHit[] raycastHitArray,int count,Func<TRaycastHit,bool> onCheckHit,Func<TRaycastHit,float> onGetDistance)
	{
		var loopCount = Math.Min(count,raycastHitArray.Length);
		var closestDistance = float.MaxValue;
		var closestHit = default(TRaycastHit);

		for(var i=0;i<loopCount;i++)
		{
			var currentHit = raycastHitArray[i];
			var currentDistance = onGetDistance(currentHit);

			if(onCheckHit(currentHit) && currentDistance < closestDistance)
			{
				closestDistance = currentDistance;
				closestHit = currentHit;
			}
		}

		return closestHit;
	}
	#endregion ClosestHit

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

	public static bool CheckGameObject<T>(RaycastHit2D[] _raycastArray,int _length,T _filter,out RaycastHit2D _raycast) where T : Object
	{
		for(var i=_length-1;i>=0;i--)
		{
			if(_filter.Equals(_raycastArray[i].collider.gameObject))
			{
				_raycast = _raycastArray[i];

				return true;
			}
		}

		_raycast = default;

		return false;
	}

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
	public static List<Collider2D> ExtractCollider2D(Collider2D[] colliderArray,Collider2D filterCollider)
	{
		return ExtractCollider2D(colliderArray,colliderArray.Length,filterCollider);
	}

	public static List<Collider2D> ExtractCollider2D(Collider2D[] colliderArray,int count,Collider2D filterCollider)
	{
		static Collider2D _GetCollider(Collider2D collider)
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
	public static List<RaycastHit2D> ExtractCollider2D(RaycastHit2D[] raycastHitArray,Collider2D filterCollider)
	{
		return ExtractCollider2D(raycastHitArray,raycastHitArray.Length,filterCollider);
	}
	public static List<RaycastHit2D> ExtractCollider2D(RaycastHit2D[] raycastHitArray,int count,Collider2D filterCollider)
	{
		static Collider2D _GetCollider(RaycastHit2D raycastHit)
		{
			return raycastHit.collider;
		}

		return _ExtractCollider(raycastHitArray,count,filterCollider,_GetCollider);
	}


	private static List<TValue> _ExtractCollider<TValue,UCollider>(TValue[] valueArray,int count,UCollider filterCollider,Func<TValue,UCollider> onGetCollider)
	{
		var filteredList = new List<TValue>(count);

		for(var i=0;i<count;i++)
		{
			var value = valueArray[i];
			var collider = onGetCollider(value);

			if(collider.Equals(filterCollider))
			{
				filteredList.Add(value);
			}
		}

		return filteredList;
	}
	#endregion ExtractByCollider
}