using UnityEngine;
using KZLib;
using System.Runtime.CompilerServices;

#if UNITY_EDITOR

using UnityEditor.Callbacks;
using UnityEditorInternal;
using UnityEditor;
using System.Reflection;
using System.Text.RegularExpressions;

#endif

public class LogTag : Enumeration
{
	public static readonly LogTag System = new(nameof(System));

	public static readonly LogTag Build = new(nameof(Build));

	public static readonly LogTag Network = new(nameof(Network));

	public static readonly LogTag UI = new(nameof(UI));
	public static readonly LogTag FX = new(nameof(FX));

	public static readonly LogTag Editor = new(nameof(Editor));

	public static readonly LogTag Test = new(nameof(Test));

	public LogTag(string _name) : base(_name) { }
}

/// <summary>
/// Show Only Editor
/// </summary>
public static class LogExtension
{
	#region I : Info Log
	public static void I(this LogTag _log,object _message,[CallerMemberName] string _memberName = null,[CallerFilePath] string _filePath = null,[CallerLineNumber] int _lineNum = 0)
	{
		var text = LogMgr.In.CreateLog(_log,_message,_memberName,_filePath,_lineNum);
#if UNITY_EDITOR
		Debug.Log(text);
#endif
	}
	#endregion I : Info Log

	#region W : Warning Log
	public static void W(this LogTag _log,object _message,[CallerMemberName] string _memberName = null,[CallerFilePath] string _filePath = null,[CallerLineNumber] int _lineNum = 0)
	{
		var text = LogMgr.In.CreateLog(_log,_message,_memberName,_filePath,_lineNum);
#if UNITY_EDITOR
		Debug.LogWarning(text);
#endif
	}
	#endregion W : Warning Log

	#region E : Error Log
	public static void E(this LogTag _log,object _message,[CallerMemberName] string _memberName = null,[CallerFilePath] string _filePath = null,[CallerLineNumber] int _lineNum = 0)
	{
		var text = LogMgr.In.CreateLog(_log,_message,_memberName,_filePath,_lineNum);
#if UNITY_EDITOR
		Debug.LogError(text);
#endif
	}
	#endregion E : Error Log

	#region A : Assert Log
	public static void A(this LogTag _log,bool _condition,object _message,[CallerMemberName] string _memberName = null,[CallerFilePath] string _filePath = null,[CallerLineNumber] int _lineNum = 0)
	{
		if(_condition)
		{
			return;
		}

		var text = LogMgr.In.CreateLog(_log,_message,_memberName,_filePath,_lineNum);
#if UNITY_EDITOR
		Debug.Assert(_condition,_message);
#endif
	}
	#endregion A : Assert Log

#if UNITY_EDITOR
	[OnOpenAsset(0)]
	private static bool OnOpenDebugLog(int _instance,int _)
	{
		var name = EditorUtility.InstanceIDToObject(_instance).name;

		if(name.IsEmpty() || !name.IsEqual(nameof(LogTag)))
		{
			return false;
		}

		var stackTrace = GetStackTrace();

		if(stackTrace.IsEmpty())
		{
			return false;
		}

		var match = Regex.Match(stackTrace,@"\(at (.+)\)",RegexOptions.IgnoreCase);

		if(match.Success)
		{
			match = match.NextMatch();
		}

		if(match.Success)
		{
			var pathArray = match.Groups[1].Value.Split(':');

			if(pathArray.Length < 2 || !int.TryParse(pathArray[1],out int lineNumber))
			{
				return false;
			}

			InternalEditorUtility.OpenFileAtLineExternal(CommonUtility.GetAbsolutePath(pathArray[0],true),lineNumber);

			return true;
		}

		return false;
	}

	private static string GetStackTrace()
	{
		var assembly = Assembly.GetAssembly(typeof(EditorWindow));

		if(assembly == null)
		{
			return null;
		}

		var windowType = assembly.GetType("UnityEditor.ConsoleWindow");

		if(windowType == null)
		{
			return null;
		}

		var windowFieldInfo = windowType.GetField("ms_ConsoleWindow",BindingFlags.Static | BindingFlags.NonPublic);

		if(windowFieldInfo == null)
		{
			return null;
		}

		var window = windowFieldInfo.GetValue(null);

		if(window != (object)EditorWindow.focusedWindow)
		{
			return null;
		}

		var activeFieldInfo = windowType.GetField("m_ActiveText",BindingFlags.Instance | BindingFlags.NonPublic);

		if(activeFieldInfo == null)
		{
			return null;
		}

		return activeFieldInfo.GetValue(window).ToString();
	}
#endif
}