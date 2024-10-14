using System;
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

	#region Clamp
	public static int LoopClamp(int _index,int _size)
	{
		return _size < 1 ? 0 : _index < 0 ? _size-1+(_index+1)%_size : _index%_size;
	}

	public static float LoopClamp(float _index,int _size)
	{
		return _size < 1 ? 0 : _index < 0.0f ? _size-1+(_index+1)%_size : _index%_size;
	}

	public static TCompare Clamp<TCompare>(TCompare _curValue,TCompare _minValue,TCompare _maxValue) where TCompare : IComparable<TCompare>
	{
		return _curValue.CompareTo(_minValue) < 0 ? _minValue : _curValue.CompareTo(_maxValue) > 0 ? _maxValue : _curValue;
	}

	public static TCompare MinClamp<TCompare>(TCompare _curValue,TCompare _minValue) where TCompare : IComparable<TCompare>
	{
		return Clamp(_curValue,_minValue,_curValue);
	}

	public static TCompare MaxClamp<TCompare>(TCompare _curValue,TCompare _maxValue) where TCompare : IComparable<TCompare>
	{
		return Clamp(_curValue,_curValue,_maxValue);
	}
	#endregion Clamp

	#region Slope
	public static float SlopeToRadian(float _x,float _y)
	{
		return SlopeToRadian(new Vector2(_x,_y));
	}

	public static float SlopeToRadian(Vector2 _gradient)
	{
		return Mathf.Atan2(_gradient.y,_gradient.x);
	}

	public static Vector2 RadianToSlope(float _angle)
	{
		return new Vector2(Mathf.Cos(_angle),Mathf.Sin(_angle));
	}

	public static float SlopeToDegree(float _x,float _y)
	{
		return SlopeToDegree(new Vector2(_x,_y));
	}

	public static float SlopeToDegree(Vector2 _gradient)
	{
		return SlopeToRadian(_gradient)*Mathf.Rad2Deg;
	}

	public static Vector2 DegreeToSlope(float _angle)
	{
		return new Vector2(Mathf.Cos(_angle*Mathf.Deg2Rad),Mathf.Sin(_angle*Mathf.Deg2Rad));
	}
	#endregion Slope

	#region Alignment
	/// <summary>
	/// Set alignment ( 0 / -0.5 +0.5 / -1 0 +1 / -1.5 -0.5 +0.5 +1.5)
	/// </summary>
	public static float[] MiddleAlignment(int _length)
	{
		var pivotArray = new float[_length];
		var pivot = pivotArray.Length/2.0f;

		//? odd
		if(_length%2 == 1)
		{
			for(var i=0;i<(int)pivot;i++)
			{
				pivotArray[(int) Mathf.Floor(pivot)-(i+1)]=-(i+1);
				pivotArray[(int) Mathf.Ceil(pivot)+(i+0)]=+(i+1);
			}
		}
		//? even
		else
		{
			var divide = 0.5f;

			for(var i=0;i<(int) pivot;i++)
			{
				pivotArray[(int) Mathf.Floor(pivot)-(i+1)]=-(2*i+1)*divide;
				pivotArray[(int) Mathf.Ceil(pivot)+(i+0)]=+(2*i+1)*divide;
			}
		}

		return pivotArray;
	}

	public static void SetAlignmentGameObjectList(List<GameObject> _objectList,int _xMax,int _hMax,float _xGap,float _yGap)
	{
		var widthAble   = _xMax != -1;
		var heightAble  = _xMax != -1;
		var widthArray  = widthAble ? MiddleAlignment(_xMax) : null;
		var heightArray = heightAble ? MiddleAlignment(Mathf.CeilToInt(_objectList.Count/(float) _hMax)) : null;

		for(var i=0;i<_objectList.Count;i++)
		{
			var width   = widthAble     ? widthArray[i%_xMax]*_xGap     : _objectList[i].transform.position.x;
			var height  = heightAble    ? heightArray[i/_hMax]*_yGap    : _objectList[i].transform.position.z;

			_objectList[i].transform.position = new Vector3(width,0.0f,height);
		}
	}
	#endregion Alignment
}