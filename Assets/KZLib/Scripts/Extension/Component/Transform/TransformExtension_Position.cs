using UnityEngine;

public static partial class TransformExtension
{
	public static void SetPosition(this Transform _transform,Vector3 _position)
	{
		_transform.position = _position;
	}

	public static void SetPositionXY(this Transform _transform,Vector2 _point)
	{
		_transform.position = new Vector3(_point.x,_point.y,_transform.position.z);
	}

	public static void SetPositionXZ(this Transform _transform,Vector2 _point)
	{
		_transform.position = new Vector3(_point.x,_transform.position.y,_point.y);
	}

	public static void SetPositionYZ(this Transform _transform,Vector2 _point)
	{
		_transform.position = new Vector3(_transform.position.x,_point.x,_point.y);
	}

	public static void SetPositionX(this Transform _transform,float _x)
	{
		_transform.position = new Vector3(_x,_transform.position.y,_transform.position.z);
	}
	
	public static void SetPositionY(this Transform _transform,float _y)
	{
		_transform.position = new Vector3(_transform.position.x,_y,_transform.position.z);
	}

	public static void SetPositionZ(this Transform _transform,float _z)
	{
		_transform.position = new Vector3(_transform.position.x,_transform.position.y,_z);
	}

	public static void AddPositionX(this Transform _transform,float _x)
	{
		_transform.SetPositionX(_transform.position.x+_x);
	}

	public static void AddPositionY(this Transform _transform,float _y)
	{
		_transform.SetPositionY(_transform.position.y+_y);
	}

	public static void AddPositionZ(this Transform _transform,float _z)
	{
		_transform.SetPositionZ(_transform.position.z+_z);
	}

	public static void SetLocalPosition(this Transform _transform,Vector3 _position)
	{
		_transform.localPosition = _position;
	}

	public static void SetLocalPositionXY(this Transform _transform,Vector2 _point)
	{
		_transform.localPosition = new Vector3(_point.x,_point.y,_transform.localPosition.z);
	}

	public static void SetLocalPositionXZ(this Transform _transform,Vector2 _point)
	{
		_transform.localPosition = new Vector3(_point.x,_transform.localPosition.y,_point.y);
	}

	public static void SetLocalPositionYZ(this Transform _transform,Vector2 _point)
	{
		_transform.localPosition = new Vector3(_transform.localPosition.x,_point.x,_point.y);
	}

	public static void SetLocalPositionX(this Transform _transform,float _x)
	{
		_transform.localPosition = new Vector3(_x,_transform.localPosition.y,_transform.localPosition.z);
	}
	
	public static void SetLocalPositionY(this Transform _transform,float _y)
	{
		_transform.localPosition = new Vector3(_transform.localPosition.x,_y,_transform.localPosition.z);
	}

	public static void SetLocalPositionZ(this Transform _transform,float _z)
	{
		_transform.localPosition = new Vector3(_transform.localPosition.x,_transform.localPosition.y,_z);
	}

	public static void AddLocalPositionX(this Transform _transform,float _x)
	{
		_transform.SetLocalPositionX(_transform.localPosition.x+_x);
	}

	public static void AddLocalPositionY(this Transform _transform,float _y)
	{
		_transform.SetLocalPositionY(_transform.localPosition.y+_y);
	}

	public static void AddLocalPositionZ(this Transform _transform,float _z)
	{
		_transform.SetLocalPositionZ(_transform.localPosition.z+_z);
	}

	public static Vector2 ScreenPosition(this Transform _transform)
	{
		return ScreenPosition(_transform,Camera.main);
	}
	
	public static Vector2 ScreenPosition(this Transform _transform,Camera _camera)
	{
		return RectTransformUtility.WorldToScreenPoint(_camera,_transform.position);
	}

	public static Vector3 ViewportPosition(this Transform _transform)
	{
		return ViewportPosition(_transform,Camera.main);
	}
	
	public static Vector3 ViewportPosition(this Transform _transform,Camera _camera)
	{
		return RectTransformUtility.WorldToScreenPoint(_camera,_transform.position);
	}
}