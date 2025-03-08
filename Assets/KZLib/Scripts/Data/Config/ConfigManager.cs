using System;
using System.Collections.Generic;
using YamlDotNet.Serialization;
using KZLib.KZUtility;
using KZLib.KZData;
using System.IO;
using ConfigData;

namespace KZLib
{
	public class ConfigManager : DataSingleton<ConfigManager>
	{
		private readonly Dictionary<string,IConfig> m_configDict = new();

		private readonly static Type[] s_defaultConfigArray = new Type[] { typeof(GameConfig),typeof(OptionConfig),typeof(NetworkConfig) };

		protected override void Initialize()
		{
			base.Initialize();

			_Access(typeof(GameConfig));
		}

		/// <summary>
		/// If config is not exist, create new config.
		/// </summary>
		public TConfig Access<TConfig>() where TConfig : class,IConfig,new()
		{
			var type = typeof(TConfig);

			return _Access(type) as TConfig;
		}

		/// <summary>
		/// If config is not exist, create new config.
		/// </summary>
		private IConfig _Access(Type type)
		{
			var key = type.Name;

			if(!m_configDict.TryGetValue(key,out var config))
			{
				config = Create(key,type);

				if(config == null)
				{
					LogTag.System.E($"{key}config is not exist.");

					return null;
				}

				m_configDict.Add(key,config);
			}

			return config;
		}

		public void Clear<TConfig>()
		{
			var key = typeof(TConfig).Name;

			if(m_configDict.ContainsKey(key))
			{
				m_configDict.Remove(key);
			}
		}

		protected override void Clear()
		{
			m_configDict.Clear();
		}

		private IConfig Create(string name,Type type)
		{
			if(type == typeof(OptionConfig))
			{
				//? OptionConfig -> only use playerPrefs
				return new OptionConfig();
			}

			var deserializer = new DeserializerBuilder().IncludeNonPublicProperties().Build();
			var text = LoadConfigFile(name);

			try
			{
				return deserializer.Deserialize(text,type) as IConfig;
			}
			catch(Exception exception)
			{
				LogTag.System.E($"Failed to deserialize {name}.yaml [{exception.Message}]");
			}

			return null;
		}

		private string LoadConfigFile(string name)
		{
			var fileName = $"{name.Replace("Config","")}.yaml";

			var text = string.Empty;

			//? check custom. [only editor]
#if UNITY_EDITOR
			text = ReadConfigFile(Path.Combine(Global.CUSTOM_CONFIG_FOLDER_PATH,$"Custom{fileName}"));
#endif

			if(!text.IsEmpty())
			{
				return text;
			}

			//? if use addressable ? check gameResource folder.

			// TODO 어드레서블 체크 및 로드

			if(!text.IsEmpty())
			{
				return text;
			}

			//? check resource folder.
			var configRoute = RouteManager.In.GetOrCreateRoute($"defaultRes:config:{fileName}");

			text = ReadConfigFile(configRoute.AbsolutePath);

			if(!text.IsEmpty())
			{
				return text;
			}

			LogTag.System.E($"{name} yaml file is not exist. generate config first.");

			return null;
		}

		private string ReadConfigFile(string filePath)
		{
			return File.Exists(filePath) ? File.ReadAllText(filePath) : string.Empty;
		}

		public static bool IsDefaultConfig(string filePath)
		{
			foreach(var type in s_defaultConfigArray)
			{
				var name = type.Name.Replace("Config","");

				if(filePath.Contains(name))
				{
					return true;
				}
			}

			return false;
		}
	}
}