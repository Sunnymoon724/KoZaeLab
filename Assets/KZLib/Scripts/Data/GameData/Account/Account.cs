﻿using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using KZLib;

namespace GameData
{
	public class Account : IGameData
	{
		private interface IAccountData { }

		private const string c_table_name = "Account_Table";

		private readonly List<string> m_DataKeyList = new();

		private Dictionary<string,IAccountData> m_AccountDataDict = null;

		public virtual void Initialize()
		{
			// EventMgr.In.EnableListener<string>(EventTag.UpdateAccount,OnUpdateData);
		}

		public void Release()
		{
			// EventMgr.In.DisableListener<string>(EventTag.UpdateAccount,OnUpdateData);
		}

		public async UniTask LoadDataAsync()
		{
			m_AccountDataDict = new();

			var gameCfg = ConfigManager.In.Access<ConfigData.GameConfig>();

			if(gameCfg.IsLocalSave)
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

		private IAccountData GetAccountData(string key)
		{
			var type = Type.GetType($"GameData.Account+{key.Replace(" ", "")}");
			var defaultData = Activator.CreateInstance(type) as IAccountData;

			if(LocalStorageMgr.In.HasKey(c_table_name,key))
			{
				return LocalStorageMgr.In.GetObject(c_table_name,key,defaultData);
			}
			else
			{
				LocalStorageMgr.In.SetObject(c_table_name,key,defaultData);

				return defaultData;
			}
		}

		protected virtual async UniTask LoadServerDataAsync()
		{
			await UniTask.Yield();
		}

		private void OnUpdateData(string _key)
		{
			var gameCfg = ConfigManager.In.Access<ConfigData.GameConfig>();

			if(gameCfg.IsLocalSave)
			{
				UpdateLocalData(_key);
			}
			else
			{
				UpdateServerData(_key);
			}
		}

		protected virtual void UpdateLocalData(string key)
		{
			if(m_AccountDataDict.TryGetValue(key,out var data))
			{
				LocalStorageMgr.In.SetObject(c_table_name,key,data);
			}
		}

		protected virtual void UpdateServerData(string key)
		{

		}
	}
}