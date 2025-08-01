#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace KZLib.KZMenu
{
	public partial class KZMenuItem
	{
		[MenuItem("KZMenu/Explorer/Open Route File",false,MenuOrder.Explorer.OPEN)]
		private static void _OnOpenRouteFile()
		{
			var filePath = RouteMgr.In.GetRouteFilePath();

			CommonUtility.Open(filePath);
		}

		[MenuItem("KZMenu/Explorer/Open Documents Folder",false,MenuOrder.Explorer.OPEN)]
		private static void _OnOpenDocumentFolder()
		{
			_OpenFolder("Documents",Global.DOCUMENT_FOLDER_PATH);
		}

		[MenuItem("KZMenu/Explorer/Open Tools Folder",false,MenuOrder.Explorer.OPEN)]
		private static void _OnOpenToolFolder()
		{
			_OpenFolder("Tools",Global.TOOL_FOLDER_PATH);
		}

		[MenuItem("KZMenu/Explorer/Open DataPath Folder",false,MenuOrder.Explorer.OPEN)]
		private static void _OnOpenDataPathFolder()
		{
			_OpenFolder("DataPath",Application.dataPath);
		}

		[MenuItem("KZMenu/Explorer/Open PersistentDataPath Folder",false,MenuOrder.Explorer.OPEN)]
		private static void _OnOpenPersistentDataPathFolder()
		{
			_OpenFolder("PersistentDataPath",Application.persistentDataPath);
		}
	}
}
#endif