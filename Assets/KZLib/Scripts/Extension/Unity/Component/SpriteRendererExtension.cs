using UnityEngine;

public static class SpriteRendererExtension
{
	public static void SetSafeSpriteRenderer(this SpriteRenderer spriteRenderer,Sprite sprite,Color? color = null)
	{
		if(!_IsValid(spriteRenderer))
		{
			return;
		}

		if(!sprite)
		{
			spriteRenderer.gameObject.EnsureActive(false);

			return;
		}

		spriteRenderer.gameObject.EnsureActive(true);

		spriteRenderer.sprite = sprite;

		if(!color.HasValue)
		{
			return;
		}

		spriteRenderer.color = color.Value;
	}

	private static bool _IsValid(SpriteRenderer spriteRenderer)
	{
		if(!spriteRenderer)
		{
			LogTag.System.E("SpriteRenderer is null");

			return false;
		}

		return true;
	}
}