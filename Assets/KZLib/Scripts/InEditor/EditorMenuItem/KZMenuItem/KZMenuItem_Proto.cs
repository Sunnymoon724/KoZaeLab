#if UNITY_EDITOR
using System.IO;
using KZLib.KZWindow;
using UnityEditor;

namespace KZLib.KZMenu
{
	public partial class KZMenuItem
	{
		[MenuItem("KZMenu/Proto/Open Proto Folder",false,(int) MenuType.Data_Open)]
		private static void _OnOpenProtoFolder()
		{
			CommonUtility.Open(Global.PROTO_FOLDER_PATH);
		}

		[MenuItem("KZMenu/Proto/Open Proto Folder",true,(int) MenuType.Data_Open)]
		private static bool _IsExistProtoFolder()
		{
			return Directory.Exists(Global.PROTO_FOLDER_PATH);
		}

		[MenuItem("KZMenu/Proto/Open Proto Window",false,(int) MenuType.Data_Window)]
		private static void _OnOpenProtoWindow()
		{
			EditorWindow.GetWindow<ProtoWindow>().Show();
		}
	}
}
#endif