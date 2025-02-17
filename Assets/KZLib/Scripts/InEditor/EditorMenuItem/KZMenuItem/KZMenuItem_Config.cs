#if UNITY_EDITOR
using System;
using System.IO;
using System.Text;
using ConfigData;
using KZLib.KZData;
using KZLib.KZTool;
using KZLib.KZUtility;
using UnityEditor;

namespace KZLib.KZMenu
{
	public partial class KZMenuItem
	{
		[MenuItem("KZMenu/Config/Generate All Config",false,(int) MenuType.Config_GenerateAll)]
		private static void OnGenerateAllConfig()
		{
			var templateFilePath = CommonUtility.FindTemplateText("ConfigTemplate.txt");
			var outputRoute = RouteManager.In.GetOrCreateRoute("defaultRes:config");

			var stringBuilder = new StringBuilder();

			foreach(var configFilePath in Directory.GetFiles(Global.CONFIG_FOLDER_PATH))
			{
				if(!CommonUtility.IsExcelFile(configFilePath))
				{
					var errorText = $"{configFilePath} is not exist. -> generate failed";

					LogTag.System.E(errorText);

					stringBuilder.AppendLine(errorText);

					continue;
				}

				try
				{
					var fileName = Path.GetFileName(configFilePath);

					if(ConfigManager.IsDefaultConfig(fileName))
					{
						var errorText = $"{fileName} is default config. -> generate failed";

						LogTag.System.E(errorText);

						stringBuilder.AppendLine(errorText);

						continue;
					}

					ConfigGenerator.TryGenerateConfig(configFilePath,outputRoute.AbsolutePath,templateFilePath,out var result);

					stringBuilder.AppendLine(result);
				}
				catch(Exception exception)
				{
					stringBuilder.AppendLine(exception.ToString());

					LogTag.System.E(exception);
				}
			}

			CommonUtility.DisplayInfo(stringBuilder.ToString());
		}

		// [MenuItem("KZMenu/Config/Generate GameConfig Template File",false,(int) MenuType.Config_Generate)]
		// private static void OnGenerateGameConfigTemplateFile()
		// {
		// 	var configFolderPath = Path.Combine(CommonUtility.GetProjectParentPath(),c_config_path);

		// 	ConfigGenerator.GenerateConfigTemplateFile(configFolderPath,"Game",out var result);

		// 	CommonUtility.DisplayInfo(result);
		// }

		#region Game Config
		[MenuItem("KZMenu/Config/Game/Generate GameConfig Yaml File",false,(int) MenuType.Config_Generate)]
		private static void OnGenerateGameConfigYamlFile()
		{
			_GenerateConfigYamlFile("Game.yaml",new GameConfig(),"defaultRes:config");
		}

		[MenuItem("KZMenu/Config/Game/Generate GameConfig Yaml File",true,(int) MenuType.Config_Generate)]
		private static bool IsExistGameConfigYamlFile()
		{
			return _IsExistConfigYamlFile("Game.yaml");
		}

		[MenuItem("KZMenu/Config/Game/Generate Custom GameConfig Yaml File",false,(int) MenuType.Config_Generate)]
		private static void OnGenerateCustomGameConfigYamlFile()
		{
			_GenerateCustomConfigYamlFile("Game.yaml",ConfigManager.In.Access<GameConfig>());
		}

		[MenuItem("KZMenu/Config/Game/Generate Custom GameConfig Yaml File",true,(int) MenuType.Config_Generate)]
		private static bool IsExistCustomGameConfigYamlFile()
		{
			return _IsExistCustomGameConfigYamlFile("Game.yaml");
		}
		#endregion Game Config

		#region Network Config
		[MenuItem("KZMenu/Config/Network/Generate NetworkConfig Yaml File",false,(int) MenuType.Config_Generate)]
		private static void OnGenerateNetworkConfigYamlFile()
		{
			_GenerateConfigYamlFile("Network.yaml",new NetworkConfig(),"defaultRes:config");
		}

		[MenuItem("KZMenu/Config/Network/Generate NetworkConfig Yaml File",true,(int) MenuType.Config_Generate)]
		private static bool IsExistNetworkConfigYamlFile()
		{
			return _IsExistConfigYamlFile("Network.yaml");
		}

		[MenuItem("KZMenu/Config/Network/Generate Custom NetworkConfig Yaml File",false,(int) MenuType.Config_Generate)]
		private static void OnGenerateCustomNetworkConfigYamlFile()
		{
			_GenerateCustomConfigYamlFile("Network.yaml",ConfigManager.In.Access<NetworkConfig>());
		}

		[MenuItem("KZMenu/Config/Network/Generate Custom NetworkConfig Yaml File",true,(int) MenuType.Config_Generate)]
		private static bool IsExistCustomNetworkConfigYamlFile()
		{
			return _IsExistCustomGameConfigYamlFile("Network.yaml");
		}
		#endregion Network Config

		#region Common
		private static void _GenerateConfigYamlFile(string fileName,IConfig config,string routePath)
		{
			var yamlRoute = RouteManager.In.GetOrCreateRoute(routePath);

			if(!Directory.Exists(yamlRoute.AbsolutePath))
			{
				Directory.CreateDirectory(yamlRoute.AbsolutePath);
			}

			var filePath = Path.Combine(yamlRoute.AbsolutePath,fileName);

			ConfigGenerator.GenerateConfigYamlFile(config,filePath,out var result);

			CommonUtility.DisplayInfo(result);

			AssetDatabase.Refresh();
		}

		private static bool _IsExistConfigYamlFile(string fileName)
		{
			var yamlRoute = RouteManager.In.GetOrCreateRoute("defaultRes:config");

			return !File.Exists(Path.Combine(yamlRoute.AbsolutePath,fileName));
		}

		private static void _GenerateCustomConfigYamlFile(string fileName,IConfig config)
		{
			if(!Directory.Exists(Global.CUSTOM_CONFIG_FOLDER_PATH))
			{
				Directory.CreateDirectory(Global.CUSTOM_CONFIG_FOLDER_PATH);
			}

			var customFilePath = Path.Combine(Global.CUSTOM_CONFIG_FOLDER_PATH,$"Custom{fileName}");

			ConfigGenerator.GenerateConfigYamlFile(config,customFilePath,out var result);

			CommonUtility.DisplayInfo(result);
		}

		private static bool _IsExistCustomGameConfigYamlFile(string fileName)
		{
			return !File.Exists(Path.Combine(Global.CUSTOM_CONFIG_FOLDER_PATH,$"Custom{fileName}"));
		}
		#endregion Common

		[MenuItem("KZMenu/Config/Open Default Config Folder",false,(int) MenuType.Config_Open)]
		private static void OnOpenDefaultConfigFolder()
		{
			var configFolderRoute = RouteManager.In.GetOrCreateRoute("defaultRes:config");

			CommonUtility.Open(configFolderRoute.AbsolutePath);
		}

		[MenuItem("KZMenu/Config/Open Default Config Folder",true,(int) MenuType.Config_Open)]
		private static bool IsExistDefaultConfigFolder()
		{
			var configFolderRoute = RouteManager.In.GetOrCreateRoute("defaultRes:config");

			return Directory.Exists(configFolderRoute.AbsolutePath);
		}

		[MenuItem("KZMenu/Config/Open Custom Config Folder",false,(int) MenuType.Config_Open)]
		private static void OnOpenCustomConfigFolder()
		{
			CommonUtility.Open(Global.CUSTOM_CONFIG_FOLDER_PATH);
		}

		[MenuItem("KZMenu/Config/Open Custom Config Folder",true,(int) MenuType.Config_Open)]
		private static bool IsExistCustomConfigFolder()
		{
			return Directory.Exists(Global.CUSTOM_CONFIG_FOLDER_PATH);
		}
	}
}
#endif