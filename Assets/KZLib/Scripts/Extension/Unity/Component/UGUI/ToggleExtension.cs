using UnityEngine.Events;
using UnityEngine.UI;

public static class ToggleExtension
{
	/// <summary>
	/// 이미 등록된 리스너가 있으면 제거하고 새로 등록
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
	/// 그냥 하나만 등록
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
	/// 리스너 제거
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
	/// 모든 리스너 제거
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