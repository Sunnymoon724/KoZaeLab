using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using YamlDotNet.Serialization;

namespace KZLib
{
	public interface IConfigData
	{
		string Name { get; }
		string Type { get; }
		bool Writable { get; }
		object Default { get; }

		void Initialize();
		void Release();
	}

	public class ConfigDataMgr : DataSingleton<ConfigDataMgr>
	{
		private readonly Dictionary<string,IConfigData> m_ConfigDataDict = new();

		/// <summary>
		/// If data is not exist, create new data.
		/// </summary>
		public TData Access<TData>() where TData : class,IConfigData,new()
		{
			var key = typeof(TData).Name;

			if(!m_ConfigDataDict.TryGetValue(key,out var data))
			{
				data = CreateData<TData>(key);

				if(data != null)
				{
					data.Initialize();
					m_ConfigDataDict.Add(key, data);
				}
				else
				{
					LogTag.System.E($"Configuration file for {key} not found.");

					return null;
				}
			}

			return data as TData;
		}

		public void Clear<TData>()
		{
			var key = typeof(TData).Name;

			if(m_ConfigDataDict.TryGetValue(key,out var data))
			{
				data.Release();
				m_ConfigDataDict.Remove(key);
			}
		}

		protected override void ClearAll()
		{
			foreach(var value in m_ConfigDataDict.Values)
			{
				value.Release();
			}

			m_ConfigDataDict.Clear();
		}

		private TData CreateData<TData>(string _name) where TData : class,IConfigData,new()
		{
			var yamlDeserializer = new DeserializerBuilder().Build();
			var text = LoadConfigFile(_name);

			if(text.IsEmpty())
			{
				return null;
			}

			try
			{
				return yamlDeserializer.Deserialize<TData>(text);
			}
			catch(Exception _ex)
			{
				LogTag.System.E($"Failed to deserialize {_name}.yaml [{_ex.Message}]");

				return null;
			}
		}

		private string LoadConfigFile(string _name)
		{
			var text = string.Empty;
#if UNITY_EDITOR
			text = ReadConfigFile(CommonUtility.PathCombine(GameSettings.In.CustomConfigDataFilePath,$"Custom{_name}.yaml"));
#endif
			if(text.IsEmpty())
			{
				text = ReadConfigFile(CommonUtility.PathCombine(GameSettings.In.ConfigDataFilePath,$"{_name}.yaml"));

				if(text.IsEmpty())
				{
					// TODO excel 파일로 yaml 파일 만들기
				}
			}

			return text;
		}

		private string ReadConfigFile(string _filePath)
		{
			return CommonUtility.IsFileExist(_filePath) ? CommonUtility.ReadFileToText(_filePath) : string.Empty;
		}
	}
}