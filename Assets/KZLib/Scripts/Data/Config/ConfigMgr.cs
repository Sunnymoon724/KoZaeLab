using System;
using System.Collections.Generic;
using YamlDotNet.Serialization;
using KZLib.KZUtility;
using KZLib.KZData;
using System.IO;
using ConfigData;

namespace KZLib
{
	public class ConfigMgr : Singleton<ConfigMgr>
	{
		private const string c_editorYaml = "Editor.yaml";

		private bool m_disposed = false;

		private readonly Dictionary<string,IConfig> m_configDict = new();

		private readonly static Type[] s_defaultConfigArray = new Type[] { typeof(GameConfig),typeof(OptionConfig),typeof(ServiceConfig),typeof(EditorConfig) };

		protected override void Initialize()
		{
			base.Initialize();

			_Access(typeof(GameConfig));
		}

		protected override void Release(bool disposing)
		{
			if(m_disposed)
			{
				return;
			}

			if(disposing)
			{
				m_configDict.Clear();
			}

			m_disposed = true;

			base.Release(disposing);
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
				config = _Create(key,type);

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

		private IConfig _Create(string name,Type type)
		{
			if(type == typeof(OptionConfig))
			{
				//? OptionConfig -> only use playerPrefs
				return new OptionConfig();
			}

			var deserializer = new DeserializerBuilder().IncludeNonPublicProperties().Build();
			var text = _LoadConfigFile(name);

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

		private string _LoadConfigFile(string name)
		{
			var fileName = $"{name.Replace("Config","")}.yaml";
			var text = string.Empty;

			//? check custom. [only editor]
#if UNITY_EDITOR
			text = FileUtility.ReadFileToText(Path.Combine(Global.CUSTOM_CONFIG_FOLDER_PATH,$"Custom{fileName}"));
#endif

			if(!text.IsEmpty())
			{
				return text;
			}

			text = _ReadConfigFileInAddressable(fileName);

			if(!text.IsEmpty())
			{
				return text;
			}


			//? check resource folder.
#if UNITY_EDITOR

			var routePath = fileName == c_editorYaml ? $"workRes:config:{fileName}" : $"defaultRes:config:{fileName}";
#else
			var routePath = $"defaultRes:config:{fileName}";
#endif

			text = FileUtility.ReadFileToText(RouteMgr.In.GetOrCreateRoute(routePath).AbsolutePath);

			if(!text.IsEmpty())
			{
				return text;
			}

			LogTag.System.E($"{name} yaml file is not exist. generate config first.");

			return null;
		}

		private string _ReadConfigFileInAddressable(string fileName)
		{
			if(fileName == c_editorYaml)
			{
				// Editor is only editor
				return string.Empty;
			}

			//? if use addressable ? check gameResource folder.

			// TODO 어드레서블 체크 및 로드

			return string.Empty;
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