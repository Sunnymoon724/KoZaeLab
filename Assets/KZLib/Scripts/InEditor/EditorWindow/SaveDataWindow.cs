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
					var data = m_IsEncrypted ? SecurityUtility.AESEncryptData(m_TableName,value) : value;

					SaveDataMgr.In.SetData(m_TableName,key,data);

					LogTag.Editor.I($"{data} is changed in {key} in {m_TableName}.");
				}
			}

			public string RealKey => m_IsEncrypted ? SecurityUtility.AESEncryptData(m_TableName,m_Key) : m_Key;

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

		[VerticalGroup("0",Order = 0),LabelText("Table Name"),ShowInInspector,ValueDropdown(nameof(TableNameList)),ShowIf(nameof(IsExistTable))]
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
					var key = SecurityUtility.AESDecryptData(value,data.Key);
				}
				catch(CryptographicException)
				{
					isEncrypted = false;
				}

				foreach(var pair in SaveDataMgr.In.GetDataInTable(TableName))
				{
					var key = isEncrypted ? SecurityUtility.AESDecryptData(TableName,pair.Key) : pair.Key;
					var result = isEncrypted ? SecurityUtility.AESDecryptData(TableName,pair.Value) : pair.Value;

					m_DataList.Add(new SaveData(TableName,key,result,isEncrypted));
				}
			}
		}

		private bool IsSelectedTable => IsExistTable && !TableName.IsEmpty();

		[VerticalGroup("1",Order = 1),LabelText(" "),SerializeField,ListDrawerSettings(ShowFoldout = false,DraggableItems = false,HideAddButton = true,CustomRemoveIndexFunction = nameof(OnRemoveData)),ShowIf(nameof(IsSelectedTable))]
		private List<SaveData> m_DataList = new();

		[VerticalGroup("1",Order = 1),HideLabel,ShowInInspector,HideIf(nameof(IsExistTable)),KZRichText]
		private string InfoText => "Table is empty";

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

			LogTag.Editor.I($"{key} is removed in {TableName}");
		}

		[VerticalGroup("2",Order = 2),Button("Delete Table",ButtonHeight = 30),ShowIf(nameof(IsSelectedTable))]
		private void OnDeleteTable()
		{
			SaveDataMgr.In.DeleteTable(TableName);

			LogTag.Editor.I($"{TableName} is deleted");

			TableName = null;

			m_TableNameList = null;
		}
	}
}
#endif