using UnityEngine;

public static partial class TransformExtension
{
	public static void ResetTransform(this Transform _transform,Transform _parent = null)
	{
		if(!_transform)
		{
			LogTag.System.E("Transform is null.");

			return;
		}

		if(_parent)
		{
			_transform.SetParent(_parent,true);
		}

		_transform.SetLocalPositionAndRotation(Vector3.zero,Quaternion.identity);
		_transform.localScale = Vector3.one;
	}

	public static bool IsFront(this Transform _transform,Vector3 _target)
	{
		if(!_transform)
		{
			LogTag.System.E("Transform is null.");

			return false;
		}

		return Vector3.Dot(_transform.forward,_target-_transform.position) >= 0.0f;
	}

	public static bool IsRight(this Transform _transform,Vector3 _target)
	{
		if(!_transform)
		{
			LogTag.System.E("Transform is null.");

			return false;
		}

		return Vector3.Cross(_transform.forward,_target-_transform.position).y >= 0.0f;
	}

	public static void LookAtSlowly(this Transform _transform,Transform _target,float _speed = 1.0f,bool _holdX = false,bool _holdY = false,bool _holdZ = false)
	{
		if(!_transform || !_target)
		{
			LogTag.System.E($"Transform or Target is null. {_transform} or {_target}");

			return;
		}

		var direction = _target.position-_transform.position;

		direction.x = _holdX ? 0.0f : direction.x;
		direction.y = _holdY ? 0.0f : direction.y;
		direction.z = _holdZ ? 0.0f : direction.z;

		var rotation = Quaternion.LookRotation(direction);

		_transform.rotation = Quaternion.Slerp(_transform.rotation,rotation,Time.deltaTime*_speed);
	}

	public static bool IsInside(this Transform _transform,Collider _collider)
	{
		if(!_transform || !_collider)
		{
			LogTag.System.E($"Transform or Collider is null. {_transform} or {_collider}");

			return false;
		}

		return _transform.position.IsInside(_collider);
	}

	public static bool IsNearlyFacingTowards(this Transform _transform,Vector3 _position,float _cos = 0.95f,bool isSamePlane = false)
	{
		if(!_transform)
		{
			LogTag.System.E("Transform is null.");

			return false;
		}

		var position = _transform.position;

		if(isSamePlane)
		{
			_position = new Vector3(_position.x,position.y,_position.z);
		}

		var direction = _position - position;

		return Vector3.Dot(_transform.forward,direction.normalized) >= _cos;
	}

	public static void LookAt2D(this Transform _transform,Vector3 _target)
	{
		if(!_transform)
		{
			LogTag.System.E("Transform is null.");

			return;
		}

		_transform.rotation = Quaternion.LookRotation(Vector3.forward,-(_target-_transform.position));
		_transform.Rotate(Vector3.forward,-90.0f);
	}
}