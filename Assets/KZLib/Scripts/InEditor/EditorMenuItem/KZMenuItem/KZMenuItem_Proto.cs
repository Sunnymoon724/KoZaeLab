#if UNITY_EDITOR
using KZLib.KZWindow;
using UnityEditor;

namespace KZLib.KZMenu
{
	public partial class KZMenuItem
	{
		[MenuItem("KZMenu/Proto/Open Proto Folder",false,MenuOrder.Data.GENERATE)]
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