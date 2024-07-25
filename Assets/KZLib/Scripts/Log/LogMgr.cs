using System.Collections.Generic;
using UnityEngine;
using System.Collections;
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
			var head = string.Format("<{0}> {1}",GetLogTag(_type),DateTime.Now.ToString("MM/dd HH:mm:ss:ff"));
			var body = string.Empty;

			if(_type == LogType.Exception)
			{
				body = string.Format("{0}\n\n{1}",_condition,_stackTrace);
			}
			else
			{
				var stackTraceArray = _stackTrace.Split('\n');
				var index = stackTraceArray.FindIndex(x => x.Contains(nameof(LogTag)));
				var stackTrace = stackTraceArray[index+1];

				body = string.Format("{0}\n\n{1}",_condition,stackTrace);
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

		public string ShowLog(LogTag _tag,object _message,params object[] _argumentArray)
		{
			var text = string.Format("[{0}] {1}",_tag,ConvertText(_message));

			if(_argumentArray.IsNullOrEmpty())
			{
				return text;
			}
			else
			{
				var objectArray = new object[_argumentArray.Length];

				for(var i=0;i<_argumentArray.Length;i++)
				{
					objectArray[i] = _argumentArray[i] is not string && _argumentArray[i] is IEnumerable ? ConvertText(_argumentArray[i]) : _argumentArray[i];
				}

				return string.Format(text,objectArray);
			}
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

		private string ConvertText(object _object)
		{
			if(_object == null)
			{
				return "NULL";
			}

			if(_object is not string && _object is IEnumerable dataGroup)
			{
				var textList = new List<string>();

				foreach(var data in dataGroup)
				{
					textList.Add(ConvertText(data));
				}

				var count = textList.Count;

				return count == 0 ? string.Format("Empty - [{0}]",_object) : string.Format("{0} - [{1}]",count,string.Join(" & ",textList));
			}

			return _object.ToString();
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

			var texture = CommonUtility.GetScreenShot();

			await CommonUtility.SendBugReportAsync(m_LogDataQueue,texture.EncodeToPNG());

			// 한번 보내고 30초동안 대기 -> 너무 자주 보내면 부하가 있음
			await UniTask.WaitForSeconds(COOL_TIME_TIMER);

			m_SendLock = false;
		}
#endif
	}
}