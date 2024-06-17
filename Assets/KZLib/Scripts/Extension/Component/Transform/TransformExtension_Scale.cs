using UnityEngine;

public static partial class TransformExtension
{
	public static void SetLocalScale(this Transform _transform,float _scale)
	{
		_transform.localScale = Vector3.one*_scale;
	}

	public static void SetLocalScaleX(this Transform _transform,float _x)
	{
		_transform.localScale = new Vector3(_x,_transform.localScale.y,_transform.localScale.z);
	}

	public static void SetLocalScaleY(this Transform _transform,float _y)
	{
		_transform.localScale = new Vector3(_transform.localScale.x,_y,_transform.localScale.z);
	}

	public static void SetLocalScaleZ(this Transform _transform,float _z)
	{
		_transform.localScale = new Vector3(_transform.localScale.x,_transform.localScale.y,_z);
	}

	public static void SetLossyScale(this Transform _transform,Vector3 _lossyScale)
	{
		_transform.localScale = Vector3.one;

		var lossyScale = _transform.lossyScale;

		_transform.localScale = new Vector3(_lossyScale.x/lossyScale.x,_lossyScale.y/lossyScale.y,_lossyScale.z/lossyScale.z);
	}
}