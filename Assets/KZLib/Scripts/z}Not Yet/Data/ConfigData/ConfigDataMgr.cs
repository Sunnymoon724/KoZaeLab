using System;
using System.Collections.Generic;
using YamlDotNet.Serialization;
using KZLib.KZUtility;

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
		private readonly Dictionary<string,IConfigData> m_configDataDict = new();

		/// <summary>
		/// If data is not exist, create new data.
		/// </summary>
		public TData Access<TData>() where TData : class,IConfigData,new()
		{
			var key = typeof(TData).Name;

			if(!m_configDataDict.TryGetValue(key,out var data))
			{
				data = CreateData<TData>(key);

				if(data != null)
				{
					data.Initialize();
					m_configDataDict.Add(key, data);
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

			if(m_configDataDict.TryGetValue(key,out var data))
			{
				data.Release();
				m_configDataDict.Remove(key);
			}
		}

		protected override void ClearAll()
		{
			foreach(var value in m_configDataDict.Values)
			{
				value.Release();
			}

			m_configDataDict.Clear();
		}

		private TData CreateData<TData>(string name) where TData : class,IConfigData,new()
		{
			var yamlDeserializer = new DeserializerBuilder().Build();
			var text = LoadConfigFile(name);

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
				LogTag.System.E($"Failed to deserialize {name}.yaml [{_ex.Message}]");

				return null;
			}
		}

		private string LoadConfigFile(string name)
		{
			var text = string.Empty;
#if UNITY_EDITOR
			text = ReadConfigFile(CommonUtility.PathCombine(GameSettings.In.CustomConfigDataFilePath,$"Custom{name}.yaml"));
#endif
			if(text.IsEmpty())
			{
				text = ReadConfigFile(CommonUtility.PathCombine(GameSettings.In.ConfigDataFilePath,$"{name}.yaml"));

				if(text.IsEmpty())
				{
					// TODO excel 파일로 yaml 파일 만들기
				}
			}

			return text;
		}

		private string ReadConfigFile(string filePath)
		{
			return CommonUtility.IsFileExist(filePath) ? CommonUtility.ReadFileToText(filePath) : string.Empty;
		}
	}
}