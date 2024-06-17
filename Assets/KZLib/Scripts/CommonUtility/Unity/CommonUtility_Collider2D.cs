using UnityEngine;

public static partial class CommonUtility
{
	public static Bounds GetLocalBounds(BoxCollider2D _collider)
	{
		return new Bounds(_collider.offset,_collider.size);
	}
	
	public static Bounds GetLocalBounds(CircleCollider2D _collider)
	{
		var diameter = _collider.radius*2.0f;

		return new Bounds(_collider.offset,new Vector3(diameter,diameter));
	}

	public static float GetRadius(BoxCollider2D _collider,out Vector2 _localCenter)
	{
		_localCenter = _collider.offset;

		var halfSize = _collider.size;

		halfSize.x *= 0.5f;
		halfSize.y *= 0.5f;

		return Mathf.Sqrt(halfSize.x*halfSize.x+halfSize.y*halfSize.y)+_collider.edgeRadius;
	}

	public static float GetRadius(CircleCollider2D _collider,out Vector2 _localCenter)
	{
		_localCenter = _collider.offset;
		
		return _collider.radius;
	}

	public static bool ApplyScale(BoxCollider2D _collider,Vector3 _scale)
	{
		_collider.offset *= _scale;
		_collider.size *= _scale.Abs();

		return true;
	}

	public static bool ApplyScale(CircleCollider2D _collider,Vector3 _scale)
	{
		_scale = _scale.Abs();
		_collider.radius *= Mathf.Max(_scale.x,_scale.y);

		return true;
	}
}