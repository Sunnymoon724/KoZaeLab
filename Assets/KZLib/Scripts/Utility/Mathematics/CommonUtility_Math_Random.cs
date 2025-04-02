using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public static partial class CommonUtility
{
	private static readonly Random s_random = new();

	#region Integer
	/// <summary>
	/// min <= n <= max
	/// </summary>
	public static int GenerateRandomInt(int minValue,int maxValue)
	{
		(minValue,maxValue) = _NormalizeRange(minValue,maxValue);

		return minValue == maxValue ? minValue : s_random.Next(minValue,maxValue+1);
	}

	public static IEnumerable<int> GenerateRandomIntGroup(int minValue,int maxValue,int count,bool allowDuplicate = true)
	{
		(minValue,maxValue) = _NormalizeRange(minValue,maxValue);

		if(allowDuplicate)
		{
			for(var i=0;i<count;i++)
			{
				yield return GenerateRandomInt(minValue,maxValue);
			}
		}
		else
		{
			var range = maxValue-minValue+1;

			if(count > range)
			{
				LogTag.System.E($"Range({minValue} ~ {maxValue}) is smaller than count ({count}).");

				yield break;
			}

			var uniqueHashSet = new HashSet<int>();

			while(uniqueHashSet.Count < count)
			{
				var value = GenerateRandomInt(minValue,maxValue);

				if(uniqueHashSet.Add(value))
				{
					yield return value;
				}
			}
		}
	}

	public static int GenerateWeightedRandomInt(float[] weightedArray)
	{
		if(weightedArray.IsNullOrEmpty())
		{
			LogTag.System.E("Weighted is null or empty");

			return Global.INVALID_INDEX;
		}

		if(weightedArray.Length == 1)
		{
			return 0;
		}

		var total = 0.0f;

		foreach(var weight in weightedArray)
		{
			if(weight < 0.0f)
			{
				LogTag.System.E($"Weight is below zero -> {weight} < 0.0f");

				return Global.INVALID_INDEX;
			}

			total += weight;
		}

		var pivot = GenerateRandomFloat(0.0f,total);

		for(var i=0;i<weightedArray.Length;i++)
		{
			if(pivot <= weightedArray[i])
			{
				return i;
			}

			pivot -= weightedArray[i];
		}

		return Global.INVALID_INDEX;
	}
	#endregion Integer

	#region Single
	/// <summary>
	/// 0<= n < 1
	/// </summary>
	public static float GenerateRandomFloat()
	{
		return (float) s_random.NextDouble();
	}

	/// <summary>
	/// -value <= n <= +value
	/// </summary>
	public static float GenerateRandomFloat(float value)
	{
		return value == 0.0f ? value : GenerateRandomFloat(-value,+value);
	}

	/// <summary>
	/// min <= n <= max
	/// </summary>
	public static float GenerateRandomFloat(float minValue,float maxValue)
	{
		(minValue,maxValue) = _NormalizeRange(minValue,maxValue);

		return GenerateRandomFloat()*(maxValue-minValue)+minValue;
	}

	public static IEnumerable<float> GenerateRandomFloatGroup(float minValue,float maxValue,int count,bool allowDuplicate = true)
	{
		(minValue,maxValue) = _NormalizeRange(minValue,maxValue);

		if(allowDuplicate)
		{
			for(var i=0;i<count;i++)
			{
				yield return GenerateRandomFloat(minValue,maxValue);
			}
		}
		else
		{
			var range = maxValue-minValue+1;

			if(count > range)
			{
				LogTag.System.E($"Range({minValue} ~ {maxValue}) is smaller than count ({count}).");

				yield break;
			}

			var uniqueHashSet = new HashSet<float>();

			while(uniqueHashSet.Count < count)
			{
				var value = GenerateRandomFloat(minValue,maxValue);

				if(uniqueHashSet.Add(value))
				{
					yield return value;
				}
			}
		}
	}
	#endregion Single

	#region Double
	/// <summary>
	/// 0<= n < 1
	/// </summary>
	public static double GenerateRandomDouble()
	{
		return s_random.NextDouble();
	}

	/// <summary>
	/// -value <= n <= +value
	/// </summary>
	public static double GenerateRandomDouble(double value)
	{
		return value == 0.0d ? value : GenerateRandomDouble(-value,+value);
	}

	/// <summary>
	/// min <= n <= max
	/// </summary>
	public static double GenerateRandomDouble(double minValue,double maxValue)
	{
		return GenerateRandomDouble()*(maxValue-minValue)+minValue;
	}

	public static IEnumerable<double> GenerateRandomDoubleGroup(double minValue,double maxValue,int count,bool allowDuplicate = true)
	{
		(minValue,maxValue) = _NormalizeRange(minValue,maxValue);

		if(allowDuplicate)
		{
			for(var i=0;i<count;i++)
			{
				yield return GenerateRandomDouble(minValue,maxValue);
			}
		}
		else
		{
			var range = maxValue-minValue+1;

			if(count > range)
			{
				LogTag.System.E($"Range({minValue} ~ {maxValue}) is smaller than count ({count}).");

				yield break;
			}

			var uniqueHashSet = new HashSet<double>();

			while(uniqueHashSet.Count < count)
			{
				var value = GenerateRandomDouble(minValue,maxValue);

				if(uniqueHashSet.Add(value))
				{
					yield return value;
				}
			}
		}
	}
	#endregion Double

	#region Boolean
	/// <summary>
	/// true or false
	/// </summary>
	public static bool GenerateRandomBool()
	{
		return GenerateRandomInt(0,2) == 0;
	}
	#endregion Boolean

	#region String
	public static string GenerateRandomString(int length,bool allowDuplicate = true)
	{
		var text = "abcdefghijklmnopqrstuvwxyz";
		var maxLength = Mathf.Max(text.Length,length);
		var characterList = new List<char>(text.GenerateRandomValueGroup(maxLength,allowDuplicate));

		characterList.Randomize();

		return string.Concat(characterList);
	}
	#endregion String

	#region Gaussian
	public static int GenerateGaussian(int mean,int deviation)
	{
		return mean+(int) GenerateGaussian()*deviation;
	}

	public static float GenerateGaussian(float mean,float deviation)
	{
		return mean+GenerateGaussian()*deviation;
	}

	public static float GenerateGaussian()
	{
		float radiusSquared, randomX, randomY;

		do
		{
			randomX = GenerateRandomFloat(-1.0f, +1.0f);
			randomY = GenerateRandomFloat(-1.0f, +1.0f);

			radiusSquared = randomX * randomX + randomY * randomY;
		}
		while (radiusSquared >= 1.0f || radiusSquared.ApproximatelyZero());

		return randomX*Mathf.Sqrt(-2.0f*Mathf.Log(radiusSquared)/radiusSquared);
	}
	#endregion Gaussian

	/// <summary>
	/// -1, 0, 1
	/// </summary>
	public static int GetRandomSign(bool includeZero = true)
	{
		return includeZero ? GenerateRandomInt(0,2)-1 : GenerateRandomDouble() < 0.5d ? -1 : 1;
	}

	/// <summary>
	/// Check value in [0,1].
	/// </summary>
	public static bool IsInProbability(float percent)
	{
		return GenerateRandomFloat() <= percent;
	}

	/// <summary>
	/// Check value in [minValue,maxValue].
	/// </summary>
	public static bool IsProbabilityInRange(float minValue,float maxValue)
	{
		var value = GenerateRandomFloat();

		(minValue,maxValue) = _NormalizeRange(minValue,maxValue);

		return minValue <= value && value <= maxValue;
	}

	private static (TValue min,TValue max) _NormalizeRange<TValue>(TValue minValue,TValue maxValue) where TValue : IComparable<TValue>
	{
		return minValue.CompareTo(maxValue) > 0 ? (maxValue,minValue) : (minValue,maxValue);
	}
}