#if UNITY_EDITOR
using UnityEditor;

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
	}
}
#endif