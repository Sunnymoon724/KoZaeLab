#if UNITY_EDITOR
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;
using System;
using KZLib.KZAttribute;

namespace KZLib.KZWindow
{
	/// <summary>
	/// 로컬 저장 데이터 관리
	/// </summary>
	public class SaveDataWindow : OdinEditorWindow
	{
		#region Local Save Data
		[Serializable]
		private class SaveData
		{
			[SerializeField,HideInInspector]
			private string m_Key = null;

			[SerializeField,HideInInspector]
			private string m_Data = null;

			[TitleGroup("$m_Key",BoldTitle = false),HideLabel,ShowInInspector,MultiLineProperty(3)]
			public string Data
			{
				get => m_Data;
				private set
				{
					if(m_Data.IsEqual(value))
					{
						return;
					}

					m_Data = value;

					var key = RealKey;
					var data = m_IsEncrypted ? CommonUtility.AESEncryptData(m_TableName,value) : value;

					SaveDataMgr.In.SetData(m_TableName,key,data);

					Log.Editor.I(string.Format("{0}의 {1}의 값이 {2}로 변경 되었습니다.",m_TableName,key,data));
				}
			}

			public string RealKey => m_IsEncrypted ? CommonUtility.AESEncryptData(m_TableName,m_Key) : m_Key;

			private readonly bool m_IsEncrypted = false;
			private readonly string m_TableName = null;

			public SaveData(string _tableName,string _key,string _data,bool _isEncrypted)
			{
				m_TableName = _tableName;
				m_IsEncrypted = _isEncrypted;

				m_Key = _key;
				m_Data = _data;
			}
		}
		#endregion Local Save Data

		private string m_TableName = null;

		[VerticalGroup("0",Order = 0),LabelText("테이블 이름"),ShowInInspector,ValueDropdown(nameof(TableNameList)),ShowIf(nameof(IsExistTable))]
		private string TableName
		{
			get => m_TableName;
			set
			{
				if(m_TableName == value)
				{
					return;
				}

				m_TableName = value;

				m_DataList.Clear();

				if(m_TableName.IsEmpty())
				{
					return;
				}

				var dataDict = SaveDataMgr.In.GetDataInTable(value);

				if(dataDict.IsNullOrEmpty())
				{
					return;
				}

				var isEncrypted = true;

				var data = dataDict.First();

				try
				{
					var key = CommonUtility.AESDecryptData(value,data.Key);
				}
				catch(CryptographicException)
				{
					isEncrypted = false;
				}

				foreach(var pair in SaveDataMgr.In.GetDataInTable(TableName))
				{
					var key = isEncrypted ? CommonUtility.AESDecryptData(TableName,pair.Key) : pair.Key;
					var result = isEncrypted ? CommonUtility.AESDecryptData(TableName,pair.Value) : pair.Value;

					m_DataList.Add(new SaveData(TableName,key,result,isEncrypted));
				}
			}
		}

		private bool IsSelectedTable => IsExistTable && !TableName.IsEmpty();

		[VerticalGroup("1",Order = 1),LabelText(" "),SerializeField,ListDrawerSettings(ShowFoldout = false,DraggableItems = false,HideAddButton = true,CustomRemoveIndexFunction = nameof(OnRemoveData)),ShowIf(nameof(IsSelectedTable))]
		private List<SaveData> m_DataList = new();

		[VerticalGroup("1",Order = 1),HideLabel,ShowInInspector,HideIf(nameof(IsExistTable)),KZRichText]
		private string InfoText => "테이블이 비어 있습니다.";

		private bool IsExistTable => TableNameList.Count > 0;

		private List<string> m_TableNameList = null;

		private List<string> TableNameList
		{
			get
			{
				if(m_TableNameList == null)
				{
					if(SaveDataMgr.HasInstance)
					{
						SaveDataMgr.In.Dispose();
					}

					m_TableNameList = new List<string>(SaveDataMgr.In.GetTableNameGroup());
				}

				return m_TableNameList;
			}
		}

		protected override void Initialize()
		{
			base.Initialize();

			if(IsExistTable)
			{
				TableName = TableNameList.First();
			}
		}

		private void OnRemoveData(int _index)
		{
			var key = m_DataList[_index].RealKey;

			SaveDataMgr.In.RemoveKey(TableName,key);

			m_DataList.RemoveAt(_index);

			Log.Editor.I(string.Format("{0}의 {1}가 삭제 되었습니다.",TableName,key));
		}

		[VerticalGroup("2",Order = 2),Button("테이블 삭제",ButtonHeight = 30),ShowIf(nameof(IsSelectedTable))]
		private void OnDeleteTable()
		{
			SaveDataMgr.In.DeleteTable(TableName);

			Log.Editor.I(string.Format("{0}이 삭제 되었습니다.",TableName));

			TableName = null;

			m_TableNameList = null;
		}
	}
}
#endif