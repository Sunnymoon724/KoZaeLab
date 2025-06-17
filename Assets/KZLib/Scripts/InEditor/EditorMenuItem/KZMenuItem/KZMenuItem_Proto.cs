#if UNITY_EDITOR
using System.Diagnostics;
using System.IO;
using KZLib.KZWindow;
using UnityEditor;

namespace KZLib.KZMenu
{
	public partial class KZMenuItem
	{
		[MenuItem("KZMenu/Proto/Generate DEV Proto",false,MenuOrder.Data.GENERATE)]
		private static void _OnGenerateDEVProto()
		{
			_OnGenerateProto(EnvironmentType.DEV);
		}

		[MenuItem("KZMenu/Proto/Generate QA Proto",false,MenuOrder.Data.GENERATE)]
		private static void _OnGenerateQAProto()
		{
			_OnGenerateProto(EnvironmentType.QA);
		}

		[MenuItem("KZMenu/Proto/Generate LIVE Proto",false,MenuOrder.Data.GENERATE)]
		private static void _OnGenerateLIVEProto()
		{
			_OnGenerateProto(EnvironmentType.LIVE);
		}

		private static void _OnGenerateProto(EnvironmentType environmentType)
		{
			var batchFilePath = Path.Combine(Global.TOOL_FOLDER_PATH,"GenerateProto",$"GenerateProto_{environmentType}.bat");

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