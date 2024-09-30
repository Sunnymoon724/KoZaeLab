using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public static class ButtonExtension
{
	/// <summary>
	/// Add One Kind Listener
	/// </summary>
	public static void AddListener(this Button _button,UnityAction _onAction,bool _overlap = false)
	{
		if(_button == null || _onAction == null)
		{
			return;
		}

		if(!_overlap)
		{
			_button.onClick.RemoveListener(_onAction);
		}

		_button.onClick.AddListener(_onAction);
	}

	/// <summary>
	/// Set One Listener
	/// </summary>
	public static void SetListener(this Button _button,UnityAction _onAction)
	{
		if(_button == null || _onAction == null)
		{
			return;
		}

		_button.onClick.RemoveAllListeners();
		_button.onClick.AddListener(_onAction);
	}

	/// <summary>
	/// Remove Listener
	/// </summary>
	public static void RemoveListener(this Button _button,UnityAction _onAction)
	{
		if(_button == null || _onAction == null)
		{
			return;
		}

		_button.onClick.RemoveListener(_onAction);
	}

	/// <summary>
	/// Clear Listener
	/// </summary>
	public static void ClearListener(this Button _button)
	{
		if(_button == null)
		{
			return;
		}

		_button.onClick.RemoveAllListeners();
	}

	public static async UniTask FadeImageAsync(this Button _button,float _duration,Color _prev,Color _next)
	{
		var image = _button.GetComponent<Image>();

		await image.FadeImageAsync(_duration,_prev,_next);
	}
}