using UnityEngine;

public static class SpriteRendererExtension
{
	public static void SetSafeSpriteRenderer(this SpriteRenderer spriteRenderer,Sprite sprite,Color? color = null)
	{
		if(!IsValid(spriteRenderer))
		{
			return;
		}

		if(!sprite)
		{
			spriteRenderer.gameObject.SetActiveIfDifferent(false);

			return;
		}

		spriteRenderer.gameObject.SetActiveIfDifferent(true);

		spriteRenderer.sprite = sprite;

		if(!color.HasValue)
		{
			return;
		}

		spriteRenderer.color = color.Value;
	}

	private static bool IsValid(SpriteRenderer spriteRenderer)
	{
		if(!spriteRenderer)
		{
			LogTag.System.E("SpriteRenderer is null");

			return false;
		}

		return true;
	}
}