using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public static class ScrollRectExtension
{
	/// <summary>
	/// Add One Kind Listener
	/// </summary>
	public static void AddListener(this ScrollRect _scrollRect,UnityAction<Vector2> _onAction,bool _overlap = false)
	{
		if(_scrollRect == null || _onAction == null)
		{
			return;
		}

		if(!_overlap)
		{
			_scrollRect.onValueChanged.RemoveListener(_onAction);
		}

		_scrollRect.onValueChanged.AddListener(_onAction);
	}

	/// <summary>
	/// Set One Listener
	/// </summary>
	public static void SetListener(this ScrollRect _scrollRect,UnityAction<Vector2> _onAction)
	{
		if(_scrollRect == null || _onAction == null)
		{
			return;
		}

		_scrollRect.onValueChanged.RemoveAllListeners();
		_scrollRect.onValueChanged.AddListener(_onAction);
	}

	/// <summary>
	/// Remove Listener
	/// </summary>
	public static void RemoveListener(this ScrollRect _scrollRect,UnityAction<Vector2> _onAction)
	{
		if(_scrollRect == null || _onAction == null)
		{
			return;
		}

		_scrollRect.onValueChanged.RemoveListener(_onAction);
	}

	/// <summary>
	/// Clear Listener
	/// </summary>
	public static void ClearListener(this ScrollRect _scrollRect)
	{
		if(_scrollRect == null)
		{
			return;
		}

		_scrollRect.onValueChanged.RemoveAllListeners();
	}
}