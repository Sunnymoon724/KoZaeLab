using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static partial class MathUtility
{
	#region Distance
	public static float GetTotalDistance(IEnumerable<Vector3> _positionGroup)
	{
		var distance = 0.0f;
		var positionArray = _positionGroup.ToArray();

		for(var i=1;i<positionArray.Length;i++)
		{
			distance += Vector3.Distance(positionArray[i-1],positionArray[i]);
		}

		return distance;
	}
	#endregion Distance
}