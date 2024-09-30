using UnityEngine.Events;
using UnityEngine.UI;

public static class ToggleExtension
{
	/// <summary>
	/// Add One Kind Listener
	/// </summary>
	public static void AddListener(this Toggle _toggle,UnityAction<bool> _onAction,bool _overlap = false)
	{
		if(_toggle == null || _onAction == null)
		{
			return;
		}

		if(!_overlap)
		{
			_toggle.onValueChanged.RemoveListener(_onAction);
		}

		_toggle.onValueChanged.AddListener(_onAction);
	}

	/// <summary>
	/// Set One Listener
	/// </summary>
	public static void SetListener(this Toggle _toggle,UnityAction<bool> _onAction)
	{
		if(_toggle == null || _onAction == null)
		{
			return;
		}

		_toggle.onValueChanged.RemoveAllListeners();
		_toggle.onValueChanged.AddListener(_onAction);
	}

	/// <summary>
	/// Remove Listener
	/// </summary>
	public static void RemoveListener(this Toggle _toggle,UnityAction<bool> _onAction)
	{
		if(_toggle == null || _onAction == null)
		{
			return;
		}

		_toggle.onValueChanged.RemoveListener(_onAction);
	}

	/// <summary>
	/// Clear Listener
	/// </summary>
	public static void ClearListener(this Toggle _toggle)
	{
		if(_toggle == null)
		{
			return;
		}

		_toggle.onValueChanged.RemoveAllListeners();
	}
}