#if UNITY_EDITOR
using UnityEditor;

namespace KZLib.EditorInternal.Menus
{
	public static partial class KZMenuItem
	{
		private static class MenuOrder
		{
			public static class Option
			{
				private const int DEFAULT					= 0 * Global.MenuOrderMainSpace;

				public const int DELETE						= DEFAULT + 0 * Global.MenuOrderSubSpace;
				public const int DELETE_ALL_MANAGER			= DELETE + 0;
				public const int DELETE_EMPTY_FOLDER		= DELETE + 1;
				public const int DELETE_UNUSED_ASSETS		= DELETE + 2;

				public const int FIND						= DEFAULT + 1 * Global.MenuOrderSubSpace;
				public const int FIND_MISSING_COMPONENT		= FIND + 0;
				public const int FIND_MISSING_MESH_FILTER	= FIND + 1;

				public const int MODULE						= DEFAULT + 2 * Global.MenuOrderSubSpace;
				public const int MODULE_PLAY_FAB			= MODULE + 0;
				public const int MODULE_IN_APP_PURCHASE		= MODULE + 1;

				public const int CHECK						= DEFAULT + 3 * Global.MenuOrderSubSpace;
			}

			public static class Explorer
			{
				private const int DEFAULT					= 1 * Global.MenuOrderMainSpace;

				public const int OPEN						= DEFAULT + 0 * Global.MenuOrderSubSpace;
				public const int OPEN_ROUTE_FILE			= OPEN + 0;
				public const int OPEN_DOCUMENTS				= OPEN + 1;
				public const int OPEN_TOOLS					= OPEN + 2;
				public const int OPEN_DATA_PATH				= OPEN + 3;
				public const int OPEN_PERSISTENT_DATA_PATH	= OPEN + 4;
			}

			public static class Data
			{
				private const int DEFAULT					= 2 * Global.MenuOrderMainSpace;

				public const int GENERATE					= DEFAULT + 0 * Global.MenuOrderSubSpace;
				public const int GENERATE_PROTO_CURRENT		= GENERATE + 1;
				public const int GENERATE_LINGO				= GENERATE + 2;
				public const int GENERATE_PROTO_ALL			= GENERATE + 3;
				public const int GENERATE_CONFIG_ALL		= GENERATE + 4;

				public const int OPEN						= DEFAULT + 1 * Global.MenuOrderSubSpace;
				public const int OPEN_CONFIG_DEFAULT		= OPEN + 0;
				public const int OPEN_CONFIG_CUSTOM			= OPEN + 1;
				public const int OPEN_CONFIG_TEST_MODE		= OPEN + 2;
				public const int OPEN_LINGO					= OPEN + 3;
				public const int OPEN_PROTO					= OPEN + 4;

				public const int WINDOW						= DEFAULT + 2 * Global.MenuOrderSubSpace;
				public const int WINDOW_PROTO				= WINDOW + 0;
			}

			public static class Display
			{
				private const int DEFAULT					= 3 * Global.MenuOrderMainSpace;

				public const int MANUAL						= DEFAULT + 0 * Global.MenuOrderSubSpace;

				public const int SCENE						= DEFAULT + 1 * Global.MenuOrderSubSpace;

				public const int CUSTOM						= DEFAULT + 2 * Global.MenuOrderSubSpace;
				public const int CUSTOM_EDITOR				= CUSTOM + 0;
				public const int CUSTOM_PLAYER_PREFS		= CUSTOM + 1;
				public const int CUSTOM_GRAPHIC_QUALITY		= CUSTOM + 2;

				public const int TEST						= DEFAULT + 3 * Global.MenuOrderSubSpace;
				public const int TEST_NETWORK				= TEST + 0;
			}
		}

		private static void _OpenFolder(string name,string folderPath)
		{
			if(!KZFileKit.IsFolderExist(folderPath))
			{
				if(!KZEditorKit.DisplayCheck($"Create {name} folder",$"{name} folder does not exist.\nDo you want to create the {name} folder?"))
				{
					return;
				}

				KZFileKit.CreateFolder(folderPath);
			}

			KZEditorKit.OpenFolder(folderPath);
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