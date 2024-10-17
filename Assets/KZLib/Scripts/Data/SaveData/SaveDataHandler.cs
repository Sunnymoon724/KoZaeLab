using System;
using Newtonsoft.Json;

namespace KZLib
{
	public class SaveDataHandler
	{
		private readonly string m_TableName = null;

		private readonly bool m_Encrypt = false;

		public SaveDataHandler(string _tableName,bool _encrypt = false)
		{
			m_TableName = _tableName;
			m_Encrypt = _encrypt;

			SaveDataMgr.In.LoadSqlTable(m_TableName);
		}

		public bool HasKey(string _key)
		{
			return SaveDataMgr.In.HasKey(m_TableName,EncryptText(_key));
		}

		public void SetString(string _key,string _string)
		{
			SetData(_key,_string);
		}

		public void SetInt(string _key,int _int32)
		{
			SetData(_key,_int32.ToString());
		}

		public void SetLong(string _key,long _int64)
		{
			SetData(_key,_int64.ToString());
		}

		public void SetFloat(string _key,float _single)
		{
			SetData(_key,_single.ToString());
		}

		public void SetDouble(string _key,double _double)
		{
			SetData(_key,_double.ToString());
		}

		public void SetBool(string _key,bool _boolean)
		{
			SetData(_key,_boolean.ToString());
		}

		public void SetEnum<TEnum>(string _key,TEnum _enum) where TEnum : struct
		{
			SetData(_key,_enum.ToString());
		}

		public void SetObject(string _key,object _object)
		{
			SetData(_key,JsonConvert.SerializeObject(_object));
		}

		private void SetData(string _key,string _data)
		{
			var data = m_Encrypt ? SecurityUtility.AESEncryptData(m_TableName,_data) : _data;

			SaveDataMgr.In.SetData(m_TableName,EncryptText(_key),data);
		}

		public string GetString(string _key,string _default = null)
		{
			var data = GetData(_key);

			return !data.IsEmpty() ? data : _default;
		}

		public int GetInt(string _key,int _default = 0)
		{
			return int.TryParse(GetData(_key),out var result) ? result : _default;
		}

		public long GetLong(string _key,long _default = 0L)
		{
			return long.TryParse(GetData(_key),out var result) ? result : _default;
		}

		public float GetFloat(string _key,float _default = 0.0f)
		{
			return float.TryParse(GetData(_key),out var result) ? result : _default;
		}

		public double GetDouble(string _key,double _default = 0.0d)
		{
			return double.TryParse(GetData(_key),out var result) ? result : _default;
		}

		public bool GetBool(string _key,bool _default = true)
		{
			return bool.TryParse(GetData(_key),out var result) ? result : _default;
		}

		public TEnum GetEnum<TEnum>(string _key,TEnum _default = default) where TEnum : struct
		{
			return Enum.TryParse(GetData(_key),true,out TEnum result) ? result : _default;
		}

		public TData GetObject<TData>(string _key,TData _default = default)
		{
			var data = GetData(_key);

			if(!data.IsEmpty())
			{
				var result = JsonConvert.DeserializeObject<TData>(data);
				var text = JsonConvert.SerializeObject(result);

				if(!text.IsEqual(data))
				{
					SetString(_key,text);
				}

				return result;
			}

			return _default;
		}

		public object GetObject(string _key,Type _type,object _default = default)
		{
			var data = GetData(_key);

			if(!data.IsEmpty())
			{
				var result = JsonConvert.DeserializeObject(data,_type);
				var text = JsonConvert.SerializeObject(result);

				if(!text.IsEqual(data))
				{
					SetString(_key,text);
				}

				return result;
			}

			return _default;
		}

		private string GetData(string _key)
		{
			return SaveDataMgr.In.TryGetData(m_TableName,EncryptText(_key),out var result) ? DecryptText(result) : null;
		}

		public void RemoveKey(string _key)
		{
			SaveDataMgr.In.RemoveKey(m_TableName,EncryptText(_key));
		}

		private string EncryptText(string _text)
		{
			return m_Encrypt ? SecurityUtility.AESEncryptData(m_TableName,_text) : _text;
		}

		private string DecryptText(string _text)
		{
			return m_Encrypt ? SecurityUtility.AESDecryptData(m_TableName,_text) : _text;
		}
	}
}