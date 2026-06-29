using System;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using MessagePipe;
using Microsoft.Extensions.DependencyInjection;
using KZLib.Collections.Generic;
using KZLib;
using KZLib.Diagnostics;

#if UNITY_EDITOR

using UnityEditor.Callbacks;
using UnityEditorInternal;
using UnityEditor;
using System.Reflection;

#else

using Cysharp.Threading.Tasks;
using KZLib.Networks;
using KZLib.Webhooks;

#endif

using Debug = UnityEngine.Debug;

/// <summary>
/// Named logging channel used to tag and filter log output by domain (Game, Scene, Network, etc.).
/// Part of a tagged logging hub that enriches messages with caller info, supports once-only deduplication,
/// captures runtime logs into a circular queue, enables editor double-click navigation to source,
/// and posts build-time exception reports via webhook.
/// </summary>
public partial class LogChannel
{
	public static readonly LogChannel None			= new(nameof(None));


	public static readonly LogChannel Game			= new(nameof(Game));
	public static readonly LogChannel Scene			= new(nameof(Scene));
	public static readonly LogChannel Input			= new(nameof(Input));

	public static readonly LogChannel Build			= new(nameof(Build));


	public static readonly LogChannel Webhook		= new(nameof(Webhook));
	public static readonly LogChannel Network		= new(nameof(Network));
	public static readonly LogChannel Server		= new(nameof(Server));
	public static readonly LogChannel Client		= new(nameof(Client));


	public static readonly LogChannel Resource		= new(nameof(Resource));
	public static readonly LogChannel Graphic		= new(nameof(Graphic));
	public static readonly LogChannel Sound			= new(nameof(Sound));
	public static readonly LogChannel Data			= new(nameof(Data));


	public static readonly LogChannel UI			= new(nameof(UI));
	public static readonly LogChannel FX			= new(nameof(FX));


	public static readonly LogChannel Editor		= new(nameof(Editor));

	public static readonly LogChannel Kit			= new(nameof(Kit));

	public static readonly LogChannel Develop		= new(nameof(Develop));
	public static readonly LogChannel Storage		= new(nameof(Storage));

	public static readonly LogChannel Script		= new(nameof(Script));


	public static readonly LogChannel External		= new(nameof(External));


	public static readonly LogChannel Test			= new(nameof(Test));


	internal readonly string m_logTag = string.Empty;

	private LogChannel(string logTag)
	{
		m_logTag = logTag;
	}

	private static readonly HashSet<string> s_logHashSet = new();

	internal static HashSet<string> LogHashSet => s_logHashSet;
}

/// <summary>
/// Extension methods for writing tagged logs through <see cref="LogChannel"/> instances.
/// </summary>
public static class LogChannelExtension
{
	private static readonly Regex s_fileNameExtractor = new(@"([^\\\/]+)(?=\.[^.\\\/]+$)",RegexOptions.Compiled);

	#region I : Info Log
	/// <summary>Writes an info log with caller context.</summary>
	public static void I(this LogChannel channel,object message,[CallerMemberName] string memberName = null,[CallerFilePath] string filePath = null,[CallerLineNumber] int lineNum = 0)
	{
		_Log(channel,message,LogType.Log,false,memberName,filePath,lineNum);
	}

	/// <summary>Writes an info log once per unique message string.</summary>
	public static void IOnce(this LogChannel channel,object message,[CallerMemberName] string memberName = null,[CallerFilePath] string filePath = null,[CallerLineNumber] int lineNum = 0)
	{
		_Log(channel,message,LogType.Log,true,memberName,filePath,lineNum);
	}
	#endregion I : Info Log

	#region W : Warning Log
	/// <summary>Writes a warning log with caller context.</summary>
	public static void W(this LogChannel channel,object message,[CallerMemberName] string memberName = null,[CallerFilePath] string filePath = null,[CallerLineNumber] int lineNum = 0)
	{
		_Log(channel,message,LogType.Warning,false,memberName,filePath,lineNum);
	}

	/// <summary>Writes a warning log once per unique message string.</summary>
	public static void WOnce(this LogChannel channel,object message,[CallerMemberName] string memberName = null,[CallerFilePath] string filePath = null,[CallerLineNumber] int lineNum = 0)
	{
		_Log(channel,message,LogType.Warning,true,memberName,filePath,lineNum);
	}
	#endregion W : Warning Log
	
	#region E : Error Log
	/// <summary>Writes an error log with caller context.</summary>
	public static void E(this LogChannel channel,object message,[CallerMemberName] string memberName = null,[CallerFilePath] string filePath = null,[CallerLineNumber] int lineNum = 0)
	{
		_Log(channel,message,LogType.Error,false,memberName,filePath,lineNum);
	}

	/// <summary>Writes an error log once per unique message string.</summary>
	public static void EOnce(this LogChannel channel,object message,[CallerMemberName] string memberName = null,[CallerFilePath] string filePath = null,[CallerLineNumber] int lineNum = 0)
	{
		_Log(channel,message,LogType.Error,true,memberName,filePath,lineNum);
	}
	#endregion E : Error Log

	#region A : Assert Log
	/// <summary>Logs an assertion failure when <paramref name="condition"/> is false.</summary>
	public static void A(this LogChannel channel,bool condition,object message,[CallerMemberName] string memberName = null,[CallerFilePath] string filePath = null,[CallerLineNumber] int lineNum = 0)
	{
		if(condition)
		{
			return;
		}

		_Log(channel,message,LogType.Assert,false,memberName,filePath,lineNum);
	}

	/// <summary>Logs an assertion failure once per unique message string when <paramref name="condition"/> is false.</summary>
	public static void AOnce(this LogChannel channel,bool condition,object message,[CallerMemberName] string memberName = null,[CallerFilePath] string filePath = null,[CallerLineNumber] int lineNum = 0)
	{
		if(condition)
		{
			return;
		}

		_Log(channel,message,LogType.Assert,true,memberName,filePath,lineNum);
	}
	#endregion A : Assert Log

	private static void _Log(LogChannel channel,object message,LogType logType,bool once,string memberName,string filePath,int lineNum)
	{
		var log = _CreateLog(channel,message,memberName,filePath,lineNum);

		if(once && !LogChannel.LogHashSet.Add(log))
		{
			return;
		}

		if(logType == LogType.Assert)
		{
			Debug.LogError(log);
		}

#if UNITY_EDITOR || DEBUG
		else
		{
			switch(logType)
			{
				case LogType.Warning:
					Debug.LogWarning(log);
					break;
				case LogType.Error:
					Debug.LogError(log);
					break;
				default:
					Debug.Log(log);
					break;
			}
		}
#endif
	}

	private static string _CreateLog(LogChannel logger,object message,string memberName,string filePath,int lineNum)
	{
		var builder = new StringBuilder();

		builder.Append($"[<b>{logger.m_logTag}</b>] {message}");

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

#if UNITY_EDITOR
		// Unity Console double-click navigation expects "(at path:line)" in a separate line.
		if(!filePath.IsEmpty() && lineNum > 0)
		{
			builder.Append("\n(at ").Append(filePath.Replace('\\','/')).Append(':').Append(lineNum).Append(')');
		}
#endif

		return builder.ToString();
	}
}

public partial class LogChannel
{
	private const int c_maxLogCount = 100;

	private static readonly CircularQueue<MessageInfo> s_logMessageInfoQueue = new(c_maxLogCount);

	public static IEnumerable<MessageInfo> LogMessageInfoGroup => s_logMessageInfoQueue;

#if !UNITY_EDITOR
		private const int c_coolTimeTimer = 30; // 30s
		private static bool m_sendLock = false;
#endif

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void _Initialize()
	{
		var builder = new ServiceCollection();
		builder.AddMessagePipe();

		var provider = builder.BuildServiceProvider();

		GlobalMessagePipe.SetProvider(provider);

		Application.logMessageReceived -= _HandleLogMessage;
		Application.logMessageReceived += _HandleLogMessage;

		LogBridge.OnInfo = _SendInfo;
		LogBridge.OnWarning = _SendWarning;
		LogBridge.OnError = _SendError;
	}

	private static void _SendInfo(string message)
	{
		External.I(message);
	}

	private static void _SendWarning(string message)
	{
		External.W(message);
	}

	private static void _SendError(string message)
	{
		External.E(message);
	}

	/// <summary>
	/// Captures Unity console output, trims LogChannel frames from the stack trace,
	/// and publishes entries to the debug overlay queue.
	/// </summary>
	private static void _HandleLogMessage(string condition,string stackTrace,LogType logType)
	{
		var currentTime = ServerClockManager.In.GetNow(true);
		var header = $"<{_ConvertType(logType)}> {currentTime:MM/dd HH:mm:ss}";
		var body = string.Empty;

		if(logType == LogType.Exception)
		{
			body = $"{condition}\n\n{stackTrace}";
		}
		else
		{
			var stackTraceArray = stackTrace.Split('\n');

			static bool _FindIndex(string stackTrace)
			{
				return stackTrace.Contains(nameof(LogChannel));
			}

			var index = stackTraceArray.IndexOf(_FindIndex);

			if(index == Global.InvalidIndex)
			{
				body = condition;
			}
			else if(index + 1 < stackTraceArray.Length)
			{
				var stack = stackTraceArray[index+1];

				body = $"{condition}\n\n{stack}";
			}
			else
			{
				body = condition;
			}
		}

		var message = new MessageInfo(header,body);

		s_logMessageInfoQueue.Enqueue(message);

		GlobalMessagePipe.GetPublisher<CommonNoticeTag,MessageInfo>().Publish(CommonNoticeTag.DisplayLog,message);

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

	public static void ClearLogMessageInfo()
	{
		s_logMessageInfoQueue.Clear();
	}

#if !UNITY_EDITOR
	private static async UniTaskVoid _SendBugAsync()
	{
		m_sendLock = true;

		await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate).SuppressCancellationThrow();

		var texture = CommonUtility.GetScreenShot();

		await WebhookManager.In.PostBugReportWebRequestAsync(m_logMessageInfoQueue,texture.EncodeToPNG());

		//? Send once and wait for 30 seconds -> If sent too frequently, it can cause a load.
		await UniTask.Delay(TimeSpan.FromSeconds(c_coolTimeTimer)).SuppressCancellationThrow();

		m_sendLock = false;
	}
#endif
}

public partial class LogChannel
{
#if UNITY_EDITOR
	private static Type s_consoleWindowType = null;
	private static FieldInfo s_consoleWindowFieldInfo = null;
	private static FieldInfo[] s_activeTextFieldInfoGroup = null;

	private static readonly Regex s_stackFrameExtractor = new(@"\(at (.+)\)",RegexOptions.Compiled | RegexOptions.IgnoreCase);

	/// <summary>
	/// Opens the caller source file when a LogChannel log entry is double-clicked in the Unity Console.
	/// </summary>
	[OnOpenAsset(0)]
	internal static bool _OnOpenDebugLog(int instance,int _)
	{
		var entity = EditorUtility.EntityIdToObject(instance);

		if(entity == null || entity.name.IsEmpty() || !string.Equals(entity.name,nameof(LogChannel)))
		{
			return false;
		}

		var stackTrace = _FindConsoleStackTrace();

		if(stackTrace.IsEmpty())
		{
			return false;
		}

		return _TryOpenCallerSource(stackTrace);
	}

	// Skips LogChannel frames and opens the first external caller location found in the stack trace.
	private static bool _TryOpenCallerSource(string stackTrace)
	{
		var textMatch = s_stackFrameExtractor.Match(stackTrace);

		while(textMatch.Success)
		{
			var content = textMatch.Groups[1].Value;

			if(!_IsLogChannelFrame(content) && _TryParseSourceLocation(content,out var filePath,out var lineNumber))
			{
				var absolutePath = KZFileKit.GetAbsolutePath(filePath,true);

				if(!absolutePath.IsEmpty())
				{
					InternalEditorUtility.OpenFileAtLineExternal(absolutePath,lineNumber);

					return true;
				}
			}

			textMatch = textMatch.NextMatch();
		}

		return false;
	}

	private static bool _TryParseSourceLocation(string content,out string filePath,out int lineNumber)
	{
		filePath = null;
		lineNumber = 0;

		if(content.IsEmpty())
		{
			return false;
		}

		var lastColonIndex = content.LastIndexOf(':');

		if(lastColonIndex <= 0 || lastColonIndex >= content.Length - 1)
		{
			return false;
		}

		if(!int.TryParse(content[(lastColonIndex + 1)..],out lineNumber) || lineNumber <= 0)
		{
			return false;
		}

		filePath = content[..lastColonIndex];

		return !filePath.IsEmpty();
	}

	private static bool _IsLogChannelFrame(string content)
	{
		return content.Contains(nameof(LogChannel)) || content.Contains(nameof(LogChannelExtension));
	}

	// Caches ConsoleWindow reflection fields because Unity does not expose them publicly.
	private static void _EnsureConsoleReflection()
	{
		if(s_consoleWindowType != null)
		{
			return;
		}

		var assembly = Assembly.GetAssembly(typeof(EditorWindow));

		if(assembly == null)
		{
			return;
		}

		s_consoleWindowType = assembly.GetType("UnityEditor.ConsoleWindow");

		if(s_consoleWindowType == null)
		{
			return;
		}

		s_consoleWindowFieldInfo = s_consoleWindowType.GetField("ms_ConsoleWindow",BindingFlags.Static | BindingFlags.NonPublic);

		var activeTextFieldNameGroup = new[] { "m_ActiveText","m_ActiveMessage" };
		var fieldInfoList = new List<FieldInfo>(activeTextFieldNameGroup.Length);

		foreach(var fieldName in activeTextFieldNameGroup)
		{
			var fieldInfo = s_consoleWindowType.GetField(fieldName,BindingFlags.Instance | BindingFlags.NonPublic);

			if(fieldInfo != null)
			{
				fieldInfoList.Add(fieldInfo);
			}
		}

		s_activeTextFieldInfoGroup = fieldInfoList.ToArray();
	}

	private static string _FindConsoleStackTrace()
	{
		_EnsureConsoleReflection();

		if(s_consoleWindowType == null || s_consoleWindowFieldInfo == null || s_activeTextFieldInfoGroup.IsNullOrEmpty())
		{
			return null;
		}

		var window = s_consoleWindowFieldInfo.GetValue(null);

		if(window == null)
		{
			return null;
		}

		foreach(var fieldInfo in s_activeTextFieldInfoGroup)
		{
			if(fieldInfo.GetValue(window) is string text && !text.IsEmpty())
			{
				return text;
			}
		}

		return null;
	}
#endif
}
