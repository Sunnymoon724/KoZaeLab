using UnityEngine.Events;

/// <summary>
/// Extension methods for safely adding, replacing, and clearing <see cref="UnityEvent"/> listeners.
/// </summary>
public static class UnityEventExtension
{
	/// <summary>
	/// Adds a listener, removing any existing registration for the same action unless <paramref name="isOverlap"/> is true.
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
	/// Replaces all listeners with a single action.
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
	/// Removes the specified listener when it is registered.
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
	/// Removes every registered listener.
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
	/// Adds a listener, removing any existing registration for the same action unless <paramref name="isOverlap"/> is true.
	/// </summary>
	public static void AddAction<TValue>(this UnityEvent<TValue> unityEvent,UnityAction<TValue> onAction,bool isOverlap = false)
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
	/// Replaces all listeners with a single action.
	/// </summary>
	public static void SetAction<TValue>(this UnityEvent<TValue> unityEvent,UnityAction<TValue> onAction)
	{
		if(!_IsValid(unityEvent))
		{
			return;
		}

		unityEvent.RemoveAllListeners();
		unityEvent.AddListener(onAction);
	}

	/// <summary>
	/// Removes the specified listener when it is registered.
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
	/// Removes every registered listener.
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
			LogChannel.Kit.E("UnityEvent is null");

			return false;
		}

		return true;
	}

	private static bool _IsValid<TValue>(UnityEvent<TValue> unityEvent)
	{
		if(unityEvent == null)
		{
			LogChannel.Kit.E("UnityEvent is null");

			return false;
		}

		return true;
	}
}