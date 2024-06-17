using UnityEngine;
using System.Collections.Generic;

public static partial class CommonUtility
{
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
}