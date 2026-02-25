using System.Runtime.CompilerServices;
using System.Collections.Generic;
using KZLib.Utilities;
using System.Text;
using UnityEngine;
using MessagePipe;
using Microsoft.Extensions.DependencyInjection;
using KZLib;
using KZLib.Collections.Generic;

#if UNITY_EDITOR

using UnityEditor.Callbacks;
using UnityEditorInternal;
using UnityEditor;
using System.Reflection;
using System.Text.RegularExpressions;

#else

using Cysharp.Threading.Tasks;
using KZLib.Networking;

#endif

using Debug = UnityEngine.Debug;

public partial class LogChannel
{
	public static readonly LogChannel None = new(nameof(None));


	public static readonly LogChannel Scene = new(nameof(Scene));
	public static readonly LogChannel System = new(nameof(System));
	public static readonly LogChannel Build = new(nameof(Build));


	public static readonly LogChannel Network = new(nameof(Network));
	public static readonly LogChannel Server = new(nameof(Server));
	public static readonly LogChannel Client = new(nameof(Client));


	public static readonly LogChannel UI = new(nameof(UI));
	public static readonly LogChannel FX = new(nameof(FX));

	public static readonly LogChannel Game = new(nameof(Game));
	public static readonly LogChannel Editor = new(nameof(Editor));


	public static readonly LogChannel External = new(nameof(External));

	public static readonly LogChannel Test = new(nameof(Test));

	internal readonly string m_logTag;

	private LogChannel(string logTag)
	{
		m_logTag = logTag;
	}

	private static readonly HashSet<string> s_logHashSet = new();

	internal static HashSet<string> LogHashSet => s_logHashSet;
}

public static class LogChannelExtension
{
#if !UNITY_EDITOR
	private static readonly Regex s_fileNameExtractor = new(@"([^\\\/]+)(?=\.[^.\\\/]+$)",RegexOptions.Compiled);
#endif

	#region I : Info Log
	public static void I(this LogChannel channel,object message,[CallerMemberName] string memberName = null,[CallerFilePath] string filePath = null,[CallerLineNumber] int lineNum = 0)
	{
		_Log(channel,message,LogType.Log,false,memberName,filePath,lineNum);
	}

	public static void IOnce(this LogChannel channel,object message,[CallerMemberName] string memberName = null,[CallerFilePath] string filePath = null,[CallerLineNumber] int lineNum = 0)
	{
		_Log(channel,message,LogType.Log,true,memberName,filePath,lineNum);
	}
	#endregion I : Info Log

	#region W : Warning Log
	public static void W(this LogChannel channel,object message,[CallerMemberName] string memberName = null,[CallerFilePath] string filePath = null,[CallerLineNumber] int lineNum = 0)
	{
		_Log(channel,message,LogType.Warning,false,memberName,filePath,lineNum);
	}

	public static void WOnce(this LogChannel channel,object message,[CallerMemberName] string memberName = null,[CallerFilePath] string filePath = null,[CallerLineNumber] int lineNum = 0)
	{
		_Log(channel,message,LogType.Warning,true,memberName,filePath,lineNum);
	}
	#endregion W : Warning Log
	
	#region E : Error Log
	public static void E(this LogChannel channel,object message,[CallerMemberName] string memberName = null,[CallerFilePath] string filePath = null,[CallerLineNumber] int lineNum = 0)
	{
		_Log(channel,message,LogType.Error,false,memberName,filePath,lineNum);
	}

	public static void EOnce(this LogChannel channel,object message,[CallerMemberName] string memberName = null,[CallerFilePath] string filePath = null,[CallerLineNumber] int lineNum = 0)
	{
		_Log(channel,message,LogType.Error,true,memberName,filePath,lineNum);
	}
	#endregion E : Error Log

	#region A : Assert Log
	public static void A(this LogChannel channel,bool condition,object message,[CallerMemberName] string memberName = null,[CallerFilePath] string filePath = null,[CallerLineNumber] int lineNum = 0)
	{
		if(condition)
		{
			return;
		}

		_Log(channel,message,LogType.Assert,false,memberName,filePath,lineNum);
	}
	#endregion A : Assert Log

	private static void _Log(LogChannel channel,object message,LogType logType,bool once,string memberName,string filePath,int lineNum)
	{
		var log = _CreateLog(channel,message,memberName,filePath,lineNum);

		if(once && !LogChannel.LogHashSet.Add(log))
		{
			return;
		}

#if UNITY_EDITOR || DEBUG
		switch(logType)
		{
			case LogType.Warning:
				Debug.LogWarning(log);
				break;
			case LogType.Error:
			case LogType.Assert:
				Debug.LogError(log);
				break;
			default:
				Debug.Log(log);
				break;
		}
#endif
	}

	private static string _CreateLog(LogChannel logger,object message,string memberName,string filePath,int lineNum)
	{
		var builder = new StringBuilder();

		builder.Append($"[<b>{logger.m_logTag}</b>] {message}");

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
	}

	private static void _HandleLogMessage(string condition,string stackTrace,LogType logType)
	{
		var currentTime = GameTimeManager.In.GetCurrentTime(true);
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

			if(index == Global.INVALID_INDEX)
			{
				body = condition;
			}
			else
			{
				var stack = stackTraceArray[index+1];

				body = $"{condition}\n\n{stack}";
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

		await WebRequestManager.In.PostBugReportWebRequestAsync(m_logMessageInfoQueue,texture.EncodeToPNG());

		//? Send once and wait for 30 seconds -> If sent too frequently, it can cause a load.
		await UniTask.Delay(TimeSpan.FromSeconds(c_coolTimeTimer)).SuppressCancellationThrow();

		m_sendLock = false;
	}
#endif
}

public partial class LogChannel
{
#if UNITY_EDITOR
	[OnOpenAsset(0)]
	internal static bool _OnOpenDebugLog(int instance,int _)
	{
		var objectName = EditorUtility.EntityIdToObject(instance).name;

		if(objectName.IsEmpty() || !objectName.IsEqual(nameof(LogChannel)))
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

			if(!_IsLogChannelFrame(content))
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

		InternalEditorUtility.OpenFileAtLineExternal(KZFileKit.GetAbsolutePath(pathArray[0], true), lineNumber);

		return true;
	}

	private static bool _IsLogChannelFrame(string content)
	{
		return content.Contains(nameof(LogChannel)) || content.Contains(nameof(LogChannelExtension));
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