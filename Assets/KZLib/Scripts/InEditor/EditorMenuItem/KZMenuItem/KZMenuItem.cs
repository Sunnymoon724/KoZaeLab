#if UNITY_EDITOR
using System.IO;
using KZLib.KZUtility;
using UnityEditor;

namespace KZLib.KZMenu
{
	public partial class KZMenuItem
	{
		private class MenuOrder
		{
			private const int MAIN_GAP = 1000000;
			public const int SUB_GAP = 50;
			
			public class Option
			{
				private const int DEFAULT			= +0 * MAIN_GAP;

				public const int DELETE				= DEFAULT + 0 * SUB_GAP;
				public const int FIND				= DEFAULT + 1 * SUB_GAP;
				public const int MODULE				= DEFAULT + 2 * SUB_GAP;
				public const int CHECK				= DEFAULT + 3 * SUB_GAP;
			}

			public class Explorer
			{
				private const int DEFAULT			= +1 * MAIN_GAP;

				public const int OPEN				= DEFAULT + 0 * SUB_GAP;
			}

			public class Data
			{
				private const int DEFAULT			= +2 * MAIN_GAP;

				public const int GENERATE			= DEFAULT + 0 * SUB_GAP;
				public const int OPEN				= DEFAULT + 1 * SUB_GAP;
				public const int WINDOW				= DEFAULT + 2 * SUB_GAP;
			}

			public class Window
			{
				private const int DEFAULT			= +3 * MAIN_GAP;

				public const int MANUAL				= DEFAULT + 0 * SUB_GAP;
				public const int CUSTOM				= DEFAULT + 1 * SUB_GAP;
				public const int TEST				= DEFAULT + 2 * SUB_GAP;
			}

			public class Scene
			{
				private const int DEFAULT			= +4 * MAIN_GAP;

				public const int QUICK				= DEFAULT + 0 * SUB_GAP;
				public const int CORE				= DEFAULT + 1 * SUB_GAP;
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