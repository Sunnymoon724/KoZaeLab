using System;
using System.Collections.Generic;
using YamlDotNet.Serialization;
using KZLib.KZUtility;
using KZLib.KZData;

namespace KZLib
{
	public class ConfigMgr : DataSingleton<ConfigMgr>
	{
		private readonly Dictionary<string,IConfig> m_configDict = new();

		/// <summary>
		/// If config is not exist, create new config.
		/// </summary>
		public TConfig Access<TConfig>() where TConfig : class,IConfig,new()
		{
			var key = typeof(TConfig).Name;

			if(!m_configDict.TryGetValue(key,out var config))
			{
				config = Create<TConfig>(key);

				if(config == null)
				{
					LogTag.System.E($"Config file for {key} not found.");

					return null;
				}

				m_configDict.Add(key,config);
			}

			return config as TConfig;
		}

		public void Clear<TConfig>()
		{
			var key = typeof(TConfig).Name;

			if(m_configDict.ContainsKey(key))
			{
				m_configDict.Remove(key);
			}
		}

		protected override void ClearAll()
		{
			m_configDict.Clear();
		}

		private TConfig Create<TConfig>(string name) where TConfig : class,IConfig,new()
		{
			var deserializer = new DeserializerBuilder().Build();
			var text = LoadConfigFile(name);

			if(!text.IsEmpty())
			{
				try
				{
					return deserializer.Deserialize<TConfig>(text);
				}
				catch(Exception exception)
				{
					LogTag.System.E($"Failed to deserialize {name}.yaml [{exception.Message}]");
				}
			}

			return null;
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