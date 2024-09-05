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
	/// <summary>
	/// <para> 만약 음수가 나오면 순환이므로 최대값을 반환한다.</para>
	/// </summary>
	public static int LoopClamp(int _index,int _size)
	{
		return _size < 1 ? 0 : _index < 0 ? _size-1+(_index+1)%_size : _index%_size;
	}

	/// <summary>
	/// <para> 만약 음수가 나오면 순환이므로 최대값을 반환한다.</para>
	/// </summary>
	public static float LoopClamp(float _index,int _size)
	{
		return _size < 1 ? 0 : _index < 0.0f ? _size-1+(_index+1)%_size : _index%_size;
	}

	/// <summary>
	/// 비교 가능한 오브젝트를 범위 안의 오브젝트로 자른다.
	/// </summary>
	public static TCompare Clamp<TCompare>(TCompare _value,TCompare _min,TCompare _max) where TCompare : IComparable<TCompare>
	{
		return _value.CompareTo(_min) < 0 ? _min : _value.CompareTo(_max) > 0 ? _max : _value;
	}

	/// <summary>
	/// 비교 가능한 오브젝트를 범위 안의 오브젝트로 자른다.
	/// </summary>
	public static TCompare MinClamp<TCompare>(TCompare _value,TCompare _min) where TCompare : IComparable<TCompare>
	{
		return _value.CompareTo(_min) < 0 ? _min : _value;
	}

	/// <summary>
	/// 비교 가능한 오브젝트를 범위 안의 오브젝트로 자른다.
	/// </summary>
	public static TCompare MaxClamp<TCompare>(TCompare _value,TCompare _max) where TCompare : IComparable<TCompare>
	{
		return _value.CompareTo(_max) > 0 ? _max : _value;
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
	/// 정해진 크기만큼 중앙배열 하는 숫자를 반환한다.
	/// <para> 예시) 0 / -0.5 +0.5 / -1 0 +1 / -1.5 -0.5 +0.5 +1.5 ...</para>
	/// </summary>
	public static float[] MiddleAlignment(int _length)
	{
		var pivotArray = new float[_length];
		var pivot = pivotArray.Length/2.0f;

		// 홀수 일 때
		if(_length%2 == 1)
		{
			for(var i=0;i<(int)pivot;i++)
			{
				pivotArray[(int) Mathf.Floor(pivot)-(i+1)]=-(i+1);
				pivotArray[(int) Mathf.Ceil(pivot)+(i+0)]=+(i+1);
			}
		}
		// 짝수 일 때
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

	/// <summary>
	/// 게임 오브젝트 정렬
	/// </summary>
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