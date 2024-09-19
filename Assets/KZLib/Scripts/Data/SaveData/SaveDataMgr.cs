using System;
using System.Collections.Generic;
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

		/// <summary>
		/// SQL 테이블을 로드합니다.
		/// </summary>
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

		/// <summary>
		/// 데이터가 존재하는지 확인합니다.
		/// </summary>
		public bool HasKey(string _tableName,string _key)
		{
			if(_tableName.IsEmpty() || _key.IsEmpty())
			{
				return false;
			}

			return m_SaveDataDict.ContainsKey(SecurityUtility.AESEncryptData(_tableName,_key));
		}

		/// <summary>
		/// 모든 테이블 이름을 가져옵니다.
		/// </summary>
		public IEnumerable<string> GetTableNameGroup()
		{
			var result = ExecuteSQL("select name from sqlite_master where type = 'table'");

			if(!result || m_DataReader == null)
			{
				yield break;
			}

			var nameList = new List<string>();

			while(m_DataReader.Read())
			{
				LoadSQLTable(m_DataReader[0].ToString());
			}

			m_DataReader.Close();

			foreach(var pair in m_SaveDataDict)
			{
				if(pair.Value.Count > 0)
				{
					yield return pair.Key;
				}
			}
		}

		/// <summary>
		/// 테이블에 저장된 데이터를 가져옵니다.
		/// </summary>
		public IReadOnlyDictionary<string,string> GetDataInTable(string _tableName)
		{
			if(m_SaveDataDict.TryGetValue(_tableName,out var dataDict))
			{
				return dataDict;
			}

			return null;
		}

		/// <summary>
		/// 저장된 데이터를 가져옵니다.
		/// </summary>
		public bool TryGetData(string _tableName,string _key,out string _result)
		{
			_result = null;

			return m_SaveDataDict.TryGetValue(_tableName,out var dataDict) && dataDict.TryGetValue(_key,out _result);
		}

		/// <summary>
		/// 저장된 데이터를 업데이트하거나 추가합니다.
		/// </summary>
		public void SetData(string _tableName,string _key,string _data)
		{
			if(!m_SaveDataDict.TryGetValue(_tableName,out var dataDict))
			{
				throw new ArgumentException($"{_tableName}이름의 테이블은 없습니다.");
			}

			var code = SecurityUtility.Base64Encode(_data);

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

        /// <summary>
        /// 저장된 데이터를 삭제합니다.
        /// </summary>
		public void RemoveKey(string _tableName,string _key)
		{
			if(m_SaveDataDict.TryGetValue(_tableName, out var dataDict) && dataDict.Remove(_key))
            {
                DeleteFromTable(_tableName,_key);
            }
		}

		private void InsertToTable(string _tableName,string _key,string _value,string _code)
		{
			ExecuteNonQuery($"insert into {_tableName} (key, dataType, dataString, dataCode ) values ( '{_key}', {STRING_TYPE}, '{_value}', '{_code}')");
		}

		private void UpdateToTable(string _tableName,string _key,string _value,string _code)
		{
			ExecuteNonQuery($"update {_tableName} set key = '{_key}', dataType = {STRING_TYPE}, dataString = '{_value}', dataCode = '{_code}' where key = '{_key}'");
		}

		private void DeleteFromTable(string _tableName,string _key)
		{
			ExecuteNonQuery($"delete from {_tableName} where key = '{_key}'");
		}

		private bool HasTable(string _tableName)
		{
			var result = ExecuteSQL($"select name from sqlite_master where type = 'table' and name = '{_tableName}'");

			if(!result || m_DataReader == null)
			{
				return false;
			}

			var exists = m_DataReader.Read();
            m_DataReader.Close();

            return exists;
		}

		private void CreateTable(string _tableName)
		{
			ExecuteNonQuery($"create table {_tableName} (key text not null , dataType integer, dataString text, dataCode text, unique (key))");

			m_SaveDataDict.Add(_tableName,new Dictionary<string,string>());
		}

		/// <summary>
		/// 테이블을 삭제합니다.
		/// </summary>
		public void DeleteTable(string _tableName)
		{
			if(ExecuteNonQuery($"drop table {_tableName}") > 0)
			{
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

					if(code.IsEqual(SecurityUtility.Base64Encode(data)))
					{
						_dataDict.AddOrUpdate(key,data);
					}
					else
					{
						removeList.Add(key);

						throw new NullReferenceException($"데이터 오류 {SecurityUtility.AESDecryptData(_tableName,key)}");
					}
				}
				else
				{
					throw new NullReferenceException($"지원하지 않는 데이터 타입 {type}");
				}
			}

			m_DataReader.Close();

			foreach(var remove in removeList)
            {
                DeleteFromTable(_tableName,remove);
            }

			return true;
		}

		private bool SelectFullFromTable(string _tableName)
		{
			return ExecuteSQL($"select * from {_tableName}");
		}

		private int ExecuteNonQuery(string _source)
		{
			using var command = m_Connection.CreateCommand();
			command.CommandText = _source;

			return command.ExecuteNonQuery();
		}

		private bool ExecuteSQL(string _command)
		{
			using var command = m_Connection.CreateCommand();
			command.CommandText = _command;
			m_DataReader = command.ExecuteReader();

			return m_DataReader != null;
		}

		private string GetDataBasePath(string _fileName)
        {
#if UNITY_EDITOR
            return $"data source={FileUtility.GetProjectParentPath()}/{_fileName}";
#elif !UNITY_EDITOR && UNITY_ANDROID
            return $"URI=file:{UnityEngine.Application.persistentDataPath}/{_fileName}";
#elif !UNITY_EDITOR && UNITY_STANDALONE
            return $"URI=file:{UnityEngine.Application.dataPath}/{_fileName}";
#else
            return $"data source={UnityEngine.Application.persistentDataPath}/{_fileName}";
#endif
        }
	}
}