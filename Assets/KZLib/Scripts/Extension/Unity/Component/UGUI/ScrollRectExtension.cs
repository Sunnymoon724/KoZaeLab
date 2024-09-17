using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public static class ScrollRectExtension
{
	/// <summary>
	/// 이미 등록된 리스너가 있으면 제거하고 새로 등록
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
	/// 그냥 하나만 등록
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
	/// 리스너 제거
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
	/// 모든 리스너 제거
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