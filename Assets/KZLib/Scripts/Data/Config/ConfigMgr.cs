using System;
using System.Collections.Generic;
using YamlDotNet.Serialization;
using KZLib.KZUtility;
using KZLib.KZData;
using System.IO;
using ConfigData;

namespace KZLib
{
	public class ConfigMgr : DataSingleton<ConfigMgr>
	{
		private readonly Dictionary<string,IConfig> m_configDict = new();

		private readonly static Type[] s_default_config_array = new Type[] { typeof(GameConfig) };

		protected override void Initialize()
		{
			base.Initialize();

			//? add default config
			foreach(var type in s_default_config_array)
			{
				_Access(type);
			}
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

		protected override void ClearAll()
		{
			m_configDict.Clear();
		}

		private IConfig Create(string name,Type type)
		{
			var deserializer = new DeserializerBuilder().Build();
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
			var text = string.Empty;

			//? check custom. [only editor]
#if UNITY_EDITOR
			text = ReadConfigFile(Path.Combine(Global.CUSTOM_CONFIG_FOLDER_PATH,$"Custom{name}Config.yaml"));
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
			var configRoute = RouteMgr.In.GetOrCreateRoute($"defaultRes:config:{name}.yaml");

			text = ReadConfigFile(configRoute.AbsolutePath);

			if(!text.IsEmpty())
			{
				return text;
			}

			LogTag.System.E("yaml file is not exist. generate config first.");

			return null;
		}

		private string ReadConfigFile(string filePath)
		{
			return File.Exists(filePath) ? File.ReadAllText(filePath) : string.Empty;
		}

		public static bool IsDefaultConfig(string filePath)
		{
			foreach(var type in s_default_config_array)
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