#if UNITY_EDITOR
using System.IO;
using UnityEditor;

namespace KZLib.KZMenu
{
	public partial class KZMenuItem
	{
		[MenuItem("KZMenu/Proto/Open Proto Folder",false,(int) MenuType.Proto_Open)]
		private static void OnOpenProtoFolder()
		{
			CommonUtility.Open(Global.PROTO_FOLDER_PATH);
		}

		[MenuItem("KZMenu/Proto/Open Proto Folder",true,(int) MenuType.Config_Open)]
		private static bool IsExistProtoFolder()
		{
			return Directory.Exists(Global.PROTO_FOLDER_PATH);
		}
	}
}
#endif