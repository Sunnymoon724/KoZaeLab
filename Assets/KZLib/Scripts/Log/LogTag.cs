using UnityEngine;
using KZLib;
using KZLib.KZUtility;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Collections.Generic;

using Debug = UnityEngine.Debug;

#if UNITY_EDITOR

using UnityEditor.Callbacks;
using UnityEditorInternal;
using UnityEditor;
using System.Reflection;
using System.Text.RegularExpressions;

#endif

public class LogTag : CustomTag
{
	public static readonly LogTag System	=	new(nameof(System));

	public static readonly LogTag Build		=	new(nameof(Build));

	public static readonly LogTag Security	=	new(nameof(Security));
	public static readonly LogTag Network	=	new(nameof(Network));

	public static readonly LogTag UI		=	new(nameof(UI));
	public static readonly LogTag FX		=	new(nameof(FX));

	public static readonly LogTag Editor	=	new(nameof(Editor));

	public static readonly LogTag Test		=	new(nameof(Test));

	public LogTag(string name) : base(name) { }
}

/// <summary>
/// Show Only Editor
/// </summary>
public static class LogExtension
{
	private static readonly HashSet<string> s_logHashSet = new();

	#region I : Info Log
	public static void I(this LogTag log,object message,[CallerMemberName] string memberName = null,[CallerFilePath] string filePath = null,[CallerLineNumber] int lineNum = 0)
	{
		var text = LogMgr.In.CreateLog(log,message,memberName,filePath,lineNum);
#if UNITY_EDITOR
		Debug.Log(text);
#endif
	}

	public static void IOnce(this LogTag log,object message,[CallerMemberName] string memberName = null,[CallerFilePath] string filePath = null,[CallerLineNumber] int lineNum = 0)
	{
		if(CheckLogAtOnce())
		{
			log.I(message,memberName,filePath,lineNum);
		}
	}
	#endregion I : Info Log

	#region W : Warning Log
	public static void W(this LogTag log,object message,[CallerMemberName] string memberName = null,[CallerFilePath] string filePath = null,[CallerLineNumber] int lineNum = 0)
	{
		var text = LogMgr.In.CreateLog(log,message,memberName,filePath,lineNum);
#if UNITY_EDITOR
		Debug.LogWarning(text);
#endif
	}

	public static void WOnce(this LogTag log,object message,[CallerMemberName] string memberName = null,[CallerFilePath] string filePath = null,[CallerLineNumber] int lineNum = 0)
	{
		if(CheckLogAtOnce())
		{
			log.W(message,memberName,filePath,lineNum);
		}
	}
	#endregion W : Warning Log

	#region E : Error Log
	public static void E(this LogTag log,object message,[CallerMemberName] string memberName = null,[CallerFilePath] string filePath = null,[CallerLineNumber] int lineNum = 0)
	{
		var text = LogMgr.In.CreateLog(log,message,memberName,filePath,lineNum);
#if UNITY_EDITOR
		Debug.LogError(text);
#endif
	}

	public static void EOnce(this LogTag log,object message,[CallerMemberName] string memberName = null,[CallerFilePath] string filePath = null,[CallerLineNumber] int lineNum = 0)
	{
		if(CheckLogAtOnce())
		{
			log.E(message,memberName,filePath,lineNum);
		}
	}
	#endregion E : Error Log

	#region A : Assert Log
	public static void A(this LogTag log,bool condition,object message,[CallerMemberName] string memberName = null,[CallerFilePath] string filePath = null,[CallerLineNumber] int lineNum = 0)
	{
		if(condition)
		{
			return;
		}

		var text = LogMgr.In.CreateLog(log,message,memberName,filePath,lineNum);
#if UNITY_EDITOR
		Debug.Assert(condition,message);
#endif
	}
	#endregion A : Assert Log

#if UNITY_EDITOR
	[OnOpenAsset(0)]
	private static bool OnOpenDebugLog(int instance,int _)
	{
		var objectName = EditorUtility.InstanceIDToObject(instance).name;

		if(objectName.IsEmpty() || !objectName.IsEqual(nameof(LogTag)))
		{
			return false;
		}

		var stackTrace = FindStackTrace();

		if(stackTrace.IsEmpty())
		{
			return false;
		}

		var textMatch = Regex.Match(stackTrace,@"\(at (.+)\)",RegexOptions.IgnoreCase);

		if(textMatch.Success)
		{
			textMatch = textMatch.NextMatch();
		}

		if(textMatch.Success)
		{
			var pathArray = textMatch.Groups[1].Value.Split(':');

			if(pathArray.Length < 2 || !int.TryParse(pathArray[1],out int lineNumber))
			{
				return false;
			}

			InternalEditorUtility.OpenFileAtLineExternal(CommonUtility.GetAbsolutePath(pathArray[0],true),lineNumber);

			return true;
		}

		return false;
	}

	private static bool CheckLogAtOnce()
	{
		var stackFrame = new StackFrame(2,true);
		var logInfo = $"{stackFrame.GetFileName()}:{stackFrame.GetFileLineNumber()}";

		if(s_logHashSet.Contains(logInfo))
		{
			return false;
		}

		s_logHashSet.Add(logInfo);

		return true;
	}

	private static string FindStackTrace()
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