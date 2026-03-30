using System;
using System.Text;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KZLib
{
	public class UnitStatController<TEnum> : MonoBehaviour where TEnum : struct,Enum
	{
		[SerializeField]
		private bool m_showLog = false;

		private StatProfile<TEnum> m_statProfile = null;
		private readonly BuffController<TEnum> m_buffController = new();

		public void Initialize(StatProfile<TEnum> statProfile)
		{
			m_statProfile = statProfile;
		}

		public void Initialize(StatEntry<TEnum>[] baseStatArray,StatEntry<TEnum>[] growthStatArray)
		{
			m_statProfile = new StatProfile<TEnum>(baseStatArray,growthStatArray);
		}

		public float GetStat(TEnum statType,int level)
		{
			var baseStat = m_statProfile.GetFinalStat(statType,level);

			return m_buffController.GetModifiedStat(statType,baseStat);
		}

		public void AddBuff(int buffNum)
		{
			m_buffController.AddBuff(buffNum);

			if(m_showLog)
			{
				LogChannel.Develop.I($"[UnitStat] {name} add buff [{buffNum}]");
			}
		}

		public void RemoveBuff(int buffNum,bool forceRemove = false)
		{
			m_buffController.RemoveBuff(buffNum,forceRemove);

			if(m_showLog)
			{
				LogChannel.Develop.I($"[UnitStat] {name} remove buff [{buffNum}]");
			}
		}

		public bool HasBuff(int buffNum)
		{
			return m_buffController.HasBuff(buffNum);
		}

		public void TickTime(float deltaTime)
		{
			m_buffController.TickTime(deltaTime);
		}
		
#if UNITY_EDITOR
		[Button("Show Active Buffs")]
		protected void OnShowActiveBuffs()
		{
			var builder = new StringBuilder();
			
			foreach(var buff in m_buffController.GetActiveBuffGroup())
			{
				builder.AppendLine($"BuffName : {buff.BuffName}, StackCount : {buff.CurrentStackCount}, RemainingTime: {buff.CurrentRemainingTime}");
			}

			LogChannel.Develop.I($"[UnitStat] Active Buffs :\n{builder}");
		}
#endif
	}
}
