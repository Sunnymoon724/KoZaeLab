#if UNITY_EDITOR
using System;
using System.IO;
using KZLib.Data;
using KZLib.ToolKits;
using KZLib.Utilities;
using UnityEditor;

namespace KZLib.EditorInternal.Menus
{
	public static partial class KZMenuItem
	{
		/// <summary>
		/// excel file -> yaml file
		/// </summary>
		[MenuItem("KZMenu/Config/Generate All Config",false,MenuOrder.Data.GENERATE_CONFIG_ALL)]
		private static void _OnGenerateAllConfig()
		{
			var templateText = KZEditorKit.ReadTemplateText("ConfigTemplate.txt");
			var outputRoute = RouteManager.In.Fetch("generatedScript");
			var errorCount = 0;

			foreach(var configFilePath in KZFileKit.FindExcelFilesInFolder(Global.ConfigFolderPath))
			{
				if(!KZFileKit.IsExcelFile(configFilePath))
				{
					LogChannel.Editor.W($"{configFilePath} is not excel file. -> generate skipped");

					continue;
				}

				try
				{
					var fileName = KZFileKit.GetFileName(configFilePath);

					if(ConfigManager.IsDefaultConfig(fileName))
					{
						LogChannel.Editor.W($"{fileName} is default config. -> generate skipped");

						continue;
					}

					ConfigGenerator.GenerateConfig(configFilePath,outputRoute.AbsolutePath,templateText);

					LogChannel.Editor.I($"{fileName} is generated.");
				}
				catch(Exception exception)
				{
					LogChannel.Editor.E(exception);

					errorCount++;
				}
			}

			if(errorCount > 0)
			{
				_DisplayInfo($"Generate finished with {errorCount} error(s).\nCheck the log.",true);

				return;
			}

			_DisplayGenerateEnd();
		}

		// [MenuItem("KZMenu/Config/Generate GameConfig Template File",false,MenuOrder.Data.GENERATE)]
		// private static void _OnGenerateGameConfigTemplateFile()
		// {
		// 	var configFolderPath = Path.Combine(CommonUtility.GetProjectParentPath(),c_config_path);

		// 	ConfigGenerator.GenerateConfigTemplateFile(configFolderPath,"Game",out var result);

		// 	KZEditorKit.DisplayInfo(result);
		// }

		#region Game Config
		[MenuItem("KZMenu/Config/Game/Generate GameConfig Yaml File",false,MenuOrder.Data.GENERATE)]
		private static void _OnGenerateGameConfigYamlFile()
		{
			_GenerateConfigYamlFile("Game.yaml",new GameConfig(),"defaultRes:config");
		}

		[MenuItem("KZMenu/Config/Game/Generate GameConfig Yaml File",true,MenuOrder.Data.GENERATE)]
		private static bool _IsExistGameConfigYamlFile()
		{
			return _IsExistConfigYamlFile("Game.yaml","defaultRes:config");
		}

		[MenuItem("KZMenu/Config/Game/Generate Custom GameConfig Yaml File",false,MenuOrder.Data.GENERATE)]
		private static void _OnGenerateCustomGameConfigYamlFile()
		{
			_GenerateCustomConfigYamlFile<GameConfig>("CustomGame.yaml");
		}

		[MenuItem("KZMenu/Config/Game/Generate Custom GameConfig Yaml File",true,MenuOrder.Data.GENERATE)]
		private static bool _IsExistCustomGameConfigYamlFile()
		{
			return _IsExistCustomConfigYamlFile("CustomGame.yaml");
		}
		#endregion Game Config

		#region Webhook Config
		[MenuItem("KZMenu/Config/Webhook/Generate WebhookConfig Yaml File",false,MenuOrder.Data.GENERATE)]
		private static void _OnGenerateWebhookConfigYamlFile()
		{
			_GenerateConfigYamlFile("Webhook.yaml",new WebhookConfig(),"defaultRes:config");
		}

		[MenuItem("KZMenu/Config/Webhook/Generate WebhookConfig Yaml File",true,MenuOrder.Data.GENERATE)]
		private static bool _IsExistWebhookConfigYamlFile()
		{
			return _IsExistConfigYamlFile("Webhook.yaml","defaultRes:config");
		}

		[MenuItem("KZMenu/Config/Webhook/Generate Custom WebhookConfig Yaml File",false,MenuOrder.Data.GENERATE)]
		private static void _OnGenerateCustomWebhookConfigYamlFile()
		{
			_GenerateCustomConfigYamlFile<WebhookConfig>("CustomWebhook.yaml");
		}

		[MenuItem("KZMenu/Config/Webhook/Generate Custom WebhookConfig Yaml File",true,MenuOrder.Data.GENERATE)]
		private static bool _IsExistCustomWebhookConfigYamlFile()
		{
			return _IsExistCustomConfigYamlFile("CustomWebhook.yaml");
		}
		#endregion Webhook Config

		#region TestMode Config
		[MenuItem("KZMenu/Config/TestMode/Generate TestModeConfig Yaml File",false,MenuOrder.Data.GENERATE)]
		private static void _OnGenerateTestModeConfigYamlFile()
		{
			_GenerateConfigYamlFile("TestMode.yaml",new TestModeConfig(),"workRes:config");
		}

		[MenuItem("KZMenu/Config/TestMode/Generate TestModeConfig Yaml File",true,MenuOrder.Data.GENERATE)]
		private static bool _IsExistTestModeConfigYamlFile()
		{
			return _IsExistConfigYamlFile("TestMode.yaml","workRes:config");
		}

		[MenuItem("KZMenu/Config/TestMode/Generate Custom TestModeConfig Yaml File",false,MenuOrder.Data.GENERATE)]
		private static void _OnGenerateCustomTestModeConfigYamlFile()
		{
			_GenerateCustomConfigYamlFile<TestModeConfig>("CustomTestMode.yaml");
		}

		[MenuItem("KZMenu/Config/TestMode/Generate Custom TestModeConfig Yaml File",true,MenuOrder.Data.GENERATE)]
		private static bool _IsExistCustomTestModeConfigYamlFile()
		{
			return _IsExistCustomConfigYamlFile("CustomTestMode.yaml");
		}
		#endregion TestMode Config

		#region Common
		private static void _GenerateConfigYamlFile(string fileName,IConfig config,string routePath)
		{
			var yamlRoute = RouteManager.In.Fetch(routePath);

			KZFileKit.CreateFolder(yamlRoute.AbsolutePath);

			var filePath = Path.Combine(yamlRoute.AbsolutePath,fileName);

			if(KZFileKit.IsFileExist(filePath))
			{
				_DisplayInfo($"{fileName} is already exist.",true);

				return;
			}

			try
			{
				ConfigGenerator.GenerateConfigYamlFile(config,filePath);
			}
			catch(Exception exception)
			{
				LogChannel.Editor.E(exception);

				return;
			}

			_DisplayGenerateEnd();
		}

		private static bool _IsExistConfigYamlFile(string fileName,string routePath)
		{
			var yamlRoute = RouteManager.In.Fetch(routePath);

			var filePath = Path.Combine(yamlRoute.AbsolutePath,fileName);

			return !KZFileKit.IsFileExist(filePath);
		}

		private static void _GenerateCustomConfigYamlFile<TConfig>(string fileName) where TConfig : class,IConfig,new()
		{
			KZFileKit.CreateFolder(Global.CustomConfigFolderPath);

			var customFilePath = Path.Combine(Global.CustomConfigFolderPath,fileName);

			if(KZFileKit.IsFileExist(customFilePath))
			{
				if(!KZEditorKit.DisplayCheck("Overwrite custom config",$"{fileName} already exists.\nOverwrite?"))
				{
					return;
				}
			}

			if(!ConfigManager.In.TryFetchConfig<TConfig>(out var config))
			{
				LogChannel.Editor.W($"{typeof(TConfig).Name} is not loaded. Using new instance.");

				config = new TConfig();
			}

			try
			{
				ConfigGenerator.GenerateConfigYamlFile(config,customFilePath);
			}
			catch(Exception exception)
			{
				LogChannel.Editor.E(exception);

				return;
			}

			_DisplayGenerateEnd();
		}

		private static bool _IsExistCustomConfigYamlFile(string fileName)
		{
			return !File.Exists(Path.Combine(Global.CustomConfigFolderPath,fileName));
		}
		#endregion Common

		[MenuItem("KZMenu/Config/Open Default Config Folder",false,MenuOrder.Data.OPEN_CONFIG_DEFAULT)]
		private static void _OnOpenDefaultConfigFolder()
		{
			var configFolderRoute = RouteManager.In.Fetch("defaultRes:config");

			_OpenFolder("DefaultConfig",configFolderRoute.AbsolutePath);
		}

		[MenuItem("KZMenu/Config/Open Custom Config Folder",false,MenuOrder.Data.OPEN_CONFIG_CUSTOM)]
		private static void _OnOpenCustomConfigFolder()
		{
			_OpenFolder("CustomConfig",Global.CustomConfigFolderPath);
		}

		[MenuItem("KZMenu/Config/Open TestMode Config Folder",false,MenuOrder.Data.OPEN_CONFIG_TEST_MODE)]
		private static void _OnOpenTestModeConfigFolder()
		{
			var testModeConfigRoute = RouteManager.In.Fetch("workRes:config");

			_OpenFolder("TestModeConfig",testModeConfigRoute.AbsolutePath);
		}
	}
}
#endif