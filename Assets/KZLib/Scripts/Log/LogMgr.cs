using System.Collections.Generic;
using UnityEngine;
using System;

#if !UNITY_EDITOR

using Cysharp.Threading.Tasks;

#endif

namespace KZLib
{
	public class LogMgr : Singleton<LogMgr>
	{
		private bool m_Disposed = false;

		private const int MAX_LOG_COUNT = 100;

		private readonly CircularQueue<MessageData> m_LogDataQueue = new(MAX_LOG_COUNT);

		public IEnumerable<MessageData> LogDataGroup => m_LogDataQueue;

#if !UNITY_EDITOR
		private const int COOL_TIME_TIMER = 30; // 30초
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

		private void OnGetLog(string _condition,string _stackTrace,LogType _type)
		{
			var head = $"<{GetLogTag(_type)}> {DateTime.Now:MM/dd HH:mm:ss:ff}";
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

			AddLog(head,body);
		}

		private string GetLogTag(LogType _logType)
		{
			return _logType switch
			{
				LogType.Warning => "Warning",
				LogType.Error or LogType.Exception => "Error",
				_ => "Info",
			};
		}

		public string ShowLog(LogTag _tag,object _message)
		{
			return $"[{_tag}] {_message}";
		}

		private void AddLog(string _head,string _body)
		{
			var data = new MessageData(_head,_body);

			m_LogDataQueue.Enqueue(data);
			OnAddLog?.Invoke(data);

#if !UNITY_EDITOR
			if(!m_SendLock && (_type == LogType.Exception))
			{
				SendBugAsync().Forget();
			}
#endif
		}

		public void ClearLog()
		{
			m_LogDataQueue.Clear();
		}

#if !UNITY_EDITOR
		private async UniTaskVoid SendBugAsync()
		{
			m_SendLock = true;

			await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);

			var texture = UnityUtility.GetScreenShot();

			await WebRequestUtility.SendBugReportAsync(m_LogDataQueue,texture.EncodeToPNG());

			//? Send once and wait for 30 seconds -> If sent too frequently, it can cause a load.
			await UniTask.Delay(TimeSpan.FromSeconds(COOL_TIME_TIMER));

			m_SendLock = false;
		}
#endif
	}
}