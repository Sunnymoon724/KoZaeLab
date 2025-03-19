#if UNITY_EDITOR
using KZLib.KZUtility;
using UnityEditor;

namespace KZLib.KZMenu
{
	public partial class KZMenuItem
	{
		[MenuItem("KZMenu/Explorer/Open Route File",false,(int) MenuType.Explorer_Open)]
		private static void OnOpenRouteFile()
		{
			var filePath = RouteMgr.In.FindRouteFilePath();

			CommonUtility.Open(filePath);
		}
	}
}
#endif