#if UNITY_EDITOR
using System.IO;
using KZLib.KZUtility;
using UnityEditor;

namespace KZLib.KZMenu
{
	public static partial class KZMenuItem
	{
		private static class MenuOrder
		{
			public static class Option
			{
				private const int DEFAULT			= 0 * Global.MENU_ORDER_MAIN_SPACE;

				public const int DELETE				= DEFAULT + 0 * Global.MENU_ORDER_SUB_SPACE;
				public const int FIND				= DEFAULT + 1 * Global.MENU_ORDER_SUB_SPACE;
				public const int MODULE				= DEFAULT + 2 * Global.MENU_ORDER_SUB_SPACE;
				public const int CHECK				= DEFAULT + 3 * Global.MENU_ORDER_SUB_SPACE;
			}

			public static class Explorer
			{
				private const int DEFAULT			= 1 * Global.MENU_ORDER_MAIN_SPACE;

				public const int OPEN				= DEFAULT + 0 * Global.MENU_ORDER_SUB_SPACE;
			}

			public static class Data
			{
				private const int DEFAULT			= 2 * Global.MENU_ORDER_MAIN_SPACE;

				public const int GENERATE			= DEFAULT + 0 * Global.MENU_ORDER_SUB_SPACE;
				public const int OPEN				= DEFAULT + 1 * Global.MENU_ORDER_SUB_SPACE;
				public const int WINDOW				= DEFAULT + 2 * Global.MENU_ORDER_SUB_SPACE;
			}

			public static class Display
			{
				private const int DEFAULT			= 3 * Global.MENU_ORDER_MAIN_SPACE;

				public const int MANUAL				= DEFAULT + 0 * Global.MENU_ORDER_SUB_SPACE;
				public const int CUSTOM				= DEFAULT + 1 * Global.MENU_ORDER_SUB_SPACE;
				public const int TEST				= DEFAULT + 2 * Global.MENU_ORDER_SUB_SPACE;
			}
		}

		private static void _OpenFolder(string name,string folderPath)
		{
			if(!FileUtility.IsFolderExist(folderPath))
			{
				if(!CommonUtility.DisplayCheck($"Create {name} Folder",$"{name}Folder is not exist.\n you want to create the {name}Folder?"))
				{
					return;
				}

				FileUtility.CreateFolder(folderPath);
			}

			CommonUtility.Open(folderPath);
		}

		private static void _DisplayGenerateEnd()
		{
			_DisplayInfo("Generate is done.",true);
		}

		private static void _DisplayInfo(string message,bool refreshAsset)
		{
			CommonUtility.DisplayInfo(message);

			if(refreshAsset)
			{
				AssetDatabase.Refresh();
			}
		}

		public static bool IsExcelFile(string filePath)
		{
			string a = Path.GetExtension(filePath).ToLower();
			string[] array = new string[3] { "*.xls", "*.xlsx", "*.xlsm" };
			foreach (string b in array)
			{
				if (string.Equals(a, b))
				{
					return true;
				}
			}

			return false;
		}
	}
}
#endif