#if UNITY_EDITOR
using KZLib.Data;
using UnityEditor;
using UnityEngine;

namespace KZLib.EditorInternal.Menus
{
	public static partial class KZMenuItem
	{
		[MenuItem("KZMenu/Explorer/Open Route File",false,MenuOrder.Explorer.OPEN)]
		private static void _OnOpenRouteFile()
		{
			var filePath = RouteManager.In.GetRouteFilePath();

			KZEditorKit.Open(filePath);
		}

		[MenuItem("KZMenu/Explorer/Open Documents Folder",false,MenuOrder.Explorer.OPEN)]
		private static void _OnOpenDocumentFolder()
		{
			_OpenFolder("Documents",Global.DocumentFolderPath);
		}

		[MenuItem("KZMenu/Explorer/Open Tools Folder",false,MenuOrder.Explorer.OPEN)]
		private static void _OnOpenToolFolder()
		{
			_OpenFolder("Tools",Global.ToolFolderPath);
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