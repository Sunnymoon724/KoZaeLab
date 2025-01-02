using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using KZLib;
using KZLib.KZUtility;

namespace GameData
{
	public class Account : IGameData
	{
		private interface IAccountData { }

		private SaveDataHandler m_SaveHandler = null;
		private readonly List<string> m_DataKeyList = new();

		private Dictionary<string,IAccountData> m_AccountDataDict = null;

		public virtual void Initialize()
		{
			m_SaveHandler = new("Account_Table",true);

			// EventMgr.In.EnableListener<string>(EventTag.UpdateAccount,OnUpdateData);
		}

		public void Release()
		{
			// EventMgr.In.DisableListener<string>(EventTag.UpdateAccount,OnUpdateData);
		}

		public async UniTask LoadDataAsync()
		{
			m_AccountDataDict = new();

			if(GameSettings.In.IsLocalSave)
			{
				await LoadLocalDataAsync();
			}
			else
			{
				await LoadServerDataAsync();
			}
		}

		protected virtual async UniTask LoadLocalDataAsync()
		{
			foreach(var key in m_DataKeyList)
			{
				m_AccountDataDict.Add(key,GetAccountData(key));
			}

			await UniTask.Yield();
		}

		private IAccountData GetAccountData(string _key)
		{
			var type = Type.GetType($"GameData.Account+{_key.Replace(" ", "")}");

			if(m_SaveHandler.HasKey(_key))
			{
				return m_SaveHandler.GetObject(_key,type) as IAccountData;
			}
			else
			{
				var data = Activator.CreateInstance(type) as IAccountData;

				m_SaveHandler.SetObject(_key,data);

				return data;
			}
		}

		protected virtual async UniTask LoadServerDataAsync()
		{
			await UniTask.Yield();
		}

		private void OnUpdateData(string _key)
		{
			if(GameSettings.In.IsLocalSave)
			{
				UpdateLocalData(_key);
			}
			else
			{
				UpdateServerData(_key);
			}
		}

		protected virtual void UpdateLocalData(string _key)
		{
			if(m_AccountDataDict.TryGetValue(_key,out var data))
			{
				m_SaveHandler.SetObject(_key,data);
			}
		}

		protected virtual void UpdateServerData(string _key)
		{

		}
	}
}