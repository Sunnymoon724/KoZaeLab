using System;
using System.Collections.Generic;

namespace KZLib.KZDevelop
{
	public static class Broadcaster
	{
		private static readonly Dictionary<EventTag,Delegate> s_ListenerDict = new();

		public static void EnableListener(EventTag _eventTag,Action _onAction)
		{
			EnableListenerInner(_eventTag,_onAction);
		}

		public static void EnableListener<TDelegate>(EventTag _eventTag,Action<TDelegate> _onAction)
		{
			EnableListenerInner(_eventTag,_onAction);
		}

		private static void EnableListenerInner(EventTag _eventTag,Delegate _callback)
		{
			ValidateCallback(_eventTag,_callback);

			if(!s_ListenerDict.ContainsKey(_eventTag))
			{
				s_ListenerDict.Add(_eventTag,_callback);

				return;
			}

			ValidateListener(_eventTag);
			ValidateType(_eventTag,_callback);

			s_ListenerDict[_eventTag] = Delegate.Combine(s_ListenerDict[_eventTag],_callback);
		}

		public static void DisableListener(EventTag _eventTag,Action _onAction)
		{
			DisableListenerInner(_eventTag,_onAction);
		}

		public static void DisableListener<TDelegate>(EventTag _eventTag,Action<TDelegate> _onAction)
		{
			DisableListenerInner(_eventTag,_onAction);
		}

		private static void DisableListenerInner(EventTag _eventTag,Delegate _callback)
		{
			ValidateCallback(_eventTag,_callback);

			if(!s_ListenerDict.ContainsKey(_eventTag))
			{
				return;
			}

			ValidateListener(_eventTag);
			ValidateType(_eventTag,_callback);

			s_ListenerDict[_eventTag] = Delegate.Remove(s_ListenerDict[_eventTag],_callback);

			if(s_ListenerDict[_eventTag] == null)
			{
				s_ListenerDict.Remove(_eventTag);
			}
		}

		public static void SendEvent(EventTag _eventTag)
		{
			if(!s_ListenerDict.ContainsKey(_eventTag))
			{
				return;
			}

			ValidateListener(_eventTag);

			var listener = s_ListenerDict[_eventTag];
			var result = listener as Action ?? throw new NullReferenceException(string.Format("{0}의 타입에 문제가 있습니다.[{1}]",_eventTag,listener.GetType().Name));

			result();
		}

		public static void SendEvent<TDelegate>(EventTag _eventTag,TDelegate _param)
		{
			if(!s_ListenerDict.ContainsKey(_eventTag))
			{
				return;
			}

			ValidateListener(_eventTag);

			var listener = s_ListenerDict[_eventTag];
			var result = listener as Action<TDelegate> ?? throw new NullReferenceException(string.Format("{0}의 타입에 문제가 있습니다.[{1}]",_eventTag,listener.GetType().Name));

			result(_param);
		}

		private static void ValidateCallback(EventTag _eventTag,Delegate _callback)
		{
			if(_callback == null)
			{
				throw new ArgumentNullException(string.Format("{0}의 콜백이 존재하지 않습니다.",_eventTag));
			}
		}

		private static void ValidateListener(EventTag _eventTag)
		{
			if(s_ListenerDict[_eventTag] == null)
			{
				throw new NullReferenceException(string.Format("{0}가 Null 입니다.",_eventTag));
			}
		}

		private static void ValidateType(EventTag _eventTag,Delegate _callback)
		{
			var listenerType = s_ListenerDict[_eventTag].GetType();
			var callBackType = _callback.GetType();

            if(listenerType != callBackType)
			{
				throw new InvalidOperationException(string.Format("{0}의 이벤트 타입[{1}]과 현재 이벤트의 타입[{2}]이 다릅니다.",_eventTag,listenerType.Name,callBackType.Name));
			}
		}
	}
}