using KZLib;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

using Debug = UnityEngine.Debug;
using KZLib.KZUtility;

#if UNITY_EDITOR

using UnityEditor.Callbacks;
using UnityEditorInternal;
using UnityEditor;
using System.Reflection;
using System.Text.RegularExpressions;

#endif

/// <summary>
/// LogType은 이미 존재하므로 그냥 KZLogType로
/// </summary>
public enum KZLogType
{
	None,
	System,
	Build,

	Network,
	Server,
	Client,

	UI,
	FX,

	Game,
	Editor,

	Test,
}

/// <summary>
/// Show Only Editor
/// </summary>
public static class KZLogTypeExtension
{
	private static readonly HashSet<string> s_logHashSet = new();

	#region I : Info Log
	public static void I(this KZLogType type,object message,[CallerMemberName] string memberName = null,[CallerFilePath] string filePath = null,[CallerLineNumber] int lineNum = 0)
	{
		var text = LogMgr.In.CreateLog(type,message,memberName,filePath,lineNum);

#if UNITY_EDITOR
		Debug.Log(text);
#endif
	}

	public static void IOnce(this KZLogType type,object message,[CallerMemberName] string memberName = null,[CallerFilePath] string filePath = null,[CallerLineNumber] int lineNum = 0)
	{
		var text = LogMgr.In.CreateLog(type,message,memberName,filePath,lineNum);

#if UNITY_EDITOR
		if(_CheckLogAtOnce(text))
		{
			Debug.Log(text);
		}
#endif
	}
	#endregion I : Info Log

	#region W : Warning Log
	public static void W(this KZLogType type,object message,[CallerMemberName] string memberName = null,[CallerFilePath] string filePath = null,[CallerLineNumber] int lineNum = 0)
	{
		var text = LogMgr.In.CreateLog(type,message,memberName,filePath,lineNum);

#if UNITY_EDITOR
		Debug.LogWarning(text);
#endif
	}

	public static void WOnce(this KZLogType type,object message,[CallerMemberName] string memberName = null,[CallerFilePath] string filePath = null,[CallerLineNumber] int lineNum = 0)
	{
		var text = LogMgr.In.CreateLog(type,message,memberName,filePath,lineNum);

#if UNITY_EDITOR
		if(_CheckLogAtOnce(text))
		{
			Debug.LogWarning(text);
		}
#endif
	}
	#endregion W : Warning Log

	#region E : Error Log
	public static void E(this KZLogType type,object message,[CallerMemberName] string memberName = null,[CallerFilePath] string filePath = null,[CallerLineNumber] int lineNum = 0)
	{
		var text = LogMgr.In.CreateLog(type,message,memberName,filePath,lineNum);

#if UNITY_EDITOR
		Debug.LogError(text);
#endif
	}

	public static void EOnce(this KZLogType type,object message,[CallerMemberName] string memberName = null,[CallerFilePath] string filePath = null,[CallerLineNumber] int lineNum = 0)
	{
		var text = LogMgr.In.CreateLog(type,message,memberName,filePath,lineNum);

#if UNITY_EDITOR
		if(_CheckLogAtOnce(text))
		{
			Debug.LogError(text);
		}
#endif
	}
	#endregion E : Error Log

	#region A : Assert Log
	public static void A(this KZLogType type,bool condition,object message,[CallerMemberName] string memberName = null,[CallerFilePath] string filePath = null,[CallerLineNumber] int lineNum = 0)
	{
		if(condition)
		{
			return;
		}

		var text = LogMgr.In.CreateLog(type,message,memberName,filePath,lineNum);

#if UNITY_EDITOR
		Debug.Assert(condition,text);
#endif
	}
	#endregion A : Assert Log

#if UNITY_EDITOR
	[OnOpenAsset(0)]
	public static bool OnOpenDebugLog(int instance,int _)
	{
		var objectName = EditorUtility.InstanceIDToObject(instance).name;

		// Enum.IsDefined(typeof(KZLogType),objectName);

		if(objectName.IsEmpty() || !objectName.IsEqual(nameof(KZLogType)))
		{
			return false;
		}

		var stackTrace = _FindStackTrace();

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

			InternalEditorUtility.OpenFileAtLineExternal(FileUtility.GetAbsolutePath(pathArray[0],true),lineNumber);

			return true;
		}

		return false;
	}

	private static bool _CheckLogAtOnce(string log)
	{
		if(s_logHashSet.Contains(log))
		{
			return false;
		}

		s_logHashSet.Add(log);

		return true;
	}

	private static string _FindStackTrace()
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