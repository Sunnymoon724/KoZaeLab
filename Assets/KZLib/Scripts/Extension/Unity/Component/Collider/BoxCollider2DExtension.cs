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

		var halfWidth = _collider.size.x*0.5f;
		var halfHeight = _collider.size.y*0.5f;

		return Mathf.Sqrt(halfWidth*halfWidth+halfHeight*halfHeight)+_collider.edgeRadius;
	}

	public static bool ApplyScale(this BoxCollider2D _collider,Vector3 _scale)
	{
		_collider.offset = Vector2.Scale(_collider.offset,_scale);
		_collider.size = Vector2.Scale(_collider.size,_scale.Abs());

		return true;
	}
}