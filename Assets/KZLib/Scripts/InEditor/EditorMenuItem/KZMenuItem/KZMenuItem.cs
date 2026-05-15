#if UNITY_EDITOR
using KZLib.Utilities;
using UnityEditor;

namespace KZLib.EditorInternal.Menus
{
	public static partial class KZMenuItem
	{
		private static class MenuOrder
		{
			public static class Option
			{
				private const int DEFAULT			= 0 * Global.MenuOrderMainSpace;

				public const int DELETE				= DEFAULT + 0 * Global.MenuOrderSubSpace;
				public const int FIND				= DEFAULT + 1 * Global.MenuOrderSubSpace;
				public const int MODULE				= DEFAULT + 2 * Global.MenuOrderSubSpace;
				public const int CHECK				= DEFAULT + 3 * Global.MenuOrderSubSpace;
			}

			public static class Explorer
			{
				private const int DEFAULT			= 1 * Global.MenuOrderMainSpace;

				public const int OPEN				= DEFAULT + 0 * Global.MenuOrderSubSpace;
			}

			public static class Data
			{
				private const int DEFAULT			= 2 * Global.MenuOrderMainSpace;

				public const int GENERATE			= DEFAULT + 0 * Global.MenuOrderSubSpace;
				public const int OPEN				= DEFAULT + 1 * Global.MenuOrderSubSpace;
				public const int WINDOW				= DEFAULT + 2 * Global.MenuOrderSubSpace;
			}

			public static class Display
			{
				private const int DEFAULT			= 3 * Global.MenuOrderMainSpace;

				public const int MANUAL				= DEFAULT + 0 * Global.MenuOrderSubSpace;
				public const int CUSTOM				= DEFAULT + 1 * Global.MenuOrderSubSpace;
				public const int TEST				= DEFAULT + 2 * Global.MenuOrderSubSpace;
			}
		}

		private static void _OpenFolder(string name,string folderPath)
		{
			if(!KZFileKit.IsFolderExist(folderPath))
			{
				if(!KZEditorKit.DisplayCheck($"Create {name} folder",$"{name}Folder is not exist.\n you want to create the {name}Folder?"))
				{
					return;
				}

				KZFileKit.CreateFolder(folderPath);
			}

			KZEditorKit.Open(folderPath);
		}

		private static void _DisplayGenerateEnd()
		{
			_DisplayInfo("Generate is done.",true);
		}

		private static void _DisplayInfo(string message,bool refreshAsset)
		{
			KZEditorKit.DisplayInfo(message);

			if(refreshAsset)
			{
				AssetDatabase.Refresh();
			}
		}
	}
}
#endif