using UnityEngine.Events;

public static class UnityEventExtension
{
	/// <summary>
	/// Add One Kind Listener
	/// </summary>
	public static void AddAction(this UnityEvent _Event,UnityAction _onAction,bool _overlap = false)
	{
		if(_Event == null || _onAction == null)
		{
			LogTag.System.E($"Event or action is null. [{_Event} or {_onAction}]");

			return;
		}

		if(!_overlap)
		{
			_Event.RemoveListener(_onAction);
		}

		_Event.AddListener(_onAction);
	}

	/// <summary>
	/// Set One Listener
	/// </summary>
	public static void SetAction(this UnityEvent _Event,UnityAction _onAction)
	{
		if(_Event == null || _onAction == null)
		{
			LogTag.System.E($"Event or action is null. [{_Event} or {_onAction}]");

			return;
		}

		_Event.RemoveAllListeners();
		_Event.AddListener(_onAction);
	}

	/// <summary>
	/// Remove Listener
	/// </summary>
	public static void RemoveAction(this UnityEvent _Event,UnityAction _onAction)
	{
		if(_Event == null || _onAction == null)
		{
			LogTag.System.E($"Event or action is null. [{_Event} or {_onAction}]");

			return;
		}

		_Event.RemoveListener(_onAction);
	}

	/// <summary>
	/// Clear Listener
	/// </summary>
	public static void ClearAction(this UnityEvent _Event)
	{
		if(_Event == null)
		{
			LogTag.System.E("Event is null.");

			return;
		}

		_Event.RemoveAllListeners();
	}

	/// <summary>
	/// Add One Kind Listener
	/// </summary>
	public static void AddAction<TData>(this UnityEvent<TData> _Event,UnityAction<TData> _onAction,bool _overlap = false)
	{
		if(_Event == null || _onAction == null)
		{
			LogTag.System.E($"Event or action is null. [{_Event} or {_onAction}]");

			return;
		}

		if(!_overlap)
		{
			_Event.RemoveListener(_onAction);
		}

		_Event.AddListener(_onAction);
	}

	/// <summary>
	/// Set One Listener
	/// </summary>
	public static void SetAction<TData>(this UnityEvent<TData> _Event,UnityAction<TData> _onAction)
	{
		if(_Event == null || _onAction == null)
		{
			LogTag.System.E($"Event or action is null. [{_Event} or {_onAction}]");

			return;
		}

		_Event.RemoveAllListeners();
		_Event.AddListener(_onAction);
	}

	/// <summary>
	/// Remove Listener
	/// </summary>
	public static void RemoveAction<TData>(this UnityEvent<TData> _Event,UnityAction<TData> _onAction)
	{
		if(_Event == null || _onAction == null)
		{
			LogTag.System.E($"Event or action is null. [{_Event} or {_onAction}]");

			return;
		}

		_Event.RemoveListener(_onAction);
	}

	/// <summary>
	/// Clear Listener
	/// </summary>
	public static void ClearAction<TData>(this UnityEvent<TData> _Event)
	{
		if(_Event == null)
		{
			LogTag.System.E("Event is null.");

			return;
		}

		_Event.RemoveAllListeners();
	}
}