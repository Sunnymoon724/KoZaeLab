using System.Diagnostics;

public static partial class CommonUtility
{
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

		var output = process.StandardOutput.ReadToEnd();
		var error = process.StandardError.ReadToEnd();

		process.WaitForExit();

		foreach(var line in output.Split('\n'))
		{
			Logger.Editor.I($"Output : {line}");
		}

		if(!error.IsEmpty())
		{
			errorLog = error;

			Logger.Editor.E($"Error : {error.CP949ToUTF8()}");

			return false;
		}

		return true;
	}
}