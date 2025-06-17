using UnityEngine;

public static class CircleCollider2DExtension
{
	public static Bounds CalculateLocalBounds(this CircleCollider2D circleCollider2D)
	{
		if(!_IsValid(circleCollider2D))
		{
			return default;
		}

		var diameter = circleCollider2D.radius*2.0f;

		return new Bounds(circleCollider2D.offset,new Vector3(diameter,diameter));
	}

	public static float CalculateRadius(this CircleCollider2D circleCollider2D,out Vector2 localCenter)
	{
		if(!_IsValid(circleCollider2D))
		{
			localCenter = Vector2.zero;

			return -1.0f;
		}

		localCenter = circleCollider2D.offset;
		
		return circleCollider2D.radius;
	}

	public static bool ApplyScale(this CircleCollider2D circleCollider2D,Vector3 scale)
	{
		if(!_IsValid(circleCollider2D))
		{
			return false;
		}

		scale = scale.Abs();
		circleCollider2D.radius *= Mathf.Max(scale.x,scale.y);

		return true;
	}

	private static bool _IsValid(CircleCollider2D circleCollider2D)
	{
		if(!circleCollider2D)
		{
			Logger.System.E("CircleCollider2D is null");

			return false;
		}

		return true;
	}
}