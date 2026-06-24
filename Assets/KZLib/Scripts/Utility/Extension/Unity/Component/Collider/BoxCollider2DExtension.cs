using UnityEngine;

/// <summary>
/// Extension methods for <see cref="BoxCollider2D"/> bounds, radius, and scale calculations.
/// </summary>
public static class BoxCollider2DExtension
{
	/// <summary>
	/// Returns local-space bounds from the collider offset and size.
	/// </summary>
	public static Bounds CalculateLocalBounds(this BoxCollider2D boxCollider2D)
	{
		if(!_IsValid(boxCollider2D))
		{
			return default;
		}

		return new Bounds(boxCollider2D.offset,boxCollider2D.size);
	}

	/// <summary>
	/// Returns the half-diagonal extent of the box plus <see cref="BoxCollider2D.edgeRadius"/>.
	/// </summary>
	public static float CalculateRadius(this BoxCollider2D boxCollider2D,out Vector2 localCenter)
	{
		if(!_IsValid(boxCollider2D))
		{
			localCenter = Vector2.zero;

			return -1.0f;
		}

		localCenter = boxCollider2D.offset;

		var halfWidth = boxCollider2D.size.x*0.5f;
		var halfHeight = boxCollider2D.size.y*0.5f;

		return Mathf.Sqrt(halfWidth*halfWidth+halfHeight*halfHeight)+boxCollider2D.edgeRadius;
	}

	/// <summary>
	/// Scales offset and size by the given vector; size components use absolute scale values.
	/// </summary>
	public static bool ApplyScale(this BoxCollider2D boxCollider2D,Vector3 scale)
	{
		if(!_IsValid(boxCollider2D))
		{
			return false;
		}

		boxCollider2D.offset = Vector2.Scale(boxCollider2D.offset,scale);
		boxCollider2D.size = Vector2.Scale(boxCollider2D.size,scale.Abs());

		return true;
	}

		private static bool _IsValid(BoxCollider2D boxCollider2D)
	{
		if(!boxCollider2D)
		{
			LogChannel.Kit.E("BoxCollider2D is null");

			return false;
		}

		return true;
	}
}
