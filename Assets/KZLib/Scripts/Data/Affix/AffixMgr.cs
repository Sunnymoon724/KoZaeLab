using System.Collections.Generic;
using KZLib.KZUtility;
using KZLib.KZData;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;

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

		/// <summary>
		/// If affix is not exist, create new affix.
		/// </summary>
		public async UniTask<TAffix> AccessAsync<TAffix>() where TAffix : class,IAffix,new()
		{
			var key = typeof(TAffix).Name;
			var affix = _GetAffix(key);

			if(affix == null)
			{
				var gameCfg = ConfigMgr.In.Access<ConfigData.GameConfig>();

				affix = gameCfg.IsLocalSave ? await _AccessLocalAsync<TAffix>(key) : await _AccessServerAsync<TAffix>(key);

				m_affixDict.Add(key,affix);
			}

			return affix as TAffix;
		}

		public void Revoke<TAffix>()
		{
			var key = typeof(TAffix).Name;
			var affix = _GetAffix(key);

			if(affix == null)
			{
				return;
			}

			affix.Release();

			m_affixDict.Remove(key);
		}

		private IAffix _GetAffix(string key)
		{
			return m_affixDict.TryGetValue(key,out var affix) ? affix : null;
		}

		public async UniTask UpdateAsync<TAffix>() where TAffix : class,IAffix,new()
		{
			var key = typeof(TAffix).Name;
			var affix = _GetAffix(key);

			if(affix == null)
			{
				return;
			}

			var gameCfg = ConfigMgr.In.Access<ConfigData.GameConfig>();

			if(gameCfg.IsLocalSave)
			{
				await _UpdateLocalAsync(key,affix);
			}
			else
			{
				await _UpdateServerAsync(key,affix);
			}
		}

		private async UniTask<TAffix> _AccessLocalAsync<TAffix>(string key) where TAffix : class,IAffix,new()
		{
			if(!PlayerPrefsMgr.In.TryGetObject<TAffix>(key,out var affix))
			{
				affix = new TAffix();

				affix.Initialize();

				await _UpdateLocalAsync(key,affix);
			}

			return affix;
		}

		private async UniTask<TAffix> _AccessServerAsync<TAffix>(string key) where TAffix : class,IAffix,new()
		{
#if KZLIB_PLAY_FAB
			var result = await PlayFabMgr.In.GetMyDataAsync(key);

			if(result.IsEmpty())
			{
				var affix = new TAffix();

				affix.Initialize();

				await _UpdateServerAsync(key,affix);

				return affix;
			}
			else
			{
				return JsonConvert.DeserializeObject<TAffix>(result);
			}
#else
			await UniTask.Yield();

			return null;
#endif
		}

		private async UniTask _UpdateLocalAsync(string key,IAffix affix)
		{
			PlayerPrefsMgr.In.SetObject(key,affix);

			await UniTask.Yield();
		}

		private async UniTask _UpdateServerAsync(string key,IAffix affix)
		{
#if KZLIB_PLAY_FAB
			await PlayFabMgr.In.UpdateUserDataAsync(key,JsonConvert.SerializeObject(affix));
#else
			await UniTask.Yield();
#endif
		}
	}
}