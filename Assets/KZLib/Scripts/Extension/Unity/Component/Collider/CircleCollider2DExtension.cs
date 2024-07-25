using UnityEngine;
using Cysharp.Threading.Tasks;

public static class CircleCollider2DExtension
{
	public static Bounds GetLocalBounds(this CircleCollider2D _collider)
	{
		var diameter = _collider.radius*2.0f;

		return new Bounds(_collider.offset,new Vector3(diameter,diameter));
	}

	public static float GetRadius(this CircleCollider2D _collider,out Vector2 _localCenter)
	{
		_localCenter = _collider.offset;
		
		return _collider.radius;
	}

	public static bool ApplyScale(this CircleCollider2D _collider,Vector3 _scale)
	{
		_scale = _scale.Abs();
		_collider.radius *= Mathf.Max(_scale.x,_scale.y);

		return true;
	}
}