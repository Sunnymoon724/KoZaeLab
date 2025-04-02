using UnityEngine;

public static class BoxCollider2DExtension
{
	public static Bounds CalculateLocalBounds(this BoxCollider2D boxCollider2D)
	{
		if(!_IsValid(boxCollider2D))
		{
			return default;
		}

		return new Bounds(boxCollider2D.offset,boxCollider2D.size);
	}

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
			LogTag.System.E("BoxCollider2D is null");

			return false;
		}

		return true;
	}
}