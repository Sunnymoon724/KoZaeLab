using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Text.RegularExpressions;

#if !UNITY_EDITOR

using Cysharp.Threading.Tasks;

#endif

namespace KZLib
{
	public class LogMgr : Singleton<LogMgr>
	{
		private bool m_Disposed = false;

		private static readonly Regex FILE_NAME_EXTRACTOR = new(@"([^\\\/]+)(?=\.[^.\\\/]+$)", RegexOptions.Compiled);

		private const int MAX_LOG_COUNT = 100;

		private readonly CircularQueue<MessageData> m_LogDataQueue = new(MAX_LOG_COUNT);

		public IEnumerable<MessageData> LogDataGroup => m_LogDataQueue;

#if !UNITY_EDITOR
		private const int COOL_TIME_TIMER = 30; // 30s
		private bool m_SendLock = false;
#endif

		public MoreAction<MessageData> OnAddLog { get; set; }

		protected override void Initialize()
		{
			Application.logMessageReceived += OnGetLog;
		}

		protected override void Release(bool _disposing)
		{
			if(m_Disposed)
			{
				return;
			}

			if(_disposing)
			{
				Application.logMessageReceived -= OnGetLog;

				m_LogDataQueue.Clear();
			}

			m_Disposed = true;

			base.Release(_disposing);
		}

		public string CreateLog(LogTag _tag,object _message,string _memberName,string _filePath,int _lineNum)
		{
			var builder = new StringBuilder();

			builder.Append($"[{_tag}] {_message}");

			builder.Append(" [");

			if(!_memberName.IsEmpty())
			{
				builder.Append($"at {_memberName}(..)");
			}

			if(!_filePath.IsEmpty())
			{
				var match = FILE_NAME_EXTRACTOR.Match(_filePath);

				if(match.Success)
				{
					builder.Append($" - in {match.Groups[1].Value}:{_lineNum}");
				}
			}

			builder.Append("]");

			return builder.ToString();
		}

		private void OnGetLog(string _condition,string _stackTrace,LogType _type)
		{
			var head = $"<{GetLogTag(_type)}> {DateTime.Now:MM/dd HH:mm:ss}";
			var body = string.Empty;

			if(_type == LogType.Exception)
			{
				body = $"{_condition}\n\n{_stackTrace}";
			}
			else
			{
				var stackTraceArray = _stackTrace.Split('\n');
				var index = stackTraceArray.FindIndex(x => x.Contains(nameof(LogTag)));
				var stackTrace = stackTraceArray[index+1];

				body = $"{_condition}\n\n{stackTrace}";
			}

			var data = new MessageData(head,body);

			m_LogDataQueue.Enqueue(data);
			OnAddLog?.Invoke(data);

#if !UNITY_EDITOR
			if(!m_SendLock && (_type == LogType.Exception))
			{
				SendBugAsync().Forget();
			}
#endif
		}

		private string GetLogTag(LogType _logType)
		{
			return _logType switch
			{
				LogType.Warning => "Warning",
				LogType.Error or LogType.Exception or LogType.Assert => "Error",
				_ => "Info",
			};
		}

		public void ClearLogData()
		{
			m_LogDataQueue.Clear();
		}

#if !UNITY_EDITOR
		private async UniTaskVoid SendBugAsync()
		{
			m_SendLock = true;

			await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);

			var texture = CommonUtility.GetScreenShot();

			await WebRequestUtility.SendBugReportAsync(m_LogDataQueue,texture.EncodeToPNG());

			//? Send once and wait for 30 seconds -> If sent too frequently, it can cause a load.
			await UniTask.Delay(TimeSpan.FromSeconds(COOL_TIME_TIMER));

			m_SendLock = false;
		}
#endif
	}
}