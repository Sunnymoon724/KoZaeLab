using UnityEngine.Events;

public static class UnityEventExtension
{
	/// <summary>
	/// Add One Kind Listener
	/// </summary>
	public static void AddAction(this UnityEvent unityEvent,UnityAction onAction,bool isOverlap = false)
	{
		if(!_IsValid(unityEvent))
		{
			return;
		}

		if(!isOverlap)
		{
			unityEvent.RemoveListener(onAction);
		}

		unityEvent.AddListener(onAction);
	}

	/// <summary>
	/// Set One Listener
	/// </summary>
	public static void SetAction(this UnityEvent unityEvent,UnityAction onAction)
	{
		if(!_IsValid(unityEvent))
		{
			return;
		}

		unityEvent.RemoveAllListeners();
		unityEvent.AddListener(onAction);
	}

	/// <summary>
	/// Remove Listener
	/// </summary>
	public static void RemoveAction(this UnityEvent unityEvent,UnityAction onAction)
	{
		if(!_IsValid(unityEvent))
		{
			return;
		}

		unityEvent.RemoveListener(onAction);
	}

	/// <summary>
	/// Clear Listener
	/// </summary>
	public static void ClearAction(this UnityEvent unityEvent)
	{
		if(!_IsValid(unityEvent))
		{
			return;
		}

		unityEvent.RemoveAllListeners();
	}

	/// <summary>
	/// Add One Kind Listener
	/// </summary>
	public static void AddAction<TData>(this UnityEvent<TData> unityEvent,UnityAction<TData> onAction,bool isOverlap = false)
	{
		if(!_IsValid(unityEvent))
		{
			return;
		}

		if(!isOverlap)
		{
			unityEvent.RemoveListener(onAction);
		}

		unityEvent.AddListener(onAction);
	}

	/// <summary>
	/// Set One Listener
	/// </summary>
	public static void SetAction<TData>(this UnityEvent<TData> unityEvent,UnityAction<TData> onAction)
	{
		if(!_IsValid(unityEvent))
		{
			return;
		}

		unityEvent.RemoveAllListeners();
		unityEvent.AddListener(onAction);
	}

	/// <summary>
	/// Remove Listener
	/// </summary>
	public static void RemoveAction<TValue>(this UnityEvent<TValue> unityEvent,UnityAction<TValue> onAction)
	{
		if(!_IsValid(unityEvent))
		{
			return;
		}

		unityEvent.RemoveListener(onAction);
	}

	/// <summary>
	/// Clear Listener
	/// </summary>
	public static void ClearAction<TValue>(this UnityEvent<TValue> unityEvent)
	{
		if(!_IsValid(unityEvent))
		{
			return;
		}

		unityEvent.RemoveAllListeners();
	}

	private static bool _IsValid(UnityEvent unityEvent)
	{
		if(unityEvent == null)
		{
			LogSvc.System.E("UnityEvent is null");

			return false;
		}

		return true;
	}

	private static bool _IsValid<TValue>(UnityEvent<TValue> unityEvent)
	{
		if(unityEvent == null)
		{
			LogSvc.System.E("UnityEvent is null");

			return false;
		}

		return true;
	}
}