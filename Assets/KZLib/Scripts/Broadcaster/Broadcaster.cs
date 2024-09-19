using System;
using System.Collections.Generic;

namespace KZLib
{
	public static class Broadcaster
	{
		private static readonly Dictionary<EventTag,Delegate> s_ListenerDict = new();

		/// <summary>
		/// 이벤트에 리스너를 추가
		/// </summary>
		public static void EnableListener(EventTag _eventTag,Action _onAction)
		{
			EnableListenerInner(_eventTag,_onAction);
		}

		/// <summary>
		/// 이벤트에 리스너를 추가
		/// </summary>
		public static void EnableListener<TDelegate>(EventTag _eventTag,Action<TDelegate> _onAction)
		{
			EnableListenerInner(_eventTag,_onAction);
		}

		private static void EnableListenerInner(EventTag _eventTag,Delegate _callback)
		{
			if(_callback == null)
			{
				return;
			}

			ValidateType(_eventTag,_callback);

			if(s_ListenerDict.TryGetValue(_eventTag,out var listener))
			{
				s_ListenerDict[_eventTag] = listener == null ? _callback : Delegate.Combine(listener,_callback);
			}
			else
			{
				s_ListenerDict[_eventTag] = _callback;
			}
		}

		/// <summary>
		/// 이벤트에 추가된 리스너를 삭제
		/// </summary>
		public static void DisableListener(EventTag _eventTag,Action _onAction)
		{
			DisableListenerInner(_eventTag,_onAction);
		}

		/// <summary>
		/// 이벤트에 추가된 리스너를 삭제
		/// </summary>
		public static void DisableListener<TDelegate>(EventTag _eventTag,Action<TDelegate> _onAction)
		{
			DisableListenerInner(_eventTag,_onAction);
		}

		private static void DisableListenerInner(EventTag _eventTag,Delegate _callback)
		{
			if(_callback == null || !s_ListenerDict.TryGetValue(_eventTag,out var listener) || listener == null)
			{
				return;
			}

			ValidateType(_eventTag,_callback);

			s_ListenerDict[_eventTag] = Delegate.Remove(listener,_callback);

			if(s_ListenerDict[_eventTag] == null)
			{
				s_ListenerDict.Remove(_eventTag);
			}
		}

		/// <summary>
		/// 이벤트를 발생시킵니다.
		/// </summary>
		public static void SendEvent(EventTag _eventTag)
		{
			if(s_ListenerDict.TryGetValue(_eventTag,out var listener) && listener is Action action)
			{
				action();
			}
			else
			{
				throw new InvalidOperationException($"{_eventTag}에 잘못된 리스너 타입이 등록되었습니다. [Action  != {listener.GetType().Name}]");
			}
		}

		/// <summary>
		/// 이벤트를 발생시킵니다.
		/// </summary>
		public static void SendEvent<TDelegate>(EventTag _eventTag,TDelegate _param)
		{
			if(s_ListenerDict.TryGetValue(_eventTag,out var listener) && listener is Action<TDelegate> action)
			{
				action(_param);
			}
			else
			{
				throw new InvalidOperationException($"{_eventTag}에 잘못된 리스너 타입이 등록되었습니다. [Action<TDelegate>  != {listener.GetType().Name}]");
			}
		}

		private static void ValidateType(EventTag _eventTag, Delegate _callback)
		{
			var listenerType = s_ListenerDict[_eventTag].GetType();
			var callBackType = _callback.GetType();

			if (listenerType != callBackType)
			{
				throw new InvalidOperationException($"{_eventTag}의 이벤트 타입[{listenerType.Name}]과 현재 이벤트의 타입[{callBackType.Name}]이 다릅니다.");
			}
		}
	}
}