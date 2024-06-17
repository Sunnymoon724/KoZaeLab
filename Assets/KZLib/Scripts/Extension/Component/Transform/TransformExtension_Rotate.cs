using UnityEngine;

public static partial class TransformExtension
{
	public static void SetRotation(this Transform _transform,float _angle)
	{
		_transform.rotation = new Quaternion();
		_transform.Rotate(Vector3.forward,_angle);
	}

	public static void RotateAroundTarget(this Transform _transform,Vector3 _target,Vector3 _axis,float _speed,bool _look)
	{
		var offset = Quaternion.AngleAxis(_speed,_look ? Vector3.up : _axis)*(_target-_transform.position);

		_transform.position = _target+offset;

		if(_look)
		{
			_transform.rotation = Quaternion.LookRotation(-offset,_axis);
		}
	}
}