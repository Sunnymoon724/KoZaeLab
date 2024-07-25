using UnityEngine;
using Cysharp.Threading.Tasks;

public static class BoxCollider2DExtension
{
	public static Bounds GetLocalBounds(this BoxCollider2D _collider)
	{
		return new Bounds(_collider.offset,_collider.size);
	}

	public static float GetRadius(this BoxCollider2D _collider,out Vector2 _localCenter)
	{
		_localCenter = _collider.offset;

		var halfSize = _collider.size;

		halfSize.x *= 0.5f;
		halfSize.y *= 0.5f;

		return Mathf.Sqrt(halfSize.x*halfSize.x+halfSize.y*halfSize.y)+_collider.edgeRadius;
	}

	public static bool ApplyScale(this BoxCollider2D _collider,Vector3 _scale)
	{
		_collider.offset *= _scale;
		_collider.size *= _scale.Abs();

		return true;
	}
}