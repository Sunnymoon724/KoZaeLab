#if UNITY_EDITOR
using System.Diagnostics;
using System.IO;
using UnityEditor;

namespace KZLib.KZMenu
{
	public class KZMenuItem2
	{
		private class MenuOrder
		{
			private const int MAIN_GAP = 1000000;
			public const int SUB_GAP = 50;

			public class Explorer
			{
				private const int DEFAULT			= +1 * MAIN_GAP;

				public const int LIBRARY			= DEFAULT + 1 * SUB_GAP;
			}
		}

		[MenuItem("KZMenu/Explorer/Open Library Project",false,MenuOrder.Explorer.LIBRARY)]
		private static void _OnOpenLibraryProject()
		{
			var solutionPath = Path.Combine(Global.PROJECT_PARENT_PATH,"KoZaeLibrary");
			
			LogSvc.Build.I(solutionPath);

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