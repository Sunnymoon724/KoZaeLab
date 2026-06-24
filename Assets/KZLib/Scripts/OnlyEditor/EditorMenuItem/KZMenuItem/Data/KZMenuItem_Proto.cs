#if UNITY_EDITOR
using System.IO;
using KZLib.Windows;
using UnityEditor;

namespace KZLib.EditorInternal.Menus
{
	public static partial class KZMenuItem
	{
		[MenuItem("KZMenu/Proto/Generate Current Environment Proto",false,MenuOrder.Data.GENERATE_PROTO_CURRENT)]
		private static void _OnGenerateCurrentEnvironmentProto()
		{
			var environmentType = KZGameKit.GetCurrentEnvironmentType();

			var batchFilePath = Path.Combine(Global.ToolFolderPath,"GenerateProto",$"GenerateProto_{environmentType}.bat");

			KZEditorKit.OpenBatchFile(batchFilePath);
		}
		
		[MenuItem("KZMenu/Proto/Generate All Environment Proto",false,MenuOrder.Data.GENERATE_PROTO_ALL)]
		private static void _OnGenerateAllEnvironmentProto()
		{
			var batchFilePath = Path.Combine(Global.ToolFolderPath,"GenerateProto",$"GenerateProto_All.bat");

			KZEditorKit.OpenBatchFile(batchFilePath);
		}

		[MenuItem("KZMenu/Proto/Open Proto Folder",false,MenuOrder.Data.OPEN_PROTO)]
		private static void _OnOpenProtoFolder()
		{
			_OpenFolder("Proto",Global.ProtoFolderPath);
		}

		[MenuItem("KZMenu/Proto/Open Proto Window",false,MenuOrder.Data.WINDOW_PROTO)]
		private static void _OnOpenProtoWindow()
		{
			EditorWindow.GetWindow<ProtoWindow>().Show();
		}
	}
}
#endif