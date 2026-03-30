using System;

namespace KZLib
{
	public class StatProfile<TEnum> where TEnum : struct,Enum
	{
		private readonly StatBlock<TEnum> m_baseStat = new();
		private readonly StatBlock<TEnum> m_growthStat = new();
		
		public StatProfile() { }

		private StatProfile(StatBlock<TEnum> baseStat,StatBlock<TEnum> growthStat)
		{
			m_baseStat = baseStat;
			m_growthStat = growthStat;
		}

		public void Initialize(StatEntry<TEnum>[] baseStatArray,StatEntry<TEnum>[] growthStatArray)
		{
			m_baseStat.Initialize(baseStatArray);
			m_growthStat.Initialize(growthStatArray);
		}

		public float GetFinalStat(TEnum type,int level)
		{
			return m_baseStat.Get(type)+m_growthStat.Get(type)*level;
		}

		public static StatProfile<TEnum> operator +(StatProfile<TEnum> lhs,StatProfile<TEnum> rhs)
		{
			return new StatProfile<TEnum>(lhs.m_baseStat+rhs.m_baseStat,lhs.m_growthStat+rhs.m_growthStat);
		}

		public static StatProfile<TEnum> operator -(StatProfile<TEnum> lhs,StatProfile<TEnum> rhs)
		{
			return new StatProfile<TEnum>(lhs.m_baseStat-rhs.m_baseStat,lhs.m_growthStat-rhs.m_growthStat);
		}
	}
}