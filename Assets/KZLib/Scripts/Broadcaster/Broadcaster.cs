using System;
using System.Collections.Generic;

namespace KZLib
{
	public static class Broadcaster
	{
		private static readonly Dictionary<EventTag,Delegate> s_ListenerDict = new();

		public static void EnableListener(EventTag _eventTag,Action _onAction)
		{
			ManageListener(_eventTag,_onAction,true);
		}

		public static void EnableListener<TDelegate>(EventTag _eventTag,Action<TDelegate> _onAction)
		{
			ManageListener(_eventTag,_onAction,true);
		}

		public static void DisableListener(EventTag _eventTag,Action _onAction)
		{
			ManageListener(_eventTag,_onAction,false);
		}

		public static void DisableListener<TDelegate>(EventTag _eventTag,Action<TDelegate> _onAction)
		{
			ManageListener(_eventTag,_onAction,false);
		}

		private static void ManageListener(EventTag _eventTag,Delegate _callback,bool _enable)
		{
			if(_callback == null)
			{
				return;
			}

			if(s_ListenerDict.TryGetValue(_eventTag,out var listener))
			{
				ValidateType(_eventTag,_callback,listener);

				s_ListenerDict[_eventTag] = _enable ? Delegate.Combine(listener,_callback) : Delegate.Remove(listener,_callback);

				if(!_enable && s_ListenerDict[_eventTag] == null)
				{
					s_ListenerDict.Remove(_eventTag);
				}
			}
			else if(_enable)
			{
				s_ListenerDict[_eventTag] = _callback;
			}
		}

		public static void SendEvent(EventTag _eventTag)
		{
			if(s_ListenerDict.TryGetValue(_eventTag,out var listener) && listener is Action action)
			{
				action();
			}

			throw new InvalidOperationException($"{listener.GetType().Name} is not in {_eventTag}.");
		}

		public static void SendEvent<TDelegate>(EventTag _eventTag,TDelegate _param)
		{
			if(s_ListenerDict.TryGetValue(_eventTag,out var listener) && listener is Action<TDelegate> action)
			{
				action(_param);

				return;
			}

			throw new InvalidOperationException($"{listener.GetType().Name} is not in {_eventTag}.");
		}

		private static void ValidateType(EventTag _eventTag,Delegate _callback,Delegate _listener)
		{
			var listenerType = _listener.GetType();
			var callbackType = _callback.GetType();

			if(listenerType != callbackType)
			{
				throw new InvalidOperationException($"{listenerType.Name} != {callbackType.Name} in {_eventTag}");
			}
		}

		private static void ValidateType(EventTag _eventTag,Delegate _callback)
		{
			var listenerType = s_ListenerDict[_eventTag].GetType();
			var callBackType = _callback.GetType();

			if(listenerType != callBackType)
			{
				throw new InvalidOperationException($"{listenerType.Name} != {callBackType.Name} in {_eventTag}");
			}
		}
	}
}