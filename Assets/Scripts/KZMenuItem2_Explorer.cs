#if UNITY_EDITOR
using System.Diagnostics;
using System.IO;
using UnityEditor;

namespace KZLib.KZMenu
{
	public static partial class KZMenuItem2
	{
		private class MenuOrder
		{
			public class Explorer
			{
				private const int DEFAULT			= 1 * Global.MENU_ORDER_MAIN_SPACE;

				public const int LIBRARY			= DEFAULT + 1 * Global.MENU_ORDER_SUB_SPACE;
				public const int VENDER				= DEFAULT + 2 * Global.MENU_ORDER_SUB_SPACE;
			}
		}

		[MenuItem("KZMenu/Explorer/Open Library Project",false,MenuOrder.Explorer.LIBRARY)]
		private static void _OnOpenLibraryProject()
		{
			var solutionPath = Path.Combine(Global.PROJECT_PARENT_PATH,"KoZaeLibrary");

			Process.Start(new ProcessStartInfo
			{
				FileName = "code",
				Arguments = $"\"{solutionPath}\"",
				UseShellExecute = true,
			});
		}

		[MenuItem("KZMenu/Explorer/Build Library Project",false,MenuOrder.Explorer.LIBRARY+1)]
		private static void _OnBuildLibraryProject()
		{
			var batchFilePath = Path.Combine(Global.PROJECT_PARENT_PATH,"KoZaeBuilding","BuildLibrary.bat");

			CommonUtility.OpenBatchFile(batchFilePath);
		}
	}
}
#endif