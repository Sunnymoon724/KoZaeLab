using System;
using YamlDotNet.Serialization;
using KZLib.Utilities;
using System.IO;

namespace KZLib.Data
{
	public class ConfigManager : Singleton<ConfigManager>
	{
		private const string c_editorYaml = "Editor.yaml";

		private readonly LazyRegistry<Type,IConfig> m_registry = new();

		private readonly static Type[] s_defaultConfigArray = new Type[] { typeof(GameConfig),typeof(WebhookConfig),typeof(EditorConfig) };

		protected override void _Initialize()
		{
			base._Initialize();

			_FetchConfig(typeof(GameConfig));
		}

		protected override void _Release(bool disposing)
		{
			if(disposing)
			{
				m_registry.Release();
			}

			base._Release(disposing);
		}

		/// <summary>
		/// If config is not exist, load config.
		/// </summary>
		public TConfig FetchConfig<TConfig>() where TConfig : class,IConfig
		{
			var type = typeof(TConfig);

			return _FetchConfig(type) as TConfig;
		}

		/// <summary>
		/// If config is not exist, load config.
		/// </summary>
		private IConfig _FetchConfig(Type type)
		{
			return m_registry.Fetch(type,_TryLoadConfig);
		}

		private bool _TryLoadConfig(Type type,out IConfig config)
		{
			var name = type.Name;
			var text = _LoadConfigFile(name);

			if(!text.IsEmpty())
			{
				var deserializer = new DeserializerBuilder().IncludeNonPublicProperties().Build();

				try
				{
					config = deserializer.Deserialize(text,type) as IConfig;

					return true;
				}
				catch(Exception exception)
				{
					LogChannel.Data.E($"Failed to deserialize {name}.yaml [{exception.Message}]");
				}
			}

			config = null;

			return false;
		}

		private string _LoadConfigFile(string name)
		{
			var fileName = $"{name.Replace("Config","")}.yaml";
			var text = string.Empty;

			//? check custom. [only editor]
#if UNITY_EDITOR
			text = KZFileKit.ReadFileToText(Path.Combine(Global.CUSTOM_CONFIG_FOLDER_PATH,$"Custom{fileName}"));
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
			var routePath = _IsEditorCfg(fileName) ? $"workRes:config:{fileName}" : $"defaultRes:config:{fileName}";
#else
			var routePath = $"defaultRes:config:{fileName}";
#endif
			text = KZFileKit.ReadFileToText(RouteManager.In.FetchRoute(routePath).AbsolutePath);

			if(!text.IsEmpty())
			{
				return text;
			}

			LogChannel.Data.W($"{name} yaml file is not exist. generate config first.");

			return null;
		}

		private string _ReadConfigFileInAddressable(string fileName)
		{
			if(_IsEditorCfg(fileName))
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
			for(var i=0;i<s_defaultConfigArray.Length;i++)
			{
				var name = s_defaultConfigArray[i].Name.Replace("Config","");

				if(filePath.Contains(name))
				{
					return true;
				}
			}

			return false;
		}
		
		private bool _IsEditorCfg(string fileName)
		{
			return fileName == c_editorYaml;
		}
	}
}