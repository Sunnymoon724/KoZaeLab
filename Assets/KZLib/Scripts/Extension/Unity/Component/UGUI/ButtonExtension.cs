using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public static class ButtonExtension
{
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