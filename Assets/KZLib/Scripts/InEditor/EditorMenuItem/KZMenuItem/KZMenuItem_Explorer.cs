#if UNITY_EDITOR
using KZLib.KZUtility;
using UnityEditor;

namespace KZLib.KZMenu
{
	public partial class KZMenuItem
	{
		[MenuItem("KZMenu/Explorer/Open Route File",false,MenuOrder.Explorer.OPEN)]
		private static void _OnOpenRouteFile()
		{
			var filePath = RouteMgr.In.FindRouteFilePath();

			CommonUtility.Open(filePath);
		}
	}
}
#endif