using System.Collections.Generic;
using Mono.Data.Sqlite;
using KZLib.KZUtility;
using Newtonsoft.Json;
using System;

namespace KZLib
{
	/// <summary>
	/// saving local data using Sql. / Encrypted, but not secure.
	/// </summary>
	public class LocalStorageMgr : Singleton<LocalStorageMgr>
	{
		private const string c_database_name = "KZLibDB.db";

		// Key (tableName) Value[ Key (key) Value (data) ]
		private readonly Dictionary<string,Dictionary<string,string>> m_cacheDataDict = new();

		private SqliteConnection m_connection = null;
		private SqliteCommand m_command = null;

		private bool m_disposed = false;

		protected override void Initialize()
		{
			m_connection = new SqliteConnection(DataBasePath);
			m_connection.Open();
			m_cacheDataDict.Clear();
		}

		protected override void Release(bool disposing)
		{
			if(m_disposed)
			{
				return;
			}

			if(disposing)
			{
				m_command?.Dispose();
				m_connection?.Close();

				m_command = null;
				m_connection = null;
			}

			m_disposed = true;

			base.Release(disposing);
		}

		public void LoadLocalData(string tableName)
		{
			if(tableName.IsEmpty())
			{
				return;
			}

			if(HasTable(tableName))
			{
				if(!m_cacheDataDict.ContainsKey(tableName))
				{
					m_cacheDataDict.Add(tableName,new Dictionary<string,string>());
				}

				LoadTable(tableName,m_cacheDataDict[tableName]);
			}
			else
			{
				CreateTable(tableName);
			}

			LogTag.System.I($"Sql Load Complete. [{tableName}]");
		}

		public bool HasKey(string tableName,string key)
		{
			if(tableName.IsEmpty() || key.IsEmpty())
			{
				return false;
			}

			if(!m_cacheDataDict.TryGetValue(tableName,out var dataDict))
			{
				return false;
			}

			var password = CommonUtility.GenerateAESKeyByPassword(tableName);
			var encryptKey = CommonUtility.EncryptAES(key,password);

			return dataDict.ContainsKey(encryptKey);
		}

		public IEnumerable<string> GetTableNameGroup()
		{
			if(!ExecuteSql("SELECT name FROM sqlite_master WHERE type = 'table'",out var dataReader))
			{
				yield break;
			}

			using(dataReader)
			{
				while(dataReader.Read())
				{
					yield return dataReader[0]?.ToString();
				}
			}
		}

#if UNITY_EDITOR
		public IReadOnlyDictionary<string,string> GetDataDictInTable(string tableName)
		{
			return m_cacheDataDict.TryGetValue(tableName,out var dataDict) ? dataDict : null;
		}
#endif

		public string GetString(string tableName,string key,string defaultValue = null)
		{
			var value = _GetValue(tableName,key);

			return !value.IsEmpty() ? value : defaultValue;
		}

		public int GetInt(string tableName,string key,int defaultValue = 0)
		{
			return int.TryParse(_GetValue(tableName,key),out var result) ? result : defaultValue;
		}

		public long GetLong(string tableName,string key,long defaultValue = 0L)
		{
			return long.TryParse(_GetValue(tableName,key),out var result) ? result : defaultValue;
		}

		public float GetFloat(string tableName,string key,float defaultValue = 0.0f)
		{
			return float.TryParse(_GetValue(tableName,key),out var result) ? result : defaultValue;
		}

		public double GetDouble(string tableName,string key,double defaultValue = 0.0d)
		{
			return double.TryParse(_GetValue(tableName,key),out var result) ? result : defaultValue;
		}

		public bool GetBool(string tableName,string key,bool defaultValue = true)
		{
			return bool.TryParse(_GetValue(tableName,key),out var result) ? result : defaultValue;
		}

		public TEnum GetEnum<TEnum>(string tableName,string key,TEnum defaultValue = default) where TEnum : struct,Enum
		{
			return Enum.TryParse(_GetValue(tableName,key),true,out TEnum result) ? result : defaultValue;
		}

		public TData GetObject<TData>(string tableName,string key,TData defaultValue = default)
		{
			return (TData) GetObject(tableName,key,typeof(TData),defaultValue);
		}

		public object GetObject(string tableName,string key,Type type,object defaultValue = default)
		{
			var value = _GetValue(tableName,key);

			if(!value.IsEmpty())
			{
				// TODO json의 내용이 더 있으면 기존과 달라졌을때 수정하기!

				var result = JsonConvert.DeserializeObject(value,type);
				var text = JsonConvert.SerializeObject(result);

				if(!text.IsEqual(value))
				{
					SetString(tableName,key,text);
				}

				return result;
			}

			return defaultValue;
		}

		private string _GetValue(string tableName,string key)
		{
			if(tableName.IsEmpty() || key.IsEmpty())
			{
				return null;
			}

			if(!m_cacheDataDict.TryGetValue(tableName,out var dataDict))
			{
				return null;
			}

			var password = CommonUtility.GenerateAESKeyByPassword(tableName);
			var encryptKey = CommonUtility.EncryptAES(key,password);

			if(!dataDict.TryGetValue(encryptKey,out var value))
			{
				return null;
			}

			return CommonUtility.DecryptAES(value,password);
		}

		public bool TryGetData(string tableName,string key,out string _result)
		{
			_result = null;

			return m_cacheDataDict.TryGetValue(tableName,out var dataDict) && dataDict.TryGetValue(key,out _result);
		}

		public void SetString(string tableName,string key,string value)
		{
			_SetValue(tableName,key,value);
		}

		public void SetInt(string tableName,string key,int value)
		{
			_SetValue(tableName,key,value.ToString());
		}

		public void SetLong(string tableName,string key,long value)
		{
			_SetValue(tableName,key,value.ToString());
		}

		public void SetFloat(string tableName,string key,float value)
		{
			_SetValue(tableName,key,value.ToString());
		}

		public void SetDouble(string tableName,string key,double value)
		{
			_SetValue(tableName,key,value.ToString());
		}

		public void SetBool(string tableName,string key,bool value)
		{
			_SetValue(tableName,key,value.ToString());
		}

		public void SetEnum<TEnum>(string tableName,string key,TEnum value) where TEnum : struct
		{
			_SetValue(tableName,key,value.ToString());
		}

		public void SetObject(string tableName,string key,object value)
		{
			_SetValue(tableName,key,JsonConvert.SerializeObject(value));
		}

		private void _SetValue(string tableName,string key,string value)
		{
			if(tableName.IsEmpty() || key.IsEmpty() || value.IsEmpty())
			{
				return;
			}

			if(!m_cacheDataDict.TryGetValue(tableName,out var dataDict))
			{
				return;
			}

			var password = CommonUtility.GenerateAESKeyByPassword(tableName);
			var encryptKey = CommonUtility.EncryptAES(key,password);
			var encryptValue = CommonUtility.EncryptAES(value,password);

			var code = CommonUtility.Base64Encode(encryptValue);

			if(dataDict.ContainsKey(encryptKey))
			{
				dataDict[encryptKey] = encryptValue;

				UpdateToTable(tableName,encryptKey,encryptValue,code);
			}
			else
			{
				dataDict.Add(encryptKey,encryptValue);

				InsertToTable(tableName,encryptKey,encryptValue,code);
			}
		}

		public void RemoveKey(string tableName,string key)
		{
			if(tableName.IsEmpty() || key.IsEmpty())
			{
				return;
			}

			var password = CommonUtility.GenerateAESKeyByPassword(tableName);
			var encryptKey = CommonUtility.EncryptAES(key,password);

			if(m_cacheDataDict.TryGetValue(tableName, out var dataDict) && dataDict.Remove(encryptKey))
			{
				DeleteFromTable(tableName,encryptKey);
			}
		}

		private void InsertToTable(string tableName,string key,string value,string code)
		{
			ExecuteNonQuery($"INSERT INTO {tableName} (key, value, code ) VALUES ( '{key}', '{value}', '{code}')");
		}

		private void UpdateToTable(string tableName,string key,string value,string code)
		{
			ExecuteNonQuery($"UPDATE {tableName} SET value = '{value}', code = '{code}' WHERE key = '{key}'");
		}

		private void DeleteFromTable(string tableName,string key)
		{
			ExecuteNonQuery($"DELETE FROM {tableName} WHERE key = '{key}'");
		}

		private bool HasTable(string tableName)
		{
			var result = ExecuteSql($"SELECT name FROM sqlite_master WHERE type = 'table' AND name = '{tableName}'",out _);

			return result;
		}

		private void CreateTable(string tableName)
		{
			ExecuteNonQuery($"CREATE TABLE {tableName} (key TEXT NOT NULL, value TEXT, code TEXT, UNIQUE (key))");

			m_cacheDataDict.Add(tableName,new Dictionary<string,string>());
		}

		public void DeleteTable(string tableName)
		{
			if(ExecuteNonQuery($"DROP TABLE {tableName}") > 0)
			{
				m_cacheDataDict.Remove(tableName);
			}
		}

		private bool LoadTable(string tableName,Dictionary<string,string> dataDict)
		{
			if(!ExecuteSql($"SELECT * FROM {tableName}",out var dataReader))
			{
				return false;
			}

			using(dataReader)
			{
				var removeList = new List<string>();

				while(dataReader.Read())
				{
					var key = GetValue(dataReader,"key");

					if(key.IsEmpty())
					{
						continue;
					}

					var code = GetValue(dataReader,"code");
					var value = GetValue(dataReader,"value");

					if(code.IsEmpty() || value.IsEmpty())
					{
						removeList.Add(key);

						LogTag.System.W($"{code} == null || {value} == null -> remove {key}");

						continue;
					}

					var encodeValue = CommonUtility.Base64Encode(value);

					if(code.IsEqual(encodeValue))
					{
						dataDict[key] = value;
					}
					else
					{
						removeList.Add(key);

						LogTag.System.W($"data(encode) != code [{encodeValue} != {code}] -> remove {key}");
					}
				}

				foreach(var remove in removeList)
				{
					DeleteFromTable(tableName,remove);
				}
			}

			return true;
		}

		private string GetValue(SqliteDataReader dataReader,string name)
		{
			var index = dataReader.GetOrdinal(name);

			return index != -1 ? dataReader.GetString(index) : null;
		}

		private int ExecuteNonQuery(string commandText)
		{
			using var command = m_connection.CreateCommand();
			command.CommandText = commandText;

			return command.ExecuteNonQuery();
		}

		private bool ExecuteSql(string commandText,out SqliteDataReader dataReader)
		{
			using var command = m_connection.CreateCommand();
			command.CommandText = commandText;
			dataReader = command.ExecuteReader();

			return dataReader.HasRows;
		}

		private string DataBasePath
		{
			get
			{
#if UNITY_EDITOR
			return $"data source={CommonUtility.GetProjectParentPath()}/{c_database_name}";
#elif !UNITY_EDITOR && UNITY_ANDROID
			return $"URI=file:{UnityEngine.Application.persistentDataPath}/{c_database_name}";
#elif !UNITY_EDITOR && UNITY_STANDALONE
			return $"URI=file:{UnityEngine.Application.dataPath}/{c_database_name}";
#else
			return $"data source={UnityEngine.Application.persistentDataPath}/{c_database_name}";
#endif
			}
		}
	}
}