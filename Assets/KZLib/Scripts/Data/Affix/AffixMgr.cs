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

		private bool m_isLocalSave;
		
		protected override void Initialize()
		{
			base.Initialize();

			var gameCfg = ConfigMgr.In.Access<ConfigData.GameConfig>();

			m_isLocalSave = gameCfg.IsLocalSave;
		}

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

		public IAffix Set(IAffix newAffix)
		{
			var key = typeof(IAffix).Name;

			if(m_affixDict.TryGetValue(key,out var oldAffix))
			{
				oldAffix.Set(newAffix);

				return oldAffix;
			}
			else
			{
				m_affixDict[key] = newAffix;

				return newAffix;
			}
		}

		public IAffix Update(IAffix newAffix)
		{
			var key = typeof(IAffix).Name;

			if(m_affixDict.TryGetValue(key,out var oldAffix))
			{
				oldAffix.Update(newAffix);
			}

			return oldAffix;
		}

		public TAffix Get<TAffix>() where TAffix : class,IAffix
		{
			var key = typeof(IAffix).Name;

			if(m_affixDict.TryGetValue(key,out var affix))
			{
				return affix as TAffix;
			}

			return null;
		}

		public void Revoke<TAffix>()
		{
			var key = typeof(TAffix).Name;

			if(m_affixDict.TryGetValue(key,out var affix))
			{
				affix.Release();
				m_affixDict.Remove(key);
			}
		}
	}
}