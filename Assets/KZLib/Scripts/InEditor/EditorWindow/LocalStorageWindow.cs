#if UNITY_EDITOR
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using KZLib.KZAttribute;

namespace KZLib.KZWindow
{
	public class LocalStorageWindow : OdinEditorWindow
	{
		#region Local Data
		[Serializable]
		private class LocalData
		{
			[SerializeField,HideInInspector]
			private string m_key = null;

			[SerializeField,HideInInspector]
			private string m_value = null;

			[TitleGroup("$m_key",BoldTitle = false),HideLabel,ShowInInspector,MultiLineProperty(3)]
			public string Value
			{
				get => m_value;
				private set
				{
					if(m_value.IsEqual(value))
					{
						return;
					}

					LocalStorageMgr.In.SetObject(m_tableName,CommonUtility.EncryptAES(m_key,m_password),CommonUtility.EncryptAES(value,m_password));

					LogTag.Editor.I($"{m_value} -> {value} in {m_key} in {m_tableName}.");

					m_value = value;
				}
			}

			public string Key => m_key;

			private readonly byte[] m_password = null;

			private readonly string m_tableName = null;

			public LocalData(string tableName,string key,string value)
			{
				m_tableName = tableName;

				m_key = key;
				m_value = value;

				m_password = CommonUtility.GenerateAESKeyByPassword(m_tableName);
			}
		}
		#endregion Local Data

		private string m_tableName = null;

		[VerticalGroup("0",Order = 0),ShowInInspector,ValueDropdown(nameof(TableNameList)),ShowIf(nameof(IsExistTable))]
		private string TableName
		{
			get => m_tableName;
			set
			{
				if(m_tableName == value)
				{
					return;
				}

				m_tableName = value;

				m_localDataList.Clear();

				if(m_tableName.IsEmpty())
				{
					return;
				}

				var dataDict = LocalStorageMgr.In.GetDataDictInTable(value);

				if(dataDict.IsNullOrEmpty())
				{
					return;
				}

				var password = CommonUtility.GenerateAESKeyByPassword(value);

				foreach(var pair in dataDict)
				{
					m_localDataList.Add(new LocalData(TableName,CommonUtility.DecryptAES(pair.Key,password),CommonUtility.DecryptAES(pair.Value,password)));
				}
			}
		}

		private bool IsSelectedTable => IsExistTable && !TableName.IsEmpty();

		[VerticalGroup("1",Order = 1),LabelText(" "),SerializeField,ListDrawerSettings(ShowFoldout = false,DraggableItems = false,HideAddButton = true,CustomRemoveIndexFunction = nameof(OnRemoveData)),ShowIf(nameof(IsSelectedTable))]
		private List<LocalData> m_localDataList = new();

		[VerticalGroup("1",Order = 1),HideLabel,ShowInInspector,HideIf(nameof(IsExistTable)),KZRichText]
		protected string InfoText => "Table is empty";

		private bool IsExistTable => TableNameList.Count > 0;

		private List<string> m_tableNameList = null;

		private List<string> TableNameList
		{
			get
			{
				if(m_tableNameList == null)
				{
					if(LocalStorageMgr.HasInstance)
					{
						LocalStorageMgr.In.Dispose();
					}

					m_tableNameList = new List<string>(LocalStorageMgr.In.GetTableNameGroup());
				}

				return m_tableNameList;
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

		private void OnRemoveData(int index)
		{
			var key = m_localDataList[index].Key;

			LocalStorageMgr.In.RemoveKey(TableName,key);

			m_tableNameList.RemoveAt(index);

			LogTag.Editor.I($"{key} is removed in {TableName}");
		}

		[VerticalGroup("2",Order = 2),Button("Delete Table",ButtonHeight = 30),ShowIf(nameof(IsSelectedTable))]
		private void OnDeleteTable()
		{
			LocalStorageMgr.In.DeleteTable(TableName);

			LogTag.Editor.I($"{TableName} is deleted");

			TableName = null;

			m_tableNameList = null;
		}
	}
}
#endif