using System.Collections.Generic;
using KZLib.KZUtility;
using Newtonsoft.Json;
using System;
using UnityEngine;

namespace KZLib
{
	/// <summary>
	/// saving Encrypted, but not secure.
	/// </summary>
	public class PlayerPrefsMgr : Singleton<PlayerPrefsMgr>
	{
		private readonly Dictionary<string,string> m_cacheDict = new();

		private bool m_disposed = false;

		protected override void Initialize()
		{
			m_cacheDict.Clear();
		}

		protected override void Release(bool disposing)
		{
			if(m_disposed)
			{
				return;
			}

			if(disposing)
			{
				m_cacheDict.Clear();
			}

			m_disposed = true;

			base.Release(disposing);
		}

		public bool HasKey(string key)
		{
			if(key.IsEmpty())
			{
				return false;
			}

			if(m_cacheDict.ContainsKey(key))
			{
				return true;
			}

			var result = PlayerPrefs.HasKey(key);

			if(result)
			{
				m_cacheDict.Add(key,PlayerPrefs.GetString(key));
			}

			return result;
		}

		public bool TryGetString(string key,out string value)
		{
			return _TryGetValue(key,out value);
		}

		public bool TryGetInt(string key,out int value)
		{
			value = default;

			return _TryGetValue(key,out var result) && int.TryParse(result,out value);
		}

		public bool TryGetLong(string key,out long value)
		{
			value = default;

			return _TryGetValue(key,out var result) && long.TryParse(result,out value);
		}

		public bool TryGetFloat(string key,out float value)
		{
			value = default;

			return _TryGetValue(key,out var result) && float.TryParse(result,out value);
		}

		public bool TryGetDouble(string key,out double value)
		{
			value = default;

			return _TryGetValue(key,out var result) && double.TryParse(result,out value);
		}

		public bool TryGetBool(string key,out bool value)
		{
			value = default;

			return _TryGetValue(key,out var result) && bool.TryParse(result,out value);
		}

		public bool TryGetEnum<TEnum>(string key,out TEnum value) where TEnum : struct,Enum
		{
			value = default;

			return _TryGetValue(key,out var result) && Enum.TryParse(result,true,out value);
		}

		public bool TryGetObject<TData>(string key,out TData value)
		{
			if(TryGetObject(key,typeof(TData),out var result))
			{
				value = (TData) result;

				return true;
			}
			else
			{
				value = default;

				return false;
			}
		}

		public bool TryGetObject(string key,Type type,out object value)
		{
			if(_TryGetValue(key,out var result))
			{
				// TODO json의 내용이 더 있으면 기존과 달라졌을때 수정하기!

				var deserialize = JsonConvert.DeserializeObject(result,type);
				var serialize = JsonConvert.SerializeObject(result);

				if(!serialize.IsEqual(result))
				{
					_SetValue(key,serialize);
				}

				value = deserialize;

				return true;
			}
			else
			{
				value = default;

				return false;
			}
		}

		private bool _TryGetValue(string key,out string result)
		{
			if(key.IsEmpty())
			{
				result = string.Empty;

				return false;
			}

			if(!m_cacheDict.ContainsKey(key))
			{
				if(!PlayerPrefs.HasKey(key))
				{
					result = string.Empty;

					return false;
				}

				m_cacheDict.Add(key,PlayerPrefs.GetString(key));
			}

			var value = m_cacheDict[key];

			result = value;

			return true;
		}

		public void SetString(string key,string value)
		{
			_SetValue(key,value);
		}

		public void SetInt(string key,int value)
		{
			_SetValue(key,value.ToString());
		}

		public void SetLong(string key,long value)
		{
			_SetValue(key,value.ToString());
		}

		public void SetFloat(string key,float value)
		{
			_SetValue(key,value.ToString());
		}

		public void SetDouble(string key,double value)
		{
			_SetValue(key,value.ToString());
		}

		public void SetBool(string key,bool value)
		{
			_SetValue(key,value.ToString());
		}

		public void SetEnum<TEnum>(string key,TEnum value) where TEnum : struct
		{
			_SetValue(key,value.ToString());
		}

		public void SetObject(string key,object value)
		{
			_SetValue(key,JsonConvert.SerializeObject(value));
		}

		private void _SetValue(string key,string value)
		{
			if(value.IsEmpty() || key.IsEmpty())
			{
				return;
			}

			PlayerPrefs.SetString(key,value);

			m_cacheDict[key] = value;
		}

		public void RemoveKey(string key)
		{
			if(key.IsEmpty())
			{
				return;
			}

			m_cacheDict.Remove(key);
			PlayerPrefs.DeleteKey(key);
		}

		public void Clear()
		{
			m_cacheDict.Clear();
			PlayerPrefs.DeleteAll();
		}
	}
}