using System;
using System.Collections.Generic;

namespace KZLib
{
	public record StatEntry<TEnum>(TEnum Type,float Value) where TEnum : struct,Enum;

	public class StatProfile<TEnum> where TEnum : struct,Enum
	{
		private readonly Dictionary<TEnum,float> m_baseStatDict = null;
		private readonly Dictionary<TEnum,float> m_growthStatDict = null;

		public StatProfile(StatEntry<TEnum>[] baseStatArray,StatEntry<TEnum>[] growthStatArray)
		{
			m_baseStatDict = new Dictionary<TEnum,float>();
			m_growthStatDict = new Dictionary<TEnum,float>();

			_Initialize(m_baseStatDict,baseStatArray);
			_Initialize(m_growthStatDict,growthStatArray);
		}

		protected StatProfile(Dictionary<TEnum,float> baseStatDict,Dictionary<TEnum,float> growthStatDict)
		{
			m_baseStatDict = baseStatDict;
			m_growthStatDict = growthStatDict;
		}

		protected void _Initialize(Dictionary<TEnum,float> statDict,StatEntry<TEnum>[] statEntryArray)
		{
			statDict.Clear();

			if(statEntryArray == null)
			{
				return;
			}

			foreach(var stat in statEntryArray)
			{
				if(!statDict.TryAdd(stat.Type,stat.Value))
				{
					throw new ArgumentException($"Duplicate stat type: {stat.Type}");
				}
			}
		}

		public float GetFinalStat(TEnum type,int level)
		{
			m_baseStatDict.TryGetValue(type,out var baseVal);
			m_growthStatDict.TryGetValue(type,out var growthVal);

			return baseVal+growthVal*level;
		}

		public static StatProfile<TEnum> operator +(StatProfile<TEnum> lhs,StatProfile<TEnum> rhs)
		{
			return new StatProfile<TEnum>(_Combine(lhs.m_baseStatDict,rhs.m_baseStatDict,+1),_Combine(lhs.m_growthStatDict,rhs.m_growthStatDict,+1));
		}

		public static StatProfile<TEnum> operator -(StatProfile<TEnum> lhs,StatProfile<TEnum> rhs)
		{
			return new StatProfile<TEnum>(_Combine(lhs.m_baseStatDict,rhs.m_baseStatDict,-1),_Combine(lhs.m_growthStatDict,rhs.m_growthStatDict,-1));
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