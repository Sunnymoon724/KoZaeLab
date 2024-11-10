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
				LogTag.System.E("Callback is null");

				return;
			}

			if(s_ListenerDict.TryGetValue(_eventTag,out var listener))
			{
				var listenerType = listener.GetType();
				var callbackType = _callback.GetType();

				if(listenerType != callbackType)
				{
					LogTag.System.E($"{listenerType.Name} != {callbackType.Name} in {_eventTag}");

					return;
				}

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
			if(s_ListenerDict.TryGetValue(_eventTag,out var listener) && listener is Action onAction)
			{
				onAction();
			}
			else
			{
				LogTag.System.E($"{listener.GetType().Name} is not in {_eventTag}.");
			}
		}

		public static void SendEvent<TDelegate>(EventTag _eventTag,TDelegate _param)
		{
			if(s_ListenerDict.TryGetValue(_eventTag,out var listener) && listener is Action<TDelegate> onAction)
			{
				onAction(_param);
			}
			else
			{
				LogTag.System.E($"{listener.GetType().Name} is not in {_eventTag}.");
			}
		}
	}
}