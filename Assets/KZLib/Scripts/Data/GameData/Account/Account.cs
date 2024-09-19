using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using KZLib;

namespace GameData
{
	public class Account : IGameData
	{
		private class Handler : SaveDataHandler
		{
			protected override string TABLE_NAME => "Account_Table";

			protected override bool IsEncrypt => true;
			protected override bool NewSave => true;
		}

		private interface IAccountData { }

		private Handler m_SaveHandler = null;
		private readonly List<string> m_DataKeyList = new();

		private Dictionary<string,IAccountData> m_AccountDataDict = null;

		public virtual void Initialize()
		{
			m_SaveHandler = new();

			Broadcaster.EnableListener<string>(EventTag.UpdateAccount,OnUpdateData);
		}

		public void Release()
		{
			Broadcaster.DisableListener<string>(EventTag.UpdateAccount,OnUpdateData);
		}

		public async UniTask LoadDataAsync()
		{
			m_AccountDataDict = new();

			if(GameSettings.In.IsLocalSaveData)
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
				var type = Type.GetType(string.Format("GameData.Account+{0}",key.Replace(" ","")));
				var data = Activator.CreateInstance(type);

				m_AccountDataDict.Add(key,m_SaveHandler.GetObject(key,type,data) as IAccountData);
			}

			await UniTask.Yield();
		}

		protected virtual async UniTask LoadServerDataAsync()
		{
			await UniTask.Yield();
		}

		private void OnUpdateData(string _key)
		{
			if(GameSettings.In.IsLocalSaveData)
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