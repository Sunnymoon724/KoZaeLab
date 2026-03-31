using System;
using System.Text;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KZLib
{
	public class UnitStatController<TEnum> where TEnum : struct,Enum
	{
		private readonly MonoBehaviour m_unitController = null;
		private bool m_showLog = false;

		private readonly StatProfile<TEnum> m_statProfile = null;
		private readonly BuffController<TEnum> m_buffController = null;

		public UnitStatController(MonoBehaviour unitController,StatEntry<TEnum>[] baseStatArray,StatEntry<TEnum>[] growthStatArray,bool showLog = false) : this(unitController,new StatProfile<TEnum>(baseStatArray,growthStatArray),showLog) { }

		public UnitStatController(MonoBehaviour unitController,StatProfile<TEnum> statProfile,bool showLog = false)
		{
			m_unitController = unitController;
			m_statProfile = statProfile;
			m_buffController = new BuffController<TEnum>();

			SetShowLog(showLog);
		}

		public void SetShowLog(bool showLog)
		{
			m_showLog = showLog;
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
				_ShowLog($"add buff [{buffNum}]");
			}
		}

		public void RemoveBuff(int buffNum,bool forceRemove = false)
		{
			m_buffController.RemoveBuff(buffNum,forceRemove);

			if(m_showLog)
			{
				_ShowLog($"remove buff [{buffNum}]");
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
		protected void _OnShowActiveBuffs()
		{
			var builder = new StringBuilder();
			
			foreach(var buff in m_buffController.GetActiveBuffGroup())
			{
				builder.AppendLine($"BuffName : {buff.BuffName}, StackCount : {buff.CurrentStackCount}, RemainingTime: {buff.CurrentRemainingTime}");
			}

			_ShowLog($"Active Buffs :\n{builder}");
		}
#endif

		private void _ShowLog(string text)
		{
			LogChannel.Develop.I($"[UnitStat] {m_unitController.name} {text}");
		}
	}
}
