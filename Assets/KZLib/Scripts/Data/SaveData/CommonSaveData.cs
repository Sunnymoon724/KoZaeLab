using System;

namespace KZLib
{
	public static class CommonSaveData
	{
		private static readonly SaveData s_SaveData = new();

		private class SaveData : SaveDataHandler
		{
			protected override string TABLE_NAME => "Common_Table";
		}

		public static bool HasKey(string _key)
		{
			return s_SaveData.HasKey(_key);
		}

		public static void SetString(string _key,string _data)
		{
			s_SaveData.SetString(_key,_data);
		}

		public static void SetInt(string _key,int _data)
		{
			s_SaveData.SetInt(_key,_data);
		}

		public static void SetFloat(string _key,float _data)
		{
			s_SaveData.SetFloat(_key,_data);
		}

		public static void SetBool(string _key,bool _data)
		{
			s_SaveData.SetBool(_key,_data);
		}

		public static void SetEnum<TEnum>(string _key,TEnum _data) where TEnum : struct
		{
			s_SaveData.SetEnum(_key,_data);
		}

		public static void SetObject<TData>(string _key,TData _data)
		{
			s_SaveData.SetObject(_key,_data);
		}

		public static string GetString(string _key,string _default = null)
		{
			return s_SaveData.GetString(_key,_default);
		}

		public static int GetInt(string _key,int _default = 0)
		{
			return s_SaveData.GetInt(_key,_default);
		}

		public static float GetFloat(string _key,float _default = 0.0f)
		{
			return s_SaveData.GetFloat(_key,_default);
		}

		public static bool GetBool(string _key,bool _default = true)
		{
			return s_SaveData.GetBool(_key,_default);
		}

		public static TEnum GetEnum<TEnum>(string _key,TEnum _default = default) where TEnum : struct
		{
			return s_SaveData.GetEnum(_key,_default);
		}

		public static TData GetObject<TData>(string _key,TData _default = default)
		{
			return s_SaveData.GetObject(_key,_default);
		}

		public static object GetObject(string _key,Type _type,object _default = default)
		{
			return s_SaveData.GetObject(_key,_type,_default);
		}

		public static void RemoveKey(string _key)
		{
			s_SaveData.RemoveKey(_key);
		}
	}
}