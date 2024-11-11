using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public static partial class CommonUtility
{
	private static readonly Random s_Random = new();

	#region Integer
	/// <summary>
	/// min <= n <= max
	/// </summary>
	public static int GetRndInt(int _min,int _max)
	{
		return _min == _max ? _min : s_Random.Next(_min,_max+1);
	}

	public static IEnumerable<int> GetRndIntGroup(int _min,int _max,int _count)
	{
		for(var i=0;i<_count;i++)
		{
			yield return GetRndInt(_min,_max);
		}
	}

	public static int GetWeightedRndInt(float[] _weightedArray)
	{
		if(_weightedArray.IsNullOrEmpty())
		{
			LogTag.System.E("Weighted is null or empty");

			return Global.INVALID_INDEX;
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
				LogTag.System.E($"Weight is below zero -> {weight} < 0.0f");

				return Global.INVALID_INDEX;
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

		return Global.INVALID_INDEX;
	}
	#endregion Integer

	#region Single
	/// <summary>
	/// 0<= n < 1
	/// </summary>
	public static float GetRndFloat()
	{
		return (float) s_Random.NextDouble();
	}

	/// <summary>
	/// -value <= n <= +value
	/// </summary>
	public static float GetRndFloat(float _value,int? _decimals = null)
	{
		return _value == 0.0f ? _value : GetRndFloat(-_value,+_value,_decimals);
	}

	/// <summary>
	/// min <= n <= max
	/// </summary>
	public static float GetRndFloat(float _min,float _max,int? _decimals = null)
	{
		var value = GetRndFloat()*(_max-_min)+_min;

		return _decimals == null ? value : value.ToLimit(_decimals.Value);
	}

	public static IEnumerable<float> GetRndFloatGroup(float _min,float _max,int _count)
	{
		for(var i=0;i<_count;i++)
		{
			yield return GetRndFloat(_min,_max);
		}
	}
	#endregion Single

	#region Double
	/// <summary>
	/// 0<= n < 1
	/// </summary>
	public static double GetRndDouble()
	{
		return s_Random.NextDouble();
	}

	/// <summary>
	/// -value <= n <= +value
	/// </summary>
	public static double GetRndDouble(double _value,int? _decimals = null)
	{
		return _value == 0.0d ? _value : GetRndDouble(-_value,+_value,_decimals);
	}

	/// <summary>
	/// min <= n <= max
	/// </summary>
	public static double GetRndDouble(double _min,double _max,int? _decimals = null)
	{
		var value = GetRndDouble()*(_max-_min)+_min;

		return _decimals == null ? value : value.ToLimit(_decimals.Value);
	}

	public static IEnumerable<double> GetRndDoubleGroup(double _min,double _max,int _count)
	{
		for(var i=0;i<_count;i++)
		{
			yield return GetRndDouble(_min,_max);
		}
	}
	#endregion Double

	#region Boolean
	/// <summary>
	/// true or false
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
	public static int GetGaussian(int _mean,int _deviation)
	{
		return _mean+(int) GetGaussian()*_deviation;
	}

	public static int GetGaussian(int _mean,int _deviation,int _min,int _max)
	{
		int pivot;

		do
		{
			pivot = GetGaussian(_mean,_deviation);
		}while(pivot < _min || pivot > _max);

		return pivot;
	}

	public static float GetGaussian(float _mean,float _deviation)
	{
		return _mean+GetGaussian()*_deviation;
	}

	public static float GetGaussian(float _mean,float _deviation,float _min,float _max)
	{
		float pivot;

		do
		{
			pivot = GetGaussian(_mean,_deviation);
		}while(pivot < _min || pivot > _max);

		return pivot;
	}

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
	/// -1, 0, 1
	/// </summary>
	public static int GetRndSign(bool _includeZero = true)
	{
		return _includeZero ? GetRndInt(0,2)-1 : GetRndDouble() < 0.5d ? -1 : 1;
	}

	/// <summary>
	/// Check value in [0,1].
	/// </summary>
	public static bool CheckProbability(float _percent)
	{
		return _percent > 0.0f || GetRndFloat() <= _percent;
	}

	/// <summary>
	/// Check value in [low,high].
	/// </summary>
	public static bool CheckProbabilityInRange(float _low,float _high)
	{
		var value = GetRndFloat();

		if(_low == _high)
		{
			//! low == high
			return _low > 0.0f || value <= _low;
		}
		else if (_low > _high)
		{
			//! low > high
			return _high > 0.0f || (_high <= value && value <= _low);
		}
		else
		{
			//! low < high
			return _low > 0.0f || (_low <= value && value <= _high);
		}
	}
}