using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public static partial class CommonUtility
{
	private static readonly Random s_Random = new();

	#region Integer
	/// <summary>
	/// min 에서 max 사이의 숫자를 무작위로 반환한다.
	/// </summary>
	public static int GetRndInt(int _min,int _max)
	{
		return s_Random.Next(_min,_max+1);
	}

	public static int GetWeightedRndInt(float[] _weightedArray)
	{
		if(_weightedArray.IsNullOrEmpty())
		{
			throw new ArgumentException("가중치가 없습니다.");
		}

		if(_weightedArray.Length == 1)
		{
			return 0;
		}

		var total = 0.0f;

		foreach(var weight in _weightedArray)
		{
			if(weight < 0.0f)
			{
				throw new ArgumentException("가중치가 음수입니다.");
			}

			total += weight;
		}

		var pivot = GetRndFloat(0.0f,total);

		for(var i=0;i<_weightedArray.Length;i++)
		{
			if(pivot <= _weightedArray[i])
			{
				return i;
			}

			pivot -= _weightedArray[i];
		}

		return 0;
	}
	#endregion Integer

	#region Single
	/// <summary>
	/// (0<= n < 1) 무작위 숫자를 반환한다.
	/// </summary>
	public static float GetRndFloat()
	{
		return (float) s_Random.NextDouble();
	}

	/// <summary>
	/// -value 에서 +value 사이의 숫자를 무작위로 반환한다.
	/// </summary>
	public static float GetRndFloat(float _value,int? _decimals = null)
	{
		return _value == 0.0f ? _value : GetRndFloat(-_value,+_value,_decimals);
	}

	/// <summary>
	/// min 에서 max 사이의 숫자를 무작위로 반환한다.
	/// dot 는 소수점 N째 자리를 의미한다.
	/// </summary>
	public static float GetRndFloat(float _min,float _max,int? _decimals = null)
	{
		var value = GetRndFloat()*(_max-_min)+_min;

		return _decimals == null ? value : value.ToLimit(_decimals.Value);
	}
	#endregion Single

	#region Double
	/// <summary>
	/// (0<= n < 1) 무작위 숫자를 반환한다.
	/// </summary>
	public static double GetRndDouble()
	{
		return s_Random.NextDouble();
	}

	/// <summary>
	/// -value 에서 +value 사이의 숫자를 무작위로 반환한다.
	/// </summary>
	public static double GetRndDouble(double _value,int? _decimals = null)
	{
		return _value == 0.0d ? _value : GetRndDouble(-_value,+_value,_decimals);
	}

	/// <summary>
	/// min 에서 max 사이의 숫자를 무작위로 반환한다.
	/// dot 는 소수점 N째 자리를 의미한다.
	/// </summary>
	public static double GetRndDouble(double _min,double _max,int? _decimals = null)
	{
		var value = GetRndDouble()*(_max-_min)+_min;

		return _decimals == null ? value : value.ToLimit(_decimals.Value);
	}
	#endregion Double

	#region Boolean
	/// <summary>
	/// true or false 반환한다.
	/// </summary>
	public static bool GetRndBool()
	{
		return GetRndInt(0,2) == 0;
	}
	#endregion Boolean

	#region String
	public static string GetRndString(int _length,bool _overlap = true)
	{
		var text = "abcdefghijklmnopqrstuvwxyz";
		var charList = new List<char>(_length);

		if(_overlap)
		{
			for(var i=0;i<_length;i++)
			{
				charList.Add(text.GetRndValue());
			}
		}
		else
		{
			var length = Mathf.Max(text.Length,_length);

			charList.AddRange(text);
			charList = charList.GetRndValueList(length);
		}

		charList.Randomize();

		return string.Concat(charList);
	}
	#endregion String

	#region Gaussian
	/// <summary>
	/// 정규분포에서 무작위 정수 반환
	/// </summary>
	public static int GetGaussian(int _mean,int _deviation)
	{
		return _mean+(int) GetGaussian()*_deviation;
	}

	/// <summary>
	/// 정규분포에서 무작위 정수 반환
	/// </summary>
	public static int GetGaussian(int _mean,int _deviation,int _min,int _max)
	{
		int pivot;

		do
		{
			pivot = GetGaussian(_mean,_deviation);
		}while(pivot < _min || pivot > _max);

		return pivot;
	}

	/// <summary>
	/// 정규분포에서 무작위 실수 반환
	/// </summary>
	public static float GetGaussian(float _mean,float _deviation)
	{
		return _mean+GetGaussian()*_deviation;
	}

	/// <summary>
	/// 정규분포에서 무작위 실수 반환
	/// </summary>
	public static float GetGaussian(float _mean,float _deviation,float _min,float _max)
	{
		float pivot;

		do
		{
			pivot = GetGaussian(_mean,_deviation);
		}while(pivot < _min || pivot > _max);

		return pivot;
	}

	/// <summary>
	/// 정규분포에서 무작위 실수 반환
	/// </summary>
	public static float GetGaussian()
	{
		float pivot,value1,value2;

		do
		{
			value1 = 2.0f*GetRndFloat(0.0f,1.0f)-1.0f;
			value2 = 2.0f*GetRndFloat(0.0f,1.0f)-1.0f;

			pivot = value1*value1+value2*value2;
		}while(pivot >= 1.0f || pivot.Approximately(0.0f));

		return value1*Mathf.Sqrt(-2.0f*Mathf.Log(pivot)/pivot);
	}
	#endregion Gaussian

	/// <summary>
	/// -1, 0, 1 반환
	/// </summary>
	public static int GetRndSign(bool _includeZero = true)
	{
		return _includeZero ? GetRndInt(0,2)-1 : GetRndDouble() < 0.5d ? -1 : 1;
	}

	/// <summary>
	/// 0에서 1의 실수를 반환해서 percent와 비교하여 판단한다.
	/// </summary>
	public static bool CheckProbability(float _percent)
	{
		return _percent > 0.0f || GetRndFloat() <= _percent;
	}

	/// <summary>
	/// _low와 _high 사이의 실수를 반환해서 percent와 비교하여 판단한다.
	/// </summary>
	public static bool CheckProbabilityInRange(float _low,float _high)
	{
		var value = GetRndFloat();

		if(_low == _high)
		{
			// 확률 범위가 같은 경우
			return _low > 0.0f || value <= _low;
		}
		else if (_low > _high)
		{
			// 확률 범위가 역전된 경우
			return _high > 0.0f || (_high <= value && value <= _low);
		}
		else
		{
			// 정상적인 확률 범위인 경우
			return _low > 0.0f || (_low <= value && value <= _high);
		}
	}
}