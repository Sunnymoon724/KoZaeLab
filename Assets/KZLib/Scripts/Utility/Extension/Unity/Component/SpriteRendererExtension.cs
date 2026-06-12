using UnityEngine;

/// <summary>
/// Extension methods for safely assigning sprites to a <see cref="SpriteRenderer"/>.
/// </summary>
public static class SpriteRendererExtension
{
	/// <summary>
	/// Assigns the sprite and optional color, deactivating the object when the sprite is null.
	/// </summary>
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
			LogChannel.Kit.E("SpriteRenderer is null");

			return false;
		}

		return true;
	}
}