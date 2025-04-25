#if UNITY_EDITOR
using System;
using System.IO;
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
		[MenuItem("KZMenu/Config/Generate All Config",false,MenuOrder.Data.GENERATE-1)]
		private static void _OnGenerateAllConfig()
		{
			var templateFilePath = CommonUtility.FindTemplateText("ConfigTemplate.txt");
			var outputRoute = RouteMgr.In.GetOrCreateRoute("generatedScript");

			foreach(var configFilePath in FileUtility.FindAllExcelFileGroupByFolderPath(Global.CONFIG_FOLDER_PATH))
			{
				if(!FileUtility.IsExcelFile(configFilePath))
				{
					LogTag.System.W($"{configFilePath} is not exist. -> generate failed");

					continue;
				}

				try
				{
					var fileName = FileUtility.GetFileName(configFilePath);

					if(ConfigMgr.IsDefaultConfig(fileName))
					{
						LogTag.System.W($"{fileName} is default config. -> generate failed");

						continue;
					}

					ConfigGenerator.GenerateConfig(configFilePath,outputRoute.AbsolutePath,templateFilePath);

					LogTag.System.I($"{fileName} is generated.");
				}
				catch(Exception exception)
				{
					LogTag.System.E(exception);

					return;
				}
			}

			_DisplayGenerateEnd();
		}

		// [MenuItem("KZMenu/Config/Generate GameConfig Template File",false,MenuOrder.Data.GENERATE)]
		// private static void _OnGenerateGameConfigTemplateFile()
		// {
		// 	var configFolderPath = Path.Combine(CommonUtility.GetProjectParentPath(),c_config_path);

		// 	ConfigGenerator.GenerateConfigTemplateFile(configFolderPath,"Game",out var result);

		// 	CommonUtility.DisplayInfo(result);
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
			return _IsExistConfigYamlFile("Game.yaml");
		}

		[MenuItem("KZMenu/Config/Game/Generate Custom GameConfig Yaml File",false,MenuOrder.Data.GENERATE)]
		private static void _OnGenerateCustomGameConfigYamlFile()
		{
			_GenerateCustomConfigYamlFile("CustomGame.yaml",ConfigMgr.In.Access<GameConfig>());
		}

		[MenuItem("KZMenu/Config/Game/Generate Custom GameConfig Yaml File",true,MenuOrder.Data.GENERATE)]
		private static bool _IsExistCustomGameConfigYamlFile()
		{
			return _IsExistCustomConfigYamlFile("CustomGame.yaml");
		}
		#endregion Game Config

		#region Service Config
		[MenuItem("KZMenu/Config/Service/Generate ServiceConfig Yaml File",false,MenuOrder.Data.GENERATE)]
		private static void _OnGenerateServiceConfigYamlFile()
		{
			_GenerateConfigYamlFile("Service.yaml",new ServiceConfig(),"defaultRes:config");
		}

		[MenuItem("KZMenu/Config/Service/Generate ServiceConfig Yaml File",true,MenuOrder.Data.GENERATE)]
		private static bool _IsExistServiceConfigYamlFile()
		{
			return _IsExistConfigYamlFile("Service.yaml");
		}

		[MenuItem("KZMenu/Config/Service/Generate Custom ServiceConfig Yaml File",false,MenuOrder.Data.GENERATE)]
		private static void _OnGenerateCustomServiceConfigYamlFile()
		{
			_GenerateCustomConfigYamlFile("CustomService.yaml",ConfigMgr.In.Access<ServiceConfig>());
		}

		[MenuItem("KZMenu/Config/Service/Generate Custom ServiceConfig Yaml File",true,MenuOrder.Data.GENERATE)]
		private static bool _IsExistCustomServiceConfigYamlFile()
		{
			return _IsExistCustomConfigYamlFile("CustomService.yaml");
		}
		#endregion Service Config

		#region Editor Config
		[MenuItem("KZMenu/Config/Editor/Generate EditorConfig Yaml File",false,MenuOrder.Data.GENERATE)]
		private static void _OnGenerateEditorConfigYamlFile()
		{
			_GenerateConfigYamlFile("Editor.yaml",new EditorConfig(),"workRes:config");
		}

		[MenuItem("KZMenu/Config/Editor/Generate GameConfig Yaml File",true,MenuOrder.Data.GENERATE)]
		private static bool _IsExistEditorConfigYamlFile()
		{
			return _IsExistConfigYamlFile("Editor.yaml");
		}

		[MenuItem("KZMenu/Config/Editor/Generate Custom EditorConfig Yaml File",false,MenuOrder.Data.GENERATE)]
		private static void _OnGenerateCustomEditorConfigYamlFile()
		{
			_GenerateCustomConfigYamlFile("CustomEditor.yaml",ConfigMgr.In.Access<EditorConfig>());
		}

		[MenuItem("KZMenu/Config/Editor/Generate Custom EditorConfig Yaml File",true,MenuOrder.Data.GENERATE)]
		private static bool _IsExistCustomEditorConfigYamlFile()
		{
			return _IsExistCustomConfigYamlFile("CustomEditor.yaml");
		}
		#endregion Game Config

		#region Common
		private static void _GenerateConfigYamlFile(string fileName,IConfig config,string routePath)
		{
			var yamlRoute = RouteMgr.In.GetOrCreateRoute(routePath);

			FileUtility.CreateFolder(yamlRoute.AbsolutePath);

			var filePath = Path.Combine(yamlRoute.AbsolutePath,fileName);

			try
			{
				ConfigGenerator.GenerateConfigYamlFile(config,filePath);
			}
			catch(Exception exception)
			{
				LogTag.System.E(exception);

				return;
			}

			_DisplayGenerateEnd();
		}

		private static bool _IsExistConfigYamlFile(string fileName)
		{
			var yamlRoute = RouteMgr.In.GetOrCreateRoute("defaultRes:config");

			return !File.Exists(Path.Combine(yamlRoute.AbsolutePath,fileName));
		}

		private static void _GenerateCustomConfigYamlFile(string fileName,IConfig config)
		{
			FileUtility.CreateFolder(Global.CUSTOM_CONFIG_FOLDER_PATH);

			var customFilePath = Path.Combine(Global.CUSTOM_CONFIG_FOLDER_PATH,fileName);

			try
			{
				ConfigGenerator.GenerateConfigYamlFile(config,customFilePath);
			}
			catch(Exception exception)
			{
				LogTag.System.E(exception);

				return;
			}

			_DisplayGenerateEnd();
		}

		private static bool _IsExistCustomConfigYamlFile(string fileName)
		{
			return !File.Exists(Path.Combine(Global.CUSTOM_CONFIG_FOLDER_PATH,fileName));
		}
		#endregion Common

		[MenuItem("KZMenu/Config/Open Default Config Folder",false,MenuOrder.Data.OPEN)]
		private static void _OnOpenDefaultConfigFolder()
		{
			var configFolderRoute = RouteMgr.In.GetOrCreateRoute("defaultRes:config");

			_OpenFolder("DefaultConfig",configFolderRoute.AbsolutePath);
		}

		[MenuItem("KZMenu/Config/Open Custom Config Folder",false,MenuOrder.Data.OPEN)]
		private static void _OnOpenCustomConfigFolder()
		{
			_OpenFolder("CustomConfig",Global.CUSTOM_CONFIG_FOLDER_PATH);
		}
	}
}
#endif