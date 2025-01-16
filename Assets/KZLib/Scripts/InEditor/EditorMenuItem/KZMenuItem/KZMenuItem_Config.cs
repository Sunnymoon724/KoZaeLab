#if UNITY_EDITOR
using System;
using System.IO;
using System.Text;
using ConfigData;
using KZLib.KZTool;
using KZLib.KZUtility;
using UnityEditor;

namespace KZLib.KZMenu
{
	public partial class KZMenuItem
	{
		[MenuItem("KZMenu/Config/Generate All Config",false,(int) MenuType.Config_Generate)]
		private static void OnGenerateAllConfig()
		{
			var templateFilePath = CommonUtility.FindTemplateText("ConfigTemplate.txt");
			var outputRoute = RouteMgr.In.GetOrCreateRoute("defaultRes:config");

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

					if(ConfigMgr.IsDefaultConfig(fileName))
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

		[MenuItem("KZMenu/Config/Generate GameConfig Yaml File",false,(int) MenuType.Config_Generate)]
		private static void OnGenerateGameConfigYamlFile()
		{
			var yamlRoute = RouteMgr.In.GetOrCreateRoute("defaultRes:config");

			if(!Directory.Exists(yamlRoute.AbsolutePath))
			{
				Directory.CreateDirectory(yamlRoute.AbsolutePath);
			}

			var gameFilePath = Path.Combine(yamlRoute.AbsolutePath,"Game.yaml");
			var gameConfig = new GameConfig();

			ConfigGenerator.GenerateConfigYamlFile(gameConfig,gameFilePath,out var result);

			CommonUtility.DisplayInfo(result);

			AssetDatabase.Refresh();
		}

		[MenuItem("KZMenu/Config/Generate GameConfig Yaml File",true,(int) MenuType.Config_Generate)]
		private static bool IsExistGameConfigYamlFile()
		{
			var yamlRoute = RouteMgr.In.GetOrCreateRoute("defaultRes:config");
			var gamePath = Path.Combine(yamlRoute.AbsolutePath,"Game.yaml");

			return !File.Exists(gamePath);
		}

		[MenuItem("KZMenu/Config/Generate Custom GameConfig Yaml File",false,(int) MenuType.Config_Generate)]
		private static void OnGenerateCustomGameConfigYamlFile()
		{
			if(!Directory.Exists(Global.CUSTOM_CONFIG_FOLDER_PATH))
			{
				Directory.CreateDirectory(Global.CUSTOM_CONFIG_FOLDER_PATH);
			}

			var customFilePath = Path.Combine(Global.CUSTOM_CONFIG_FOLDER_PATH,"CustomGame.yaml");
			var gameConfig = ConfigMgr.In.Access<GameConfig>();

			ConfigGenerator.GenerateConfigYamlFile(gameConfig,customFilePath,out var result);

			CommonUtility.DisplayInfo(result);
		}

		[MenuItem("KZMenu/Config/Generate GameConfig Yaml File",true,(int) MenuType.Config_Generate)]
		private static bool IsExistCustomGameConfigYamlFile()
		{
			var customFilePath = Path.Combine(Global.CUSTOM_CONFIG_FOLDER_PATH,"CustomGame.yaml");

			return !File.Exists(customFilePath);
		}

		[MenuItem("KZMenu/Config/Open Default Config Folder",false,(int) MenuType.Config_Open)]
		private static void OnOpenDefaultConfigFolder()
		{
			var configFolderRoute = RouteMgr.In.GetOrCreateRoute("defaultRes:config");

			CommonUtility.Open(configFolderRoute.AbsolutePath);
		}

		[MenuItem("KZMenu/Config/Open Default Config Folder",true,(int) MenuType.Config_Open)]
		private static bool IsExistDefaultConfigFolder()
		{
			var configFolderRoute = RouteMgr.In.GetOrCreateRoute("defaultRes:config");

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