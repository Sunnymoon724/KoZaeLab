using System;
using System.IO;
using KZLib.Utilities;
using UnityEngine;
using YamlDotNet.Serialization;

namespace KZLib.Data
{
	/// <summary>
	/// Lazy cache of YAML-backed <see cref="IConfig"/> instances (build/environment settings).
	/// Load order: Custom yaml (editor) → Addressable (optional) → RouteManager path. Requires <see cref="RouteManager"/> for default routes.
	/// </summary>
	/// <remarks>
	/// Distinct from <see cref="TuneManager"/> (PlayerPrefs user settings) and <see cref="FacetManager"/> (server user state).
	/// </remarks>
	public class ConfigManager : Singleton<ConfigManager>
	{
		private const string c_testModeYaml = "TestMode.yaml";

		private static readonly IDeserializer s_deserializer = new DeserializerBuilder().IncludeNonPublicProperties().Build();

		private readonly LazyRegistry<Type,IConfig> m_registry = new();

		private readonly static Type[] s_defaultConfigArray = new Type[]
		{
			typeof(GameConfig),
			typeof(WebhookConfig),
			typeof(TestModeConfig),
		};

		protected override void _Initialize()
		{
			base._Initialize();

			// Route paths are resolved before the first config load.
			_ = RouteManager.In;

			_Fetch(typeof(GameConfig));
		}

		protected override void _Release(bool disposing)
		{
			if(disposing)
			{
				m_registry.Release();
			}

			base._Release(disposing);
		}

		/// <summary>Returns a cached config, loading and deserializing YAML on first access. Throws when load fails.</summary>
		public TConfig Fetch<TConfig>() where TConfig : class,IConfig
		{
			_ValidateConfigType(typeof(TConfig));

			var config = _Fetch(typeof(TConfig));

			if(config is TConfig result)
			{
				return result;
			}

			throw new InvalidOperationException($"Failed to load config [{typeof(TConfig).Name}].");
		}

		/// <summary>Returns a cached config, or false when load or deserialization fails.</summary>
		public bool TryFetch<TConfig>(out TConfig config) where TConfig : class,IConfig
		{
			config = null;

			try
			{
				_ValidateConfigType(typeof(TConfig));
			}
			catch
			{
				return false;
			}

			var loaded = _Fetch(typeof(TConfig));

			if(loaded is TConfig result)
			{
				config = result;

				return true;
			}

			return false;
		}

		private IConfig _Fetch(Type type)
		{
			return m_registry.Fetch(type,_TryLoadConfig);
		}

		private bool _TryLoadConfig(Type type,out IConfig config)
		{
			var name = type.Name;
			var text = _LoadFile(name);

			if(!text.IsEmpty())
			{
				try
				{
					config = s_deserializer.Deserialize(text,type) as IConfig;

					if(config != null)
					{
						return true;
					}

					LogChannel.Data.E($"Deserialized [{name}] is not {nameof(IConfig)}.");
				}
				catch(Exception exception)
				{
					LogChannel.Data.E($"Failed to deserialize {name}.yaml [{exception.Message}]");
				}
			}

			config = null;

			return false;
		}

		private string _LoadFile(string name)
		{
			var fileName = _BuildYamlFileName(name);
			var text = string.Empty;

#if UNITY_EDITOR
			text = KZFileKit.ReadTextFromFile(Path.Combine(Global.CustomConfigFolderPath,$"Custom{fileName}"));
#endif
			if(!text.IsEmpty())
			{
				return text;
			}

#if UNITY_EDITOR
			var routePath = _IsTestModeCfg(fileName) ? $"workRes:config:{fileName}" : $"defaultRes:config:{fileName}";

			text = KZFileKit.ReadTextFromFile(RouteManager.In.Fetch(routePath).AbsolutePath);
#else
			var routePath = $"defaultRes:config:{fileName}";

			var textAsset = ResourceManager.In.GetTextAsset(RouteManager.In.Fetch(routePath).AbsolutePath);

			text = textAsset != null ? textAsset.text : string.Empty;
#endif
			if(!text.IsEmpty())
			{
				return text;
			}

			LogChannel.Data.W($"{name} yaml file does not exist. Generate config first.");

			return null;
		}

		/// <summary>Returns true when the path/name matches a built-in config (Game, Webhook, TestMode, or Custom variants).</summary>
		public static bool IsDefault(string filePath)
		{
			var fileName = Path.GetFileNameWithoutExtension(filePath);

			for(var i=0;i<s_defaultConfigArray.Length;i++)
			{
				var baseName = s_defaultConfigArray[i].Name.Replace("Config","");

				if(fileName == baseName || fileName == $"Custom{baseName}")
				{
					return true;
				}
			}

			return false;
		}

		private static string _BuildYamlFileName(string configTypeName) => $"{configTypeName.Replace("Config","")}.yaml";

		private static void _ValidateConfigType(Type type)
		{
			if(type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			if(type.IsAbstract || type.IsInterface)
			{
				throw new ArgumentException($"Type [{type.Name}] must be a concrete config class.",nameof(type));
			}

			if(!typeof(IConfig).IsAssignableFrom(type))
			{
				throw new ArgumentException($"Type [{type.Name}] must implement {nameof(IConfig)}.",nameof(type));
			}
		}

		private bool _IsTestModeCfg(string fileName)
		{
			return fileName == c_testModeYaml;
		}
	}
}