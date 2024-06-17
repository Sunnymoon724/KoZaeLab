using UnityEngine;

public static class SpriteRendererExtension
{
	public static void SetSafeSpriteRenderer(this SpriteRenderer _renderer,Sprite _sprite,Color? _color = null)
	{
		if(!_renderer)
		{
			return;
		}

		if(!_sprite)
		{
			_renderer.gameObject.SetActiveSelf(false);

			return;
		}

		_renderer.gameObject.SetActiveSelf(true);

		_renderer.sprite = _sprite;

		if(!_color.HasValue)
		{
			return;
		}

		_renderer.color = _color.Value;
	}
}