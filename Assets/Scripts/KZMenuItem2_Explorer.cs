#if UNITY_EDITOR
using System.Diagnostics;
using System.IO;
using UnityEditor;

namespace KZLib.EditorInternal.Menus
{
	public static partial class KZMenuItem2
	{
		private class MenuOrder
		{
			public class Explorer
			{
				private const int DEFAULT			= 1 * Global.MenuOrderMainSpace;

				public const int LIBRARY			= DEFAULT + 1 * Global.MenuOrderSubSpace;
				public const int VENDER				= DEFAULT + 2 * Global.MenuOrderSubSpace;
			}
		}

		[MenuItem("KZMenu/Explorer/Open Library Project",false,MenuOrder.Explorer.LIBRARY)]
		private static void _OnOpenLibraryProject()
		{
			var solutionPath = Path.Combine(Global.ProjectParentPath,"KoZaeLibrary");

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
			var batchFilePath = Path.Combine(Global.ProjectParentPath,"KoZaeBuilding","BuildLibrary.bat");

			KZEditorKit.OpenBatchFile(batchFilePath);
		}
	}
}
#endif