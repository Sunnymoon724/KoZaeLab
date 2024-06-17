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
		private const int MAX_LOG_COUNT = 100;

		private readonly CircularQueue<LogData> m_LogDataQueue = new(MAX_LOG_COUNT);

		public IEnumerable<LogData> LogDataGroup => m_LogDataQueue;

#if !UNITY_EDITOR
		private const int COOL_TIME_TIMER = 30*1000; // 30초
		private bool m_SendLock = false;
#endif
		private Action<LogData> m_OnAddLog = null;

		public event Action<LogData> OnAddLog
		{
			add { m_OnAddLog -= value; m_OnAddLog += value; }
			remove { m_OnAddLog -= value; }
		}

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

			Application.logMessageReceived -= OnGetLog;

			if(_disposing)
			{
				m_LogDataQueue.Clear();
			}

			base.Release(_disposing);
		}

		private void OnGetLog(string _condition,string _stack,LogType _type)
		{
			if(_type == LogType.Exception)
			{
				GetLog(LogType.Exception,string.Format("[Exception] {0} [{1}]",_condition,_stack));
			}
		}

		private string SetPrefix(Log _kind)
		{
			return _kind == Log.Normal ? string.Empty : string.Format("[{0}] ",_kind);
		}

		public string ShowLog(Log _kind,LogType _type,object _message,params object[] _argumentArray)
		{
			var text = string.Concat(SetPrefix(_kind),ConvertText(_message));

			if(_argumentArray.IsNullOrEmpty())
			{
				return GetLog(_type,text);
			}
			else
			{
				var objectArray = new object[_argumentArray.Length];

				for(var i=0;i<_argumentArray.Length;i++)
				{
					objectArray[i] = _argumentArray[i] is not string && _argumentArray[i] is IEnumerable ? ConvertText(_argumentArray[i]) : _argumentArray[i];
				}

				return GetLog(_type,string.Format(text,objectArray));
			}
		}

		private string GetLog(LogType _type,string _text)
		{
			AddLogData(_type,_text);

#if !UNITY_EDITOR
			if(!m_SendLock && (_type == LogType.Exception))
			{
				SendBugAsync().Forget();
			}
#endif

			return _text;
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

		private void AddLogData(LogType _type,string _log)
		{
			var data = new LogData(_type,_log);

			m_LogDataQueue.Enqueue(data);

			m_OnAddLog?.Invoke(data);
		}

#if !UNITY_EDITOR
		private async UniTaskVoid SendBugAsync()
		{
			m_SendLock = true;

			await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);

			var texture = CommonUtility.GetScreenShot();

			await CommonUtility.SendBugReportAsync(m_LogDataQueue,texture.EncodeToPNG());

			await UniTask.WaitForSeconds(COOL_TIME_TIMER);

			m_SendLock = false;
		}
#endif
	}
}