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
		/// <summary>
		/// excel file -> yaml file
		/// </summary>
		[MenuItem("KZMenu/Config/Generate All Config",false,(int) MenuType.Data_GenerateAll)]
		private static void OnGenerateAllConfig()
		{
			var templateFilePath = CommonUtility.FindTemplateText("ConfigTemplate.txt");
			var outputRoute = RouteMgr.In.GetOrCreateRoute("defaultRes:config");

			var stringBuilder = new StringBuilder();

			foreach(var configFilePath in Directory.GetFiles(Global.CONFIG_FOLDER_PATH))
			{
				if(!FileUtility.IsExcelFile(configFilePath))
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

		// [MenuItem("KZMenu/Config/Generate GameConfig Template File",false,(int) MenuType.Data_Generate)]
		// private static void OnGenerateGameConfigTemplateFile()
		// {
		// 	var configFolderPath = Path.Combine(CommonUtility.GetProjectParentPath(),c_config_path);

		// 	ConfigGenerator.GenerateConfigTemplateFile(configFolderPath,"Game",out var result);

		// 	CommonUtility.DisplayInfo(result);
		// }

		#region Game Config
		[MenuItem("KZMenu/Config/Game/Generate GameConfig Yaml File",false,(int) MenuType.Data_Generate)]
		private static void OnGenerateGameConfigYamlFile()
		{
			_GenerateConfigYamlFile("Game.yaml",new GameConfig(),"defaultRes:config");
		}

		[MenuItem("KZMenu/Config/Game/Generate GameConfig Yaml File",true,(int) MenuType.Data_Generate)]
		private static bool IsExistGameConfigYamlFile()
		{
			return _IsExistConfigYamlFile("Game.yaml");
		}

		[MenuItem("KZMenu/Config/Game/Generate Custom GameConfig Yaml File",false,(int) MenuType.Data_Generate)]
		private static void OnGenerateCustomGameConfigYamlFile()
		{
			_GenerateCustomConfigYamlFile("CustomGame.yaml",ConfigMgr.In.Access<GameConfig>());
		}

		[MenuItem("KZMenu/Config/Game/Generate Custom GameConfig Yaml File",true,(int) MenuType.Data_Generate)]
		private static bool IsExistCustomGameConfigYamlFile()
		{
			return _IsExistCustomConfigYamlFile("CustomGame.yaml");
		}
		#endregion Game Config

		#region Service Config
		[MenuItem("KZMenu/Config/Service/Generate ServiceConfig Yaml File",false,(int) MenuType.Data_Generate)]
		private static void OnGenerateServiceConfigYamlFile()
		{
			_GenerateConfigYamlFile("Service.yaml",new ServiceConfig(),"defaultRes:config");
		}

		[MenuItem("KZMenu/Config/Service/Generate ServiceConfig Yaml File",true,(int) MenuType.Data_Generate)]
		private static bool IsExistServiceConfigYamlFile()
		{
			return _IsExistConfigYamlFile("Service.yaml");
		}

		[MenuItem("KZMenu/Config/Service/Generate Custom ServiceConfig Yaml File",false,(int) MenuType.Data_Generate)]
		private static void OnGenerateCustomServiceConfigYamlFile()
		{
			_GenerateCustomConfigYamlFile("CustomService.yaml",ConfigMgr.In.Access<ServiceConfig>());
		}

		[MenuItem("KZMenu/Config/Service/Generate Custom ServiceConfig Yaml File",true,(int) MenuType.Data_Generate)]
		private static bool IsExistCustomServiceConfigYamlFile()
		{
			return _IsExistCustomConfigYamlFile("CustomService.yaml");
		}
		#endregion Service Config

		#region Editor Config
		[MenuItem("KZMenu/Config/Editor/Generate EditorConfig Yaml File",false,(int) MenuType.Data_Generate)]
		private static void OnGenerateEditorConfigYamlFile()
		{
			_GenerateConfigYamlFile("Editor.yaml",new EditorConfig(),"workRes:config");
		}

		[MenuItem("KZMenu/Config/Editor/Generate GameConfig Yaml File",true,(int) MenuType.Data_Generate)]
		private static bool IsExistEditorConfigYamlFile()
		{
			return _IsExistConfigYamlFile("Editor.yaml");
		}

		[MenuItem("KZMenu/Config/Editor/Generate Custom EditorConfig Yaml File",false,(int) MenuType.Data_Generate)]
		private static void OnGenerateCustomEditorConfigYamlFile()
		{
			_GenerateCustomConfigYamlFile("CustomEditor.yaml",ConfigMgr.In.Access<EditorConfig>());
		}

		[MenuItem("KZMenu/Config/Editor/Generate Custom EditorConfig Yaml File",true,(int) MenuType.Data_Generate)]
		private static bool IsExistCustomEditorConfigYamlFile()
		{
			return _IsExistCustomConfigYamlFile("CustomEditor.yaml");
		}
		#endregion Game Config

		#region Common
		private static void _GenerateConfigYamlFile(string fileName,IConfig config,string routePath)
		{
			var yamlRoute = RouteMgr.In.GetOrCreateRoute(routePath);

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
			var yamlRoute = RouteMgr.In.GetOrCreateRoute("defaultRes:config");

			return !File.Exists(Path.Combine(yamlRoute.AbsolutePath,fileName));
		}

		private static void _GenerateCustomConfigYamlFile(string fileName,IConfig config)
		{
			if(!Directory.Exists(Global.CUSTOM_CONFIG_FOLDER_PATH))
			{
				Directory.CreateDirectory(Global.CUSTOM_CONFIG_FOLDER_PATH);
			}

			var customFilePath = Path.Combine(Global.CUSTOM_CONFIG_FOLDER_PATH,fileName);

			ConfigGenerator.GenerateConfigYamlFile(config,customFilePath,out var result);

			CommonUtility.DisplayInfo(result);
		}

		private static bool _IsExistCustomConfigYamlFile(string fileName)
		{
			return !File.Exists(Path.Combine(Global.CUSTOM_CONFIG_FOLDER_PATH,fileName));
		}
		#endregion Common

		[MenuItem("KZMenu/Config/Open Default Config Folder",false,(int) MenuType.Data_Open)]
		private static void OnOpenDefaultConfigFolder()
		{
			var configFolderRoute = RouteMgr.In.GetOrCreateRoute("defaultRes:config");

			CommonUtility.Open(configFolderRoute.AbsolutePath);
		}

		[MenuItem("KZMenu/Config/Open Default Config Folder",true,(int) MenuType.Data_Open)]
		private static bool IsExistDefaultConfigFolder()
		{
			var configFolderRoute = RouteMgr.In.GetOrCreateRoute("defaultRes:config");

			return Directory.Exists(configFolderRoute.AbsolutePath);
		}

		[MenuItem("KZMenu/Config/Open Custom Config Folder",false,(int) MenuType.Data_Open)]
		private static void OnOpenCustomConfigFolder()
		{
			CommonUtility.Open(Global.CUSTOM_CONFIG_FOLDER_PATH);
		}

		[MenuItem("KZMenu/Config/Open Custom Config Folder",true,(int) MenuType.Data_Open)]
		private static bool IsExistCustomConfigFolder()
		{
			return Directory.Exists(Global.CUSTOM_CONFIG_FOLDER_PATH);
		}
	}
}
#endif