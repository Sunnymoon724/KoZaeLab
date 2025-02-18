#if UNITY_EDITOR
using System.Diagnostics;
using System.IO;
using UnityEditor;

namespace KZLib.KZMenu
{
	public class KZMenuItem
	{
		private const int c_pivot_order = 10;
		private const int c_menu_line = 20;

		private enum InnerMenuType
		{
			Explorer			= 1 << c_pivot_order,

			Explorer_Etc1		= Explorer+3*c_menu_line,
			Explorer_Etc2		= Explorer+4*c_menu_line,
		}

		[MenuItem("KZMenu/Explorer/Open Library Project",false,(int) InnerMenuType.Explorer_Etc1)]
		private static void OnOpenLibraryProject()
		{
			var solutionPath = Path.Combine(Global.PROJECT_PARENT_PATH,"KoZaeLibrary");

			Process.Start(new ProcessStartInfo
			{
				FileName = "code",
				Arguments = $"\"{solutionPath}\"",
				UseShellExecute = true,
			});
		}

		[MenuItem("KZMenu/Explorer/Build Library Project",false,(int) InnerMenuType.Explorer_Etc2)]
		private static void OnBuildLibraryProject()
		{
			var batchFilePath = Path.Combine(Global.PROJECT_PARENT_PATH,"BuildLibrary.bat");

			Process.Start(new ProcessStartInfo
			{
				FileName = batchFilePath,
				UseShellExecute = true,
			});
		}
	}
}
#endif