using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using KZLib;
using KZLib.KZDevelop;

namespace GameData
{
	public partial class Account : IGameData
	{
		private class Handler : SaveDataHandler
		{
			protected override string TABLE_NAME => "Account_Table";

			protected override bool IsEncrypt => true;
			protected override bool NewSave => true;
		}

		private interface IData { }

		private Handler m_SaveHandler = null;

		private string[] m_DataTypeArray = null;
		private Dictionary<string,IData> m_DataDict = null;

		private DateTime m_CurrentTime = DateTime.Now;

		private bool m_IsAccessCompleteInServer = false;

		public void Initialize()
		{
			m_SaveHandler = new();

			Initialize_Partial();

			Broadcaster.EnableListener<string>(EventTag.UpdateAccount,OnUpdateData);
		}

		public void Release()
		{
			Release_Partial();

			Broadcaster.DisableListener<string>(EventTag.UpdateAccount,OnUpdateData);
		}

		public void StartGame()
		{
			m_CurrentTime = DateTime.Now;
		}

		public async UniTask LoadDataAsync()
		{
			if(GameSettings.In.IsLocalSaveData)
			{
				m_DataDict = new();

				foreach(var key in m_DataTypeArray)
				{
					var type = Type.GetType(string.Format("GameData.Account+{0}",key.Replace(" ","")));
					var data = Activator.CreateInstance(type);

					m_DataDict.Add(key,m_SaveHandler.GetObject(key,type,data) as IData);
				}
			}
			else
			{
				m_IsAccessCompleteInServer = false;

				LoadDataInServer_Partial();

				await UniTask.WaitUntil(() => m_IsAccessCompleteInServer);
			}
		}

		private void OnUpdateData(string _key)
		{
			if(GameSettings.In.IsLocalSaveData)
			{
				var data = m_DataDict[_key];

				m_SaveHandler.SetObject(_key,data);
			}
			else
			{
				//TODO m_DataDict 통으로 서버로 보내기

				// var name = string.Format("Update{0}Data",ACCOUNT_KEY);
				// var parameter = new Dictionary<string,object>()
				// {
				// 	{ string.Format("[Account] {0}",ACCOUNT_KEY),data }
				// };

				// PlayFabMgr.In.ExecuteCloud(name,parameter,(result)=>
				// {
				// 	Log.Data.I("계정 정보 갱신 완료 [{0}]",result);

				// },(error)=>
				// {
				// 	Log.Data.I("계정 정보 갱신 실패 [{0}]",error);
				// });

				// m_IsAccessCompleteInServer = true;
			}
		}

		partial void Initialize_Partial();
		partial void Release_Partial();

		partial void LoadDataInServer_Partial();
		partial void UpdateDataInServer_Partial(string _key);
	}
}