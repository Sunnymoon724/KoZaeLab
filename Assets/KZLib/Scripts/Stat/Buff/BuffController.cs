using System;
using System.Collections.Generic;

namespace KZLib
{
	public class BuffController<TEnum> where TEnum : struct,Enum
	{
		private readonly Dictionary<int,ActiveBuff> m_activeBuffDict = new();

		private readonly List<int> m_expiredBuffList = new();

		public void AddBuff(int buffNum)
		{
			if(m_activeBuffDict.TryGetValue(buffNum,out var buff))
			{
				buff.TryAddStack();
			}
			else
			{
				m_activeBuffDict.Add(buffNum,new ActiveBuff(buffNum));
			}
		}

		public void RemoveBuff(int buffNum,bool forceRemove = false)
		{
			if(m_activeBuffDict.TryGetValue(buffNum,out var buff))
			{
				if(forceRemove)
				{
					m_activeBuffDict.Remove(buffNum);
				}
				else
				{
					buff.TryReduceStack();
				}
			}
		}

		public bool HasBuff(int buffNum)
		{
			return m_activeBuffDict.ContainsKey(buffNum);
		}

		public int GetBuffCount(int buffNum)
		{
			return m_activeBuffDict.TryGetValue(buffNum,out var buff) ? buff.CurrentStackCount : 0;
		}

		public void TickTime(float deltaTime)
		{
			m_expiredBuffList.Clear();

			foreach(var pair in m_activeBuffDict)
			{
				var isExpired = pair.Value.TickTime(deltaTime);

				if(isExpired)
				{
					m_expiredBuffList.Add(pair.Key);
				}
			}

			for(var i=0;i<m_expiredBuffList.Count;i++)
			{
				m_activeBuffDict.Remove(m_expiredBuffList[i]);
			}
		}

		public float GetModifiedStat(TEnum statType,float baseValue)
		{
			var statName = statType.ToString();
			var flatBonus = 0.0f;
			var percentBonus = 0.0f;

			foreach(var pair in m_activeBuffDict)
			{
				var buff = pair.Value;

				var buffEntryArray = buff.BuffEntryArray;
				
				for(int i=0;i<buffEntryArray.Length;i++)
				{
					var buffEntry = buffEntryArray[i];

					if(!buffEntry.StatName.IsEqual(statName))
					{
						continue;
					}

					var buffValue = buffEntry.Value*buff.CurrentStackCount;

					if(buffEntry.IsPercent)
					{
						percentBonus += buffValue;
					}
					else
					{
						flatBonus += buffValue;
					}
				}
			}

			return (baseValue+flatBonus)*(1.0f+percentBonus/100.0f);
		}

		public IEnumerable<ActiveBuff> GetActiveBuffGroup()
		{
			return m_activeBuffDict.Values;
		}
	}
}
