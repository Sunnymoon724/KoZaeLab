#if UNITY_EDITOR
using System.IO;
using KZLib.Windows;
using UnityEditor;

namespace KZLib.EditorInternal.Menus
{
	public static partial class KZMenuItem
	{
		[MenuItem("KZMenu/Proto/Generate Current Environment Proto",false,MenuOrder.Data.GENERATE)]
		private static void _OnGenerateCurrentEnvironmentProto()
		{
			var environmentType = CommonUtility.GetCurrentEnvironmentType();

			var batchFilePath = Path.Combine(Global.TOOL_FOLDER_PATH,"GenerateProto",$"GenerateProto_{environmentType}.bat");

			CommonUtility.OpenBatchFile(batchFilePath);
		}
		
		[MenuItem("KZMenu/Proto/Generate All Environment Proto",false,MenuOrder.Data.GENERATE)]
		private static void _OnGenerateAllEnvironmentProto()
		{
			var batchFilePath = Path.Combine(Global.TOOL_FOLDER_PATH,"GenerateProto",$"GenerateProto_All.bat");

			CommonUtility.OpenBatchFile(batchFilePath);
		}

		[MenuItem("KZMenu/Proto/Open Proto Folder",false,MenuOrder.Data.OPEN)]
		private static void _OnOpenProtoFolder()
		{
			_OpenFolder("Proto",Global.PROTO_FOLDER_PATH);
		}

		[MenuItem("KZMenu/Proto/Open Proto Window",false,MenuOrder.Data.WINDOW)]
		private static void _OnOpenProtoWindow()
		{
			EditorWindow.GetWindow<ProtoWindow>().Show();
		}
	}
}
#endif