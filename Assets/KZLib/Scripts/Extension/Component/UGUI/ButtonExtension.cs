using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public static class ButtonExtension
{
	public static void SetDisabledColor(this Button _button,Color _color)
	{
		var colorBlock = _button.colors;

		colorBlock.disabledColor = _color;
		_button.colors = colorBlock;
	}

	public static void SetSelectedColor(this Button _button,Color _color)
	{
		var colorBlock = _button.colors;

		colorBlock.selectedColor = _color;
		_button.colors = colorBlock;
	}

	public static void SetNormalColor(this Button _button,Color _color)
	{
		var colorBlock = _button.colors;

		colorBlock.normalColor = _color;
		_button.colors = colorBlock;
	}

	public static void SetHighlightedColor(this Button _button,Color _color)
	{
		var colorBlock = _button.colors;

		colorBlock.highlightedColor = _color;
		_button.colors = colorBlock;
	}

	public static void SetPressedColor(this Button _button,Color _color)
	{
		var colorBlock = _button.colors;

		colorBlock.pressedColor = _color;
		_button.colors = colorBlock;
	}

	/// <summary>
	/// 중복 방지 용
	/// </summary>
	public static void SetOnClickListener(this Button _button,UnityAction _onAction)
	{
		if(_onAction == null)
		{
			return;
		}

		_button.onClick.RemoveListener(_onAction);
		_button.onClick.AddListener(_onAction);
	}

	public static async UniTask FadeImageAsync(this Button _button,float _duration,Color _prev,Color _next)
	{
		var image = _button.GetComponent<Image>();

		await image.FadeImageAsync(_duration,_prev,_next);
	}
}