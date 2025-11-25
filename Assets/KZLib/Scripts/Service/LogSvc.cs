using System.Runtime.CompilerServices;
using System.Collections.Generic;
using KZLib.KZUtility;
using System.Text;
using UnityEngine;
using System;

#if UNITY_EDITOR

using UnityEditor.Callbacks;
using UnityEditorInternal;
using UnityEditor;
using System.Reflection;
using System.Text.RegularExpressions;

#else

using Cysharp.Threading.Tasks;
using KZLib.KZNetwork;

#endif

using Debug = UnityEngine.Debug;

public class LogSvc
{
	public static readonly LogChannel None = new("None");

	public static readonly LogChannel Scene = new("Scene");
	public static readonly LogChannel System = new("System");
	public static readonly LogChannel Build = new("Build");

	public static readonly LogChannel Network = new("Network");
	public static readonly LogChannel Server = new("Server");
	public static readonly LogChannel Client = new("Client");

	public static readonly LogChannel UI = new("UI");
	public static readonly LogChannel FX = new("FX");

	public static readonly LogChannel Game = new("Game");
	public static readonly LogChannel Editor = new("Editor");

	public static readonly LogChannel Test = new("Test");

	public static IEnumerable<MessageData> LogDataGroup => LogChannel.LogDataGroup;
}

public class LogChannel
{
	private const int c_maxLogCount = 100;

	private static readonly HashSet<string> s_logHashSet = new();
	private static readonly CircularQueue<MessageData> m_logDataQueue = new(c_maxLogCount);

	public static IEnumerable<MessageData> LogDataGroup => m_logDataQueue;

	private readonly string m_logTag;

#if !UNITY_EDITOR
		private const int c_coolTimeTimer = 30; // 30s
		private static bool m_sendLock = false;
#endif

	public LogChannel(string logTag)
	{
		m_logTag = logTag;
	}
	
	#region I : Info Log
	public void I(object message,[CallerMemberName] string memberName = null,[CallerFilePath] string filePath = null,[CallerLineNumber] int lineNum = 0)
	{
		_Log(message,LogType.Log,false,memberName,filePath,lineNum);
	}

	public void IOnce(object message,[CallerMemberName] string memberName = null,[CallerFilePath] string filePath = null,[CallerLineNumber] int lineNum = 0)
	{
		_Log(message,LogType.Log,true,memberName,filePath,lineNum);
	}
	#endregion I : Info Log

	#region W : Warning Log
	public void W(object message,[CallerMemberName] string memberName = null,[CallerFilePath] string filePath = null,[CallerLineNumber] int lineNum = 0)
	{
		_Log(message,LogType.Warning,false,memberName,filePath,lineNum);
	}

	public void WOnce(object message,[CallerMemberName] string memberName = null,[CallerFilePath] string filePath = null,[CallerLineNumber] int lineNum = 0)
	{
		_Log(message,LogType.Warning,true,memberName,filePath,lineNum);
	}
	#endregion W : Warning Log
	
	#region E : Error Log
	public void E(object message,[CallerMemberName] string memberName = null,[CallerFilePath] string filePath = null,[CallerLineNumber] int lineNum = 0)
	{
		_Log(message,LogType.Error,false,memberName,filePath,lineNum);
	}

	public void EOnce(object message,[CallerMemberName] string memberName = null,[CallerFilePath] string filePath = null,[CallerLineNumber] int lineNum = 0)
	{
		_Log(message,LogType.Error,true,memberName,filePath,lineNum);
	}
	#endregion E : Error Log

	#region A : Assert Log
	public void A(bool condition,object message,[CallerMemberName] string memberName = null,[CallerFilePath] string filePath = null,[CallerLineNumber] int lineNum = 0)
	{
		if(condition)
		{
			return;
		}

		_Log(message,LogType.Assert,false,memberName,filePath,lineNum);
	}
	#endregion A : Assert Log
	
	private void _Log(object message,LogType logType,bool once,string memberName,string filePath,int lineNum)
	{
		var text = _CreateLog(message,memberName,filePath,lineNum);

		if(once && !s_logHashSet.Add(text))
		{
			return;
		}

#if UNITY_EDITOR || DEBUG
		switch (logType)
		{
			case LogType.Warning:
				Debug.LogWarning(text);
				break;
			case LogType.Error:
			case LogType.Assert:
				Debug.LogError(text);
				break;
			default:
				Debug.Log(text);
				break;
		}
#endif
	}

	private string _CreateLog(object message,string memberName,string filePath,int lineNum)
	{
		var builder = new StringBuilder();

		builder.Append($"[<b>{m_logTag}</b>] {message}");

#if !UNITY_EDITOR
		if(!memberName.IsEmpty() || !filePath.IsEmpty())
		{
			builder.Append(" [");

			if(!memberName.IsEmpty())
			{
				builder.Append($"at {memberName}(..)");
			}

			if(!filePath.IsEmpty())
			{
				var match = s_fileNameExtractor.Match(filePath);

				if(match.Success)
				{
					builder.Append($" - in {match.Groups[1].Value}:{lineNum}");
				}
			}

			builder.Append("]");
		}
#endif

		return builder.ToString();
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void _Initialize()
	{
		Application.logMessageReceived -= _HandleLogMessage;
		Application.logMessageReceived += _HandleLogMessage;
	}
	
	private static void _HandleLogMessage(string condition,string stackTrace,LogType logType)
	{
		var header = $"<{_ConvertType(logType)}> {DateTime.Now:MM/dd HH:mm:ss}";
		var body = string.Empty;

		if(logType == LogType.Exception)
		{
			body = $"{condition}\n\n{stackTrace}";
		}
		else
		{
			var stackTraceArray = stackTrace.Split('\n');
			var index = stackTraceArray.IndexOf(x => x.Contains(nameof(LogChannel)));
			
			if(index == -1)
			{
				body = condition;
			}
			else
			{
				var stack = stackTraceArray[index+1];

				body = $"{condition}\n\n{stack}";
			}
		}

		var message = new MessageData(header,body);

		m_logDataQueue.Enqueue(message);

		Broadcaster.SendEvent(Global.DISPLAY_LOG,message);

#if !UNITY_EDITOR
		if(!m_sendLock && (logType == LogType.Exception))
		{
			_SendBugAsync().Forget();
		}
#endif
	}
	
	private static string _ConvertType(LogType logType)
	{
		return logType switch
		{
			LogType.Warning => "Warning",
			LogType.Error or LogType.Exception or LogType.Assert => "Error",
			_ => "Info",
		};
	}
	
	public static void ClearLogData()
	{
		m_logDataQueue.Clear();
	}

#if !UNITY_EDITOR
	private static async UniTaskVoid _SendBugAsync()
	{
		m_sendLock = true;

		await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);

		var texture = CommonUtility.GetScreenShot();

		await WebRequestManager.In.PostBugReportWebRequestAsync(m_logDataQueue,texture.EncodeToPNG());

		//? Send once and wait for 30 seconds -> If sent too frequently, it can cause a load.

		await UniTask.Delay(TimeSpan.FromSeconds(c_coolTimeTimer));

		m_sendLock = false;
	}
#endif

#if UNITY_EDITOR
	[OnOpenAsset(0)]
	protected static bool _OnOpenDebugLog(int instance,int _)
	{
		var objectName = EditorUtility.InstanceIDToObject(instance).name;

		if(objectName.IsEmpty() || !objectName.IsEqual(nameof(LogSvc)))
		{
			return false;
		}

		var stackTrace = _FindStackTrace();

		if(stackTrace.IsEmpty())
		{
			return false;
		}

		var textMatch = Regex.Match(stackTrace,@"\(at (.+)\)",RegexOptions.IgnoreCase);

		while(textMatch.Success)
		{
			var content = textMatch.Groups[1].Value;

			if(!content.Contains(nameof(LogSvc)))
			{
				break;
			}
			
			textMatch = textMatch.NextMatch();
		}

		if(!textMatch.Success)
		{
			return false;
		}

		var pathArray = textMatch.Groups[1].Value.Split(':');

		if(pathArray.Length < 2 || !int.TryParse(pathArray[1], out int lineNumber))
		{
			return false;
		}

		InternalEditorUtility.OpenFileAtLineExternal(FileUtility.GetAbsolutePath(pathArray[0], true), lineNumber);

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