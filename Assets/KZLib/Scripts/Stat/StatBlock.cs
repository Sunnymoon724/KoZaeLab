using System;
using System.Collections.Generic;

namespace KZLib
{
	public readonly struct StatEntry<TStatType> where TStatType : struct,Enum
	{
		public TStatType Type { get; }
		public float Value { get; }

		public StatEntry(TStatType type,float value)
		{
			Type = type;
			Value = value;
		}
	}

	public class StatBlock<TStatType> where TStatType : struct,Enum
	{
		private readonly Dictionary<TStatType,float> m_statDict = null;

		public StatBlock()
		{
			m_statDict = new();
		}

		private StatBlock(Dictionary<TStatType,float> statDict)
		{
			m_statDict = statDict;
		}

		public void Initialize(params StatEntry<TStatType>[] statArray)
		{
			m_statDict.Clear();

			if(statArray == null)
			{
				return;
			}

			foreach(var stat in statArray)
			{
				if(!m_statDict.TryAdd(stat.Type,stat.Value))
				{
					throw new ArgumentException($"Duplicate stat type: {stat.Type}");
				}
			}
		}

		public float Get(TStatType type)
		{
			return m_statDict.TryGetValue(type,out var value) ? value : 0.0f;
		}

		public bool Has(TStatType statType)
		{
			return m_statDict.ContainsKey(statType);
		}

		public IEnumerable<KeyValuePair<TStatType,float>> GetAll() => m_statDict;

		public void Clear()
		{
			m_statDict.Clear();
		}

		public static StatBlock<TStatType> operator +(StatBlock<TStatType> lhs,StatBlock<TStatType> rhs)
		{
			return _Combine(lhs,rhs,+1);
		}

		public static StatBlock<TStatType> operator -(StatBlock<TStatType> lhs,StatBlock<TStatType> rhs)
		{
			return _Combine(lhs,rhs,-1);
		}

		private static StatBlock<TStatType> _Combine(StatBlock<TStatType> lhs,StatBlock<TStatType> rhs,int sign)
		{
			var statDict = new Dictionary<TStatType,float>(lhs.m_statDict);

			foreach(var pair in rhs.m_statDict)
			{
				statDict.TryGetValue(pair.Key,out var existing);

				statDict[pair.Key] = existing+pair.Value*sign;
			}

			return new StatBlock<TStatType>(statDict);
		}
	}
}