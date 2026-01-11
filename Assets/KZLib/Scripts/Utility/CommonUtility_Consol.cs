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