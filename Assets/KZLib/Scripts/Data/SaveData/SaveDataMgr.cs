using System;
using System.Collections.Generic;
using Mono.Data.Sqlite;

namespace KZLib
{
	/// <summary>
	/// saving local data using Sql. / Encrypted, but not secure.
	/// </summary>
	public class SaveDataMgr : DataSingleton<SaveDataMgr>
	{
		private const string DATABASE_NAME = "KZLibDB.db";

		// Key (tableName) Value[ Key (key) Value (data) ]
		private readonly Dictionary<string,Dictionary<string,string>> m_CacheDataDict = new();

		private SqliteConnection m_Connection = null;
		private SqliteCommand m_Command = null;
		private SqliteDataReader m_DataReader = null;

		protected override void Initialize()
		{
			m_Connection = new SqliteConnection(DataBasePath);
			m_Connection.Open();
			m_CacheDataDict.Clear();
		}

		public void LoadSqlTable(string _tableName)
		{
			if(_tableName.IsEmpty())
			{
				return;
			}

			if(HasTable(_tableName))
			{
				if(!m_CacheDataDict.ContainsKey(_tableName))
				{
					m_CacheDataDict.Add(_tableName,new Dictionary<string,string>());
				}

				LoadTable(_tableName,m_CacheDataDict[_tableName]);
			}
			else
			{
				CreateTable(_tableName);
			}

			LogTag.Data.I("Sql Load Complete. [{0}]",_tableName);
		}

		protected override void ClearAll()
		{
			m_Command?.Dispose();
			m_DataReader?.Close();
			m_Connection?.Close();

			m_Command = null;
			m_DataReader = null;
			m_Connection = null;
		}

		public bool HasKey(string _tableName,string _key)
		{
			if(_tableName.IsEmpty() || _key.IsEmpty())
			{
				return false;
			}

			return m_CacheDataDict.ContainsKey(SecurityUtility.AESEncryptData(_tableName,_key));
		}

		public IEnumerable<string> GetTableNameGroup()
		{
			var result = ExecuteSql("SELECT name FROM sqlite_master WHERE type = 'table'");

			if(!result || m_DataReader == null)
			{
				yield break;
			}

			var nameList = new List<string>();

			while(m_DataReader.Read())
			{
				yield return m_DataReader[0].ToString();
			}

			m_DataReader.Close();
		}

		public IReadOnlyDictionary<string,string> GetDataInTable(string _tableName)
		{
			return m_CacheDataDict.TryGetValue(_tableName,out var dataDict) ? dataDict : null;
		}

		public bool TryGetData(string _tableName,string _key,out string _result)
		{
			_result = null;

			return m_CacheDataDict.TryGetValue(_tableName,out var dataDict) && dataDict.TryGetValue(_key,out _result);
		}

		public void SetData(string _tableName,string _key,string _data)
		{
			if(!m_CacheDataDict.TryGetValue(_tableName,out var dataDict))
			{
				throw new ArgumentException($"{_tableName} is not found.");
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

		public void RemoveKey(string _tableName,string _key)
		{
			if(m_CacheDataDict.TryGetValue(_tableName, out var dataDict) && dataDict.Remove(_key))
			{
				DeleteFromTable(_tableName,_key);
			}
		}

		private void InsertToTable(string _tableName,string _key,string _value,string _code)
		{
			ExecuteNonQuery($"INSERT INTO {_tableName} (key, value, code ) VALUES ( '{_key}', '{_value}', '{_code}')");
		}

		private void UpdateToTable(string _tableName,string _key,string _value,string _code)
		{
			ExecuteNonQuery($"UPDATE {_tableName} SET value = '{_value}', code = '{_code}' WHERE key = '{_key}'");

			// ExecuteNonQuery($"update {_tableName} set key = '{_key}', value = '{_value}', code = '{_code}' where key = '{_key}'");
		}

		private void DeleteFromTable(string _tableName,string _key)
		{
			ExecuteNonQuery($"DELETE FROM {_tableName} WHERE key = '{_key}'");
		}

		private bool HasTable(string _tableName)
		{
			var result = ExecuteSql($"SELECT name FROM sqlite_master WHERE type = 'table' AND name = '{_tableName}'");

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
			ExecuteNonQuery($"CREATE TABLE {_tableName} (key TEXT NOT NULL, value TEXT, code TEXT, UNIQUE (key))");

			m_CacheDataDict.Add(_tableName,new Dictionary<string,string>());
		}

		public void DeleteTable(string _tableName)
		{
			if(ExecuteNonQuery($"DROP TABLE {_tableName}") > 0)
			{
				m_CacheDataDict.Remove(_tableName);
			}
		}

		private bool LoadTable(string _tableName,Dictionary<string,string> _dataDict)
		{
			var result = ExecuteSql($"SELECT * FROM {_tableName}");

			if(!result || m_DataReader == null)
			{
				return false;
			}

			var removeList = new List<string>();

			while(m_DataReader.Read())
			{
				var key = GetValue("key");

				if(key == null)
				{
					continue;
				}

				var code = GetValue("code");
				var value = GetValue("value");

				if(code == null || value == null)
				{
					removeList.Add(key);

					LogTag.Data.W($"{code} == null || {value} == null -> remove {key}");

					continue;
				}

				var encode = SecurityUtility.Base64Encode(value);

				if(code.IsEqual(SecurityUtility.Base64Encode(value)))
				{
					_dataDict.AddOrUpdate(key,encode);
				}
				else
				{
					removeList.Add(key);

					LogTag.Data.W($"data(encode) != code [{encode} != {code}] -> remove {key}");
				}
			}

			m_DataReader.Close();

			foreach(var remove in removeList)
			{
				DeleteFromTable(_tableName,remove);
			}

			return true;
		}

		private string GetValue(string _name)
		{
			var index = m_DataReader.GetOrdinal(_name);

			return index != -1 ? m_DataReader.GetString(index) : null;
		}

		private int ExecuteNonQuery(string _source)
		{
			using var command = m_Connection.CreateCommand();
			command.CommandText = _source;

			return command.ExecuteNonQuery();
		}

		private bool ExecuteSql(string _command)
		{
			using var command = m_Connection.CreateCommand();
			command.CommandText = _command;
			m_DataReader = command.ExecuteReader();

			return m_DataReader != null;
		}

		private string DataBasePath
		{
			get
			{
#if UNITY_EDITOR
			return $"data source={FileUtility.GetProjectParentPath()}/{DATABASE_NAME}";
#elif !UNITY_EDITOR && UNITY_ANDROID
			return $"URI=file:{UnityEngine.Application.persistentDataPath}/{DATABASE_NAME}";
#elif !UNITY_EDITOR && UNITY_STANDALONE
			return $"URI=file:{UnityEngine.Application.dataPath}/{DATABASE_NAME}";
#else
			return $"data source={UnityEngine.Application.persistentDataPath}/{DATABASE_NAME}";
#endif
			}
		}
	}
}