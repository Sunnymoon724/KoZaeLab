using System.Diagnostics;

public static partial class CommonUtility
{
	public static bool RunCommandConsole(string _command,string _argument,out string _errorLog)
	{
		_errorLog = null;

		using var process = new Process();

		var startInfo = new ProcessStartInfo
		{
			FileName = _command,
			Arguments = _argument,
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
			LogTag.Editor.I($"Output : {line}");
		}

		if(!error.IsEmpty())
		{
			_errorLog = error;

			LogTag.Editor.E($"Error : {error.CP949ToUTF8()}");

			return false;
		}

		return true;
	}
}