using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Text.RegularExpressions;
using KZLib.KZUtility;
using UnityEngine.Events;

#if !UNITY_EDITOR

using Cysharp.Threading.Tasks;
using KZLib.KZNetwork;

#endif

namespace KZLib
{
	public class LogMgr : Singleton<LogMgr>
	{
		private bool m_disposed = false;

		private static readonly Regex s_fileNameExtractor = new(@"([^\\\/]+)(?=\.[^.\\\/]+$)",RegexOptions.Compiled);

		private const int c_max_log_count = 100;

		private readonly CircularQueue<MessageData> m_logDataQueue = new(c_max_log_count);

		public IEnumerable<MessageData> LogDataGroup => m_logDataQueue;

#if !UNITY_EDITOR
		private const int c_coolTimeTimer = 30; // 30s
		private bool m_sendLock = false;
#endif

		public event Action<MessageData> OnLogDisplay = null;

		protected override void Initialize()
		{
			Application.logMessageReceived += _HandleLogMessage;
		}

		protected override void Release(bool disposing)
		{
			if(m_disposed)
			{
				return;
			}

			if(disposing)
			{
				Application.logMessageReceived -= _HandleLogMessage;

				m_logDataQueue.Clear();
			}

			m_disposed = true;

			base.Release(disposing);
		}

		public string CreateLog(KZLogType type,object message,string memberName,string filePath,int lineNum)
		{
			var builder = new StringBuilder();

			builder.Append($"[{type}] {message}");

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

			return builder.ToString();
		}

		private void _HandleLogMessage(string condition,string stackTrace,LogType logType)
		{
			var header = $"<{_GetKZLogType(logType)}> {DateTime.Now:MM/dd HH:mm:ss}";
			var body = string.Empty;

			if(logType == LogType.Exception)
			{
				body = $"{condition}\n\n{stackTrace}";
			}
			else
			{
				var stackTraceArray = stackTrace.Split('\n');
				var index = stackTraceArray.IndexOf(x => x.Contains(nameof(KZLogType)));
				var stack = stackTraceArray[index+1];

				body = $"{condition}\n\n{stack}";
			}

			var message = new MessageData(header,body);

			m_logDataQueue.Enqueue(message);
			OnLogDisplay?.Invoke(message);

#if !UNITY_EDITOR
			if(!m_sendLock && (logType == LogType.Exception))
			{
				_SendBugAsync().Forget();
			}
#endif
		}

		private string _GetKZLogType(LogType logType)
		{
			return logType switch
			{
				LogType.Warning => "Warning",
				LogType.Error or LogType.Exception or LogType.Assert => "Error",
				_ => "Info",
			};
		}

		public void ClearLogData()
		{
			m_logDataQueue.Clear();
		}

#if !UNITY_EDITOR
		private async UniTaskVoid _SendBugAsync()
		{
			m_sendLock = true;

			await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);

			var texture = CommonUtility.GetScreenShot();

			await NetworkMgr.In.PostBugReportWebRequestAsync(m_logDataQueue,texture.EncodeToPNG());

			//? Send once and wait for 30 seconds -> If sent too frequently, it can cause a load.
			await UniTask.Delay(TimeSpan.FromSeconds(c_coolTimeTimer));

			m_sendLock = false;
		}
#endif
	}
}