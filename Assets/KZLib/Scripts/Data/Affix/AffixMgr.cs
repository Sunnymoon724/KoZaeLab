using System.Collections.Generic;
using KZLib.KZUtility;
using KZLib.KZData;

namespace KZLib
{
	public class AffixMgr : Singleton<AffixMgr>
	{
		private bool m_disposed = false;

		//? Type / Affix
		private readonly Dictionary<string,IAffix> m_affixDict = new();

		protected override void Release(bool disposing)
		{
			if(m_disposed)
			{
				return;
			}

			if(disposing)
			{
				m_affixDict.Clear();
			}

			m_disposed = true;

			base.Release(disposing);
		}

		public TAffix Set<TAffix>(TAffix newAfx) where TAffix : class,IAffix
		{
			var key = typeof(TAffix).Name;

			if(m_affixDict.TryGetValue(key,out var oldAfx))
			{
				oldAfx.Set(newAfx);

				return oldAfx as TAffix;
			}
			else
			{
				m_affixDict[key] = newAfx;

				return newAfx;
			}
		}

		public TAffix Update<TAffix>(TAffix newAfx) where TAffix : class,IAffix
		{
			var key = typeof(TAffix).Name;

			if(m_affixDict.TryGetValue(key,out var oldAfx))
			{
				oldAfx.Update(newAfx);
			}

			return oldAfx as TAffix;
		}

		public TAffix Get<TAffix>() where TAffix : class,IAffix
		{
			var key = typeof(TAffix).Name;

			if(m_affixDict.TryGetValue(key,out var affix))
			{
				return affix as TAffix;
			}

			return null;
		}

		public void Revoke<TAffix>()
		{
			var key = typeof(TAffix).Name;

			if(m_affixDict.TryGetValue(key,out var afx))
			{
				afx.Release();
				m_affixDict.Remove(key);
			}
		}
	}
}