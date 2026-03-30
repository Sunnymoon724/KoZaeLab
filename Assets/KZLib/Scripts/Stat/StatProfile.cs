using System;
using System.Collections.Generic;

namespace KZLib
{
	public record StatEntry<TEnum>(TEnum Type,float Value) where TEnum : struct,Enum;

	public class StatProfile<TEnum> where TEnum : struct,Enum
	{
		private readonly Dictionary<TEnum,float> m_baseStatDict = null;
		private readonly Dictionary<TEnum,float> m_growthStatDict = null;

		public StatProfile(StatProfile<TEnum> statProfile)
		{
			m_baseStatDict = new Dictionary<TEnum,float>(statProfile.m_baseStatDict);
			m_growthStatDict = new Dictionary<TEnum,float>(statProfile.m_growthStatDict);
		}

		public StatProfile(StatEntry<TEnum>[] baseStatArray,StatEntry<TEnum>[] growthStatArray)
		{
			m_baseStatDict = new Dictionary<TEnum,float>();
			m_growthStatDict = new Dictionary<TEnum,float>();

			_Initialize(m_baseStatDict,baseStatArray);
			_Initialize(m_growthStatDict,growthStatArray);
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

		public void Add(StatProfile<TEnum> other)
		{
			_Merge(m_baseStatDict,other.m_baseStatDict,+1);
			_Merge(m_growthStatDict,other.m_growthStatDict,+1);
		}

		public void Remove(StatProfile<TEnum> other)
		{
			_Merge(m_baseStatDict,other.m_baseStatDict,-1);
			_Merge(m_growthStatDict,other.m_growthStatDict,-1);
		}

		private static void _Merge(Dictionary<TEnum,float> target,Dictionary<TEnum,float> source,int sign)
		{
			foreach(var pair in source)
			{
				target.TryGetValue(pair.Key,out var existing);

				target[pair.Key] = existing+pair.Value*sign;
			}
		}
	}
}