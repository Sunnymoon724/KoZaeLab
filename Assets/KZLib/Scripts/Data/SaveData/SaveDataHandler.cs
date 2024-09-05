using System;
using Newtonsoft.Json;

namespace KZLib
{
	public abstract class SaveDataHandler
	{
		protected abstract string TABLE_NAME { get; }

		protected virtual bool IsEncrypt => false;
		protected virtual bool NewSave => false;

		public SaveDataHandler()
		{
			SaveDataMgr.In.LoadSQLTable(TABLE_NAME);
		}

		public bool HasKey(string _key)
		{
			return SaveDataMgr.In.HasKey(TABLE_NAME,EncryptText(_key));
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
			var data = IsEncrypt ? SecurityUtility.AESEncryptData(TABLE_NAME,_data) : _data;

			SaveDataMgr.In.SetData(TABLE_NAME,EncryptText(_key),data);
		}

		public string GetString(string _key,string _default = null)
		{
			var data = GetData(_key);

			if(!data.IsEmpty())
			{
				return data;
			}
			else
			{
				if(NewSave)
				{
					SetString(_key,_default);
				}

				return _default;
			}
		}

		public int GetInt(string _key,int _default = 0)
		{
			try
			{
				if(int.TryParse(GetData(_key),out var result))
				{
					return result;
				}
				else
				{
					if(NewSave)
					{
						SetInt(_key,_default);
					}

					return _default;
				}
			}
			catch(Exception _exception)
			{
				LogTag.Data.E("데이터가 오류나서 초기화 시켰습니다.{0}",_exception);

				SetInt(_key,_default);

				return _default;
			}
		}

		public long GetLong(string _key,long _default = 0L)
		{
			try
			{
				if(long.TryParse(GetData(_key),out var result))
				{
					return result;
				}
				else
				{
					if(NewSave)
					{
						SetLong(_key,_default);
					}

					return _default;
				}
			}
			catch(Exception _exception)
			{
				LogTag.Data.E("데이터가 오류나서 초기화 시켰습니다.{0}",_exception);

				SetLong(_key,_default);

				return _default;
			}
		}

		public float GetFloat(string _key,float _default = 0.0f)
		{
			try
			{
				if(float.TryParse(GetData(_key),out var result))
				{
					return result;
				}
				else
				{
					if(NewSave)
					{
						SetFloat(_key,_default);
					}

					return _default;
				}
			}
			catch(Exception _exception)
			{
				LogTag.Data.E("데이터가 오류나서 초기화 시켰습니다.{0}",_exception);

				SetFloat(_key,_default);

				return _default;
			}
		}

		public double GetDouble(string _key,double _default = 0.0d)
		{
			try
			{
				if(double.TryParse(GetData(_key),out var result))
				{
					return result;
				}
				else
				{
					if(NewSave)
					{
						SetDouble(_key,_default);
					}

					return _default;
				}
			}
			catch(Exception _exception)
			{
				LogTag.Data.E("데이터가 오류나서 초기화 시켰습니다.{0}",_exception);

				SetDouble(_key,_default);

				return _default;
			}
		}

		public bool GetBool(string _key,bool _default = true)
		{
			try
			{
				if(bool.TryParse(GetData(_key),out var result))
				{
					return result;
				}
				else
				{
					if(NewSave)
					{
						SetBool(_key,_default);
					}

					return _default;
				}
			}
			catch(Exception _exception)
			{
				LogTag.Data.E("데이터가 오류나서 초기화 시켰습니다.{0}",_exception);

				SetBool(_key,_default);

				return _default;
			}
		}

		public TEnum GetEnum<TEnum>(string _key,TEnum _default = default) where TEnum : struct
		{
			try
			{
				if(Enum.TryParse(GetData(_key),true,out TEnum result))
				{
					return result;
				}
				else
				{
					if(NewSave)
					{
						SetEnum(_key,_default);
					}

					return _default;
				}
			}
			catch(Exception _exception)
			{
				LogTag.Data.E("데이터가 오류나서 초기화 시켰습니다.{0}",_exception);

				SetEnum(_key,_default);

				return _default;
			}
		}

		public TData GetObject<TData>(string _key,TData _default = default)
		{
			try
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
				else
				{
					if(NewSave)
					{
						SetObject(_key,_default);
					}

					return _default;
				}
			}
			catch(Exception _exception)
			{
				LogTag.Data.E("데이터가 오류나서 초기화 시켰습니다.{0}",_exception);

				SetObject(_key,_default);

				return _default;
			}
		}

		public object GetObject(string _key,Type _type,object _default = default)
		{
			try
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
				else
				{
					if(NewSave)
					{
						SetObject(_key,_default);
					}

					return _default;
				}
			}
			catch(Exception _exception)
			{
				LogTag.Data.E("데이터가 오류나서 초기화 시켰습니다.{0}",_exception);

				SetObject(_key,_default);

				return _default;
			}
		}

		private string GetData(string _key)
		{
			return SaveDataMgr.In.TryGetData(TABLE_NAME,EncryptText(_key),out var result) ? DecryptText(result) : null;
		}

		public void RemoveKey(string _key)
		{
			SaveDataMgr.In.RemoveKey(TABLE_NAME,EncryptText(_key));
		}

		private string EncryptText(string _text)
		{
			return IsEncrypt ? SecurityUtility.AESEncryptData(TABLE_NAME,_text) : _text;
		}

		private string DecryptText(string _text)
		{
			return IsEncrypt ? SecurityUtility.AESDecryptData(TABLE_NAME,_text) : _text;
		}
	}
}