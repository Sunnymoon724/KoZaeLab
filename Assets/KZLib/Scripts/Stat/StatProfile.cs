using System;
using System.Collections.Generic;

namespace KZLib
{
	public record StatEntry<TEnum>(TEnum Type, float Value) where TEnum : struct,Enum;

	public class StatProfile<TEnum> where TEnum : struct,Enum
	{
		private readonly Dictionary<TEnum,float> m_baseDict = new();
		private readonly Dictionary<TEnum,float> m_growthDict = new();

		public StatProfile() { }

		private StatProfile(Dictionary<TEnum,float> baseDict,Dictionary<TEnum,float> growthDict)
		{
			m_baseDict = baseDict;
			m_growthDict = growthDict;
		}

		public void Initialize(StatEntry<TEnum>[] baseStatArray,StatEntry<TEnum>[] growthStatArray)
		{
			_Fill(m_baseDict,baseStatArray);
			_Fill(m_growthDict,growthStatArray);
		}

		public float GetFinalStat(TEnum type,int level)
		{
			m_baseDict.TryGetValue(type,out var baseVal);
			m_growthDict.TryGetValue(type,out var growthVal);

			return baseVal+growthVal*level;
		}

		public static StatProfile<TEnum> operator +(StatProfile<TEnum> lhs,StatProfile<TEnum> rhs)
		{
			return new StatProfile<TEnum>(_Combine(lhs.m_baseDict,rhs.m_baseDict,+1),_Combine(lhs.m_growthDict,rhs.m_growthDict,+1));
		}

		public static StatProfile<TEnum> operator -(StatProfile<TEnum> lhs,StatProfile<TEnum> rhs)
		{
			return new StatProfile<TEnum>(_Combine(lhs.m_baseDict,rhs.m_baseDict,-1),_Combine(lhs.m_growthDict,rhs.m_growthDict,-1));
		}

		private static void _Fill(Dictionary<TEnum,float> statDict,StatEntry<TEnum>[] statArray)
		{
			statDict.Clear();

			if(statArray == null)
			{
				return;
			}

			foreach(var stat in statArray)
			{
				if(!statDict.TryAdd(stat.Type,stat.Value))
				{
					throw new ArgumentException($"Duplicate stat type: {stat.Type}");
				}
			}
		}

		private static Dictionary<TEnum,float> _Combine(Dictionary<TEnum,float> lhs,Dictionary<TEnum,float> rhs,int sign)
		{
			var result = new Dictionary<TEnum,float>(lhs);

			foreach(var pair in rhs)
			{
				result.TryGetValue(pair.Key,out var existing);

				result[pair.Key] = existing+pair.Value*sign;
			}

			return result;
		}
	}
}