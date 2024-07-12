using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Data.Sqlite;

namespace KZLib
{
	/// <summary>
	/// SQL을 이용해서 로컬 데이터를 저장함 -> 암호화는 하지만 보안 X
	/// </summary>
	public class SaveDataMgr : DataSingleton<SaveDataMgr>
	{
		private const int STRING_TYPE = 1;
		private const string DATABASE_NAME = "KZLibDB.db";

		// Key (tableName) Value[ Key (key) Value (data) ]
		private readonly Dictionary<string,Dictionary<string,string>> m_SaveDataDict = new();

		private SqliteConnection m_Connection = null;
		private SqliteCommand m_Command = null;
		private SqliteDataReader m_DataReader = null;

		protected override void Initialize()
		{
			m_Connection = new SqliteConnection(GetDataBasePath(DATABASE_NAME));
			m_Connection.Open();

			m_SaveDataDict.Clear();
		}

		public void LoadSQLTable(string _tableName)
		{
			if(_tableName.IsEmpty())
			{
				return;
			}

			if(HasTable(_tableName))
			{
				if(!m_SaveDataDict.ContainsKey(_tableName))
				{
					m_SaveDataDict.Add(_tableName,new Dictionary<string,string>());
				}

				LoadTable(_tableName,m_SaveDataDict[_tableName]);
			}
			else
			{
				CreateTable(_tableName);
			}

			LogTag.Data.I("SQL 로드 완료 [{0}]",_tableName);
		}

		protected override void ClearAll()
		{
			if(m_Command != null)
			{
				m_Command.Dispose();
				m_Command = null;
			}

			if(m_DataReader != null)
			{
				m_DataReader.Close();
				m_DataReader = null;
			}

			if(m_Connection != null)
			{
				m_Connection.Close();
				m_Connection = null;
			}
		}

		public bool HasKey(string _tableName,string _key)
		{
			if(_tableName.IsEmpty() || _key.IsEmpty())
			{
				return false;
			}

			return m_SaveDataDict.ContainsKey(CommonUtility.AESEncryptData(_tableName,_key));
		}

		public IEnumerable<string> GetTableNameGroup()
		{
			var result = ExecuteSQL("select name from sqlite_master where type = 'table'");

			if(!result || m_DataReader == null)
			{
				return null;
			}

			var nameList = new List<string>();

			while(m_DataReader.Read())
			{
				nameList.Add(m_DataReader[0].ToString());
			}

			m_DataReader.Close();

			var dataDict = new Dictionary<(string,string),string>();

			for(var i=0;i<nameList.Count;i++)
			{
				var tableName = nameList[i];

				if(!m_SaveDataDict.ContainsKey(tableName))
				{
					m_SaveDataDict.Add(tableName,new Dictionary<string,string>());
				}

				LoadTable(tableName,m_SaveDataDict[tableName]);
			}
			
			return m_SaveDataDict.Where(x=>x.Value.Count > 0).Select(x=>x.Key).ToList();
		}

		public IReadOnlyDictionary<string,string> GetDataInTable(string _tableName)
		{
			if(m_SaveDataDict.TryGetValue(_tableName,out var dataDict))
			{
				return dataDict;
			}

			return null;
		}

		public bool TryGetData(string _tableName,string _key,out string _result)
		{
			_result = null;

			return m_SaveDataDict.TryGetValue(_tableName,out var dataDict) && dataDict.TryGetValue(_key,out _result);
		}

		public void SetData(string _tableName,string _key,string _data)
		{
			var dataDict = m_SaveDataDict[_tableName];
			var code = CommonUtility.Base64Encode(_data);

			if(dataDict.ContainsKey(_key))
			{
				dataDict[_key] = _data;

				UpdateToTable(_tableName,_key,_data,code);
			}
			else
			{
				dataDict.Add(_key,_data);

				InsertToTable(_tableName,_key,_data,code);
			}
		}

		public void RemoveKey(string _tableName,string _key)
		{
			if(m_SaveDataDict.TryGetValue(_tableName,out var dataDict) && dataDict.ContainsKey(_key))
			{
				m_SaveDataDict[_tableName].Remove(_key);

				DeleteFromTable(_tableName,_key);
			}
		}

		private void InsertToTable(string _tableName,string _key,string _value,string _code)
		{
			ExecuteNonQuery(string.Format("insert into {0} (key, dataType, dataString, dataCode ) values ( '{1}', {2}, '{3}', '{4}')",_tableName,_key,STRING_TYPE,_value,_code));
		}

		private void UpdateToTable(string _tableName,string _key,string _value,string _code)
		{
			ExecuteNonQuery(string.Format("update {0} set key = '{1}', dataType = {2}, dataString = '{3}', dataCode = '{4}' where key = '{1}'",_tableName,_key,STRING_TYPE,_value,_code));
		}

		private void DeleteFromTable(string _tableName,string _key)
		{
			ExecuteNonQuery(string.Format("delete from {0} where key = '{1}'",_tableName,_key));
		}

		private bool HasTable(string _tableName)
		{
			var result = ExecuteSQL(string.Format("select name from sqlite_master where type = 'table' and name = '{0}'",_tableName));

			if(!result || m_DataReader == null)
			{
				return false;
			}

			var flag = false;

			while(m_DataReader.Read())
			{
				flag = true;

				break;
			}

			m_DataReader.Close();

			return flag;
		}

		private void CreateTable(string _tableName)
		{
			ExecuteNonQuery(string.Format("create table {0} (key text not null , dataType integer, dataString text, dataCode text, unique (key))",_tableName));

			m_SaveDataDict.Add(_tableName,new Dictionary<string,string>());
		}

		public void DeleteTable(string _tableName)
		{
			var result = ExecuteNonQuery(string.Format("drop table {0}",_tableName));

			if(result > 0)
			{
				m_SaveDataDict[_tableName].Clear();

				m_SaveDataDict.Remove(_tableName);
			}
		}

		private bool LoadTable(string _tableName,Dictionary<string,string> _dataDict)
		{
			if(!SelectFullFromTable(_tableName))
			{
				return false;
			}

			var removeList = new List<string>();

			while(m_DataReader.Read())
			{
				var key = m_DataReader.GetString(m_DataReader.GetOrdinal("key"));
				var type = m_DataReader.GetInt32(m_DataReader.GetOrdinal("dataType"));
				var code = m_DataReader.GetString(m_DataReader.GetOrdinal("dataCode"));

				if(type == STRING_TYPE)
				{
					var data = m_DataReader.GetString(m_DataReader.GetOrdinal("dataString"));

					if(code.IsEqual(CommonUtility.Base64Encode(data)))
					{
						_dataDict.AddOrUpdate(key,data);
					}
					else
					{
						removeList.Add(key);

						throw new NullReferenceException(string.Format("데이터 오류 {0}",CommonUtility.AESDecryptData(_tableName,key)));
					}
				}
				else
				{
					throw new NullReferenceException(string.Format("지원하지 않는 데이터 타입 {0}",type));
				}
			}

			m_DataReader.Close();

			if(!removeList.IsNullOrEmpty())
			{
				for(var i=0;i<removeList.Count;i++)
				{
					DeleteFromTable(_tableName,removeList[i]);
				}
			}

			return true;
		}

		private bool SelectFullFromTable(string _tableName)
		{
			return ExecuteSQL(string.Format("select * from {0}",_tableName));
		}

		private int ExecuteNonQuery(string _source)
		{
			if(m_Connection != null)
			{
				m_Command = m_Connection.CreateCommand();

				if(m_Command != null)
				{
					m_Command.CommandText = _source;

					return m_Command.ExecuteNonQuery();
				}
			}

			return 0;
		}

		private bool ExecuteSQL(string _command)
		{
			if(m_Connection != null)
			{
				m_Command = m_Connection.CreateCommand();

				if(m_Command != null)
				{
					m_Command.CommandText = _command;
					m_DataReader = m_Command.ExecuteReader();

					return m_DataReader != null;
				}
			}

			return false;
		}

		private string GetDataBasePath(string _fileName)
		{
#if UNITY_EDITOR
			return string.Format("data source={0}/{1}",CommonUtility.GetProjectParentPath(),_fileName);
#elif !UNITY_EDITOR && UNITY_ANDROID
			return string.Format("URI=file:{0}/{1}",UnityEngine.Application.persistentDataPath,_fileName);
#elif !UNITY_EDITOR && UNITY_STANDALONE
			return string.Format("URI=file:{0}/{1}",UnityEngine.Application.dataPath,_fileName);
#else
			return string.Format("data source={0}/{1}",UnityEngine.Application.persistentDataPath,_fileName);
#endif
		}
	}
}