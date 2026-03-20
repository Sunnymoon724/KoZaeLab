using System;
using System.Collections.Generic;

public static partial class KZPhysicsKit
{
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
}