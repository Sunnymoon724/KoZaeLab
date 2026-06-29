using System.Collections.Generic;
using KZLib.Utilities;
using Newtonsoft.Json;
using System;
using System.Globalization;
using UnityEngine;

namespace KZLib
{
	/// <summary>
	/// Typed wrapper over Unity <see cref="PlayerPrefs"/> with an in-memory cache.
	/// All values are stored as strings. Not encrypted — do not store secrets here.
	/// </summary>
	public class PlayerPrefsManager : Singleton<PlayerPrefsManager>
	{
		private readonly Dictionary<string,string> m_cacheDict = new();

		private PlayerPrefsManager() { }

		protected override void _Initialize()
		{
			base._Initialize();

			m_cacheDict.Clear();
		}

		protected override void _Release(bool disposing)
		{
			if(disposing)
			{
				m_cacheDict.Clear();
			}

			base._Release(disposing);
		}

		/// <summary>
		/// Returns whether the key exists. On a cache miss, loads the value into <see cref="m_cacheDict"/>.
		/// Does not reflect changes made outside this manager until <see cref="_Initialize"/> runs again.
		/// </summary>
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

			return _TryGetValue(key,out var result) && int.TryParse(result,NumberStyles.Integer,CultureInfo.InvariantCulture,out value);
		}

		public bool TryGetLong(string key,out long value)
		{
			value = default;

			return _TryGetValue(key,out var result) && long.TryParse(result,NumberStyles.Integer,CultureInfo.InvariantCulture,out value);
		}

		public bool TryGetFloat(string key,out float value)
		{
			value = default;

			return _TryGetValue(key,out var result) && float.TryParse(result,NumberStyles.Float,CultureInfo.InvariantCulture,out value);
		}

		public bool TryGetDouble(string key,out double value)
		{
			value = default;

			return _TryGetValue(key,out var result) && double.TryParse(result,NumberStyles.Float,CultureInfo.InvariantCulture,out value);
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

		/// <summary>
		/// Deserializes JSON stored at <paramref name="key"/>.
		/// Returns false on missing key, invalid JSON, type mismatch, or JSON <c>null</c> for non-nullable value types.
		/// </summary>
		public bool TryGetObject<TValue>(string key,out TValue value)
		{
			value = default;

			if(!_TryGetValue(key,out var json) || !_TryDeserializeObject(key,typeof(TValue),json,out var result))
			{
				return false;
			}

			if(result == null)
			{
				return true;
			}

			if(result is TValue typed)
			{
				value = typed;

				return true;
			}

			value = default;

			return false;
		}

		/// <summary>
		/// Deserializes JSON stored at <paramref name="key"/>.
		/// On success, rewrites storage when Newtonsoft canonical form differs (legacy migration).
		/// Returns false on missing key, invalid JSON, or JSON <c>null</c> for non-nullable value types.
		/// </summary>
		public bool TryGetObject(string key,Type type,out object value)
		{
			if(!_TryGetValue(key,out var result))
			{
				value = default;

				return false;
			}

			return _TryDeserializeObject(key,type,result,out value);
		}

		private bool _TryDeserializeObject(string key,Type type,string json,out object value)
		{
			value = default;

			if(type == null)
			{
				return false;
			}

			if(_IsJsonNullLiteral(json))
			{
				if(type.IsValueType && Nullable.GetUnderlyingType(type) == null)
				{
					return false;
				}

				value = null;

				return true;
			}

			try
			{
				var deserialize = JsonConvert.DeserializeObject(json,type);
				var serialize = JsonConvert.SerializeObject(deserialize);

				// Migrate stored JSON to the current serializer format when parse succeeds but text differs.
				if(!string.Equals(serialize,json))
				{
					_SetValue(key,serialize);
				}

				value = deserialize;

				return true;
			}
			catch(JsonException)
			{
				return false;
			}
		}

		private static bool _IsJsonNullLiteral(string json) => string.Equals(json.Trim(),"null");

		/// <summary>
		/// Reads a cached string or loads from <see cref="PlayerPrefs"/> on first access.
		/// Stale if <see cref="PlayerPrefs"/> was modified without going through this manager.
		/// </summary>
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

		/// <summary>Persists a non-empty string. To clear a key, use <see cref="RemoveKey"/>.</summary>
		public void SetString(string key,string value)
		{
			_SetValue(key,value);
		}

		public void SetInt(string key,int value)
		{
			_SetValue(key,value.ToString(CultureInfo.InvariantCulture));
		}

		public void SetLong(string key,long value)
		{
			_SetValue(key,value.ToString(CultureInfo.InvariantCulture));
		}

		public void SetFloat(string key,float value)
		{
			_SetValue(key,value.ToString(CultureInfo.InvariantCulture));
		}

		public void SetDouble(string key,double value)
		{
			_SetValue(key,value.ToString(CultureInfo.InvariantCulture));
		}

		public void SetBool(string key,bool value)
		{
			_SetValue(key,value.ToString());
		}

		public void SetEnum<TEnum>(string key,TEnum value) where TEnum : struct,Enum
		{
			_SetValue(key,value.ToString());
		}

		public void SetObject(string key,object value)
		{
			_SetValue(key,JsonConvert.SerializeObject(value));
		}

		/// <summary>
		/// Writes to <see cref="PlayerPrefs"/> and cache. Empty key or value is ignored;
		/// empty values cannot be stored — use <see cref="RemoveKey"/> instead.
		/// </summary>
		private void _SetValue(string key,string value)
		{
			if(value.IsEmpty() || key.IsEmpty())
			{
				return;
			}

			PlayerPrefs.SetString(key,value);
			PlayerPrefs.Save();

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
			PlayerPrefs.Save();
		}

		/// <summary>
		/// Deletes every PlayerPrefs entry for this application, including keys not managed here.
		/// Intended for editor/debug tooling only.
		/// </summary>
		public void Clear()
		{
			m_cacheDict.Clear();

			PlayerPrefs.DeleteAll();
			PlayerPrefs.Save();
		}
	}
}