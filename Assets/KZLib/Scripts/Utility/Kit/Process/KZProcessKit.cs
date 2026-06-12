using System.Diagnostics;
using System.Threading.Tasks;

/// <summary>
/// Utility methods for launching external console processes and capturing their output.
/// </summary>
public static class KZProcessKit
{
	/// <summary>
	/// Runs a console command and logs stdout to the Editor channel.
	/// Returns false and sets errorLog when stderr is non-empty.
	/// </summary>
	public static bool RunCommandConsole(string command,string argument,out string errorLog)
	{
		errorLog = null;

		using var process = new Process();

		var startInfo = new ProcessStartInfo
		{
			FileName = command,
			Arguments = argument,
			UseShellExecute = false,
			RedirectStandardOutput = true,
			RedirectStandardError = true,
			CreateNoWindow = true,
		};

		process.StartInfo = startInfo;
		process.Start();

		var outputTask = Task.Run(() => process.StandardOutput.ReadToEnd());
		var errorTask = Task.Run(() => process.StandardError.ReadToEnd());

		process.WaitForExit();

		var output = outputTask.Result;
		var error = errorTask.Result;

		var lineArray = output.Split('\n');
		
		for(var i=0;i<lineArray.Length;i++)
		{
			LogChannel.Editor.I($"Output : {lineArray[i]}");
		}

		if(!error.IsEmpty())
		{
			errorLog = error;

			LogChannel.Editor.E($"Error : {error.CP949ToUTF8()}");

			return false;
		}

		return true;
	}
}
