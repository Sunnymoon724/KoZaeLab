using UnityEngine;

public static class CameraExtension
{
	public static Bounds OrthographicBounds(this Camera _camera)
	{
		if(!_camera)
		{
			LogTag.System.E("Camera is null");

			return default;
		}

		var aspect = Screen.width / (float)Screen.height;
		var height = _camera.orthographicSize*2.0f;

		return new Bounds(_camera.transform.position,new Vector3(height*aspect,height,0.0f));
	}

	public static void ShowLayer(this Camera _camera,int _layerIndex)
	{
		if(!_camera)
		{
			LogTag.System.E("Camera is null");

			return;
		}

		_camera.cullingMask = _camera.cullingMask.AddFlag(1 << _layerIndex);
	}

	public static void ShowLayer(this Camera _camera,params string[] _layerNameArray)
	{
		if(!_camera)
		{
			LogTag.System.E("Camera is null");

			return;
		}

		_camera.cullingMask = _camera.cullingMask.AddFlag(LayerMask.GetMask(_layerNameArray));
	}

	public static void HideLayer(this Camera _camera,int _layerIndex)
	{
		if(!_camera)
		{
			LogTag.System.E("Camera is null");

			return;
		}

		_camera.cullingMask = _camera.cullingMask.RemoveFlag(1 << _layerIndex);
	}

	public static void HideLayer(this Camera _camera,params string[] _layerNameArray)
	{
		if(!_camera)
		{
			LogTag.System.E("Camera is null");

			return;
		}

		_camera.cullingMask = _camera.cullingMask.RemoveFlag(LayerMask.GetMask(_layerNameArray));
	}

	public static void ToggleLayerVisibility(this Camera _camera,int _layerIndex)
	{
		if(!_camera)
		{
			LogTag.System.E("Camera is null");

			return;
		}

		_camera.cullingMask ^= 1 << _layerIndex;
	}

	public static void ToggleLayerVisibility(this Camera _camera,params string[] _layerNameArray)
	{
		if(!_camera)
		{
			LogTag.System.E("Camera is null");

			return;
		}

		_camera.cullingMask ^= LayerMask.GetMask(_layerNameArray);
	}

	public static bool IsLayerShown(this Camera _camera,int _layerIndex)
	{
		if(!_camera)
		{
			LogTag.System.E("Camera is null");

			return false;
		}

		return _camera.cullingMask.HasFlag(1 << _layerIndex);
	}

	public static bool IsLayerShown(this Camera _camera,params string[] _layerNameArray)
	{
		if(!_camera)
		{
			LogTag.System.E("Camera is null");

			return false;
		}

		return _camera.cullingMask.HasFlag(LayerMask.GetMask(_layerNameArray));
	}

	public static void SetLayerVisibility(this Camera _camera,bool _isShow,int _layerIndex)
	{
		if(!_camera)
		{
			LogTag.System.E("Camera is null");

			return;
		}

		if(_isShow)
		{
			_camera.ShowLayer(_layerIndex);
		} 
		else
		{
			_camera.HideLayer(_layerIndex);
		}
	}

	public static void SetLayerVisibility(this Camera _camera,bool _isShow,params string[] _layerNameArray)
	{
		if(!_camera)
		{
			LogTag.System.E("Camera is null");

			return;
		}

		if(_isShow)
		{
			_camera.ShowLayer(_layerNameArray);
		} 
		else
		{
			_camera.HideLayer(_layerNameArray);
		}
	}

	public static bool TryGetDistanceToPlane(this Camera _camera,Plane _plane,out float _distance)
	{
		if(!_camera)
		{
			LogTag.System.E("Camera is null");

			_distance = Global.INVALID_NUMBER;

			return false;
		}

		var ray = _camera.ViewportPointToRay(Global.CENTER_VIEWPORT_POINT);
		var hit = _plane.Raycast(ray,out _distance);

		_distance += _camera.nearClipPlane;

		return hit;
	}

	public static bool TryGetDistanceToPointPlane(this Camera _camera,Vector3 _position,out float _distance)
	{
		if(!_camera)
		{
			LogTag.System.E("Camera is null");

			_distance = Global.INVALID_NUMBER;

			return false;
		}

		var plane = new Plane(-_camera.transform.forward,_position);

		return TryGetDistanceToPlane(_camera,plane,out _distance);
	}

	public static bool TryGetDistanceToObjectPlane(this Camera _camera,Transform _target,out float _distance)
	{
		if(!_camera || !_target)
		{
			LogTag.System.E($"Camera or Target null {_camera} or {_target}");

			_distance = Global.INVALID_NUMBER;

			return false;
		}

		return TryGetDistanceToPointPlane(_camera,_target.position,out _distance);
	}

	public static bool TryGetScaleForConsistentSize(this Camera _camera,Transform _transform,Transform _target,out float _scale)
	{
		if(!_camera)
		{
			LogTag.System.E("Camera is null");

			_scale = Global.INVALID_NUMBER;

			return false;
		}

		if(_camera.orthographic)
		{
			_scale = 1.0f;

			return true;
		}
		
		if(!TryGetDistanceToObjectPlane(_camera,_transform,out var source) || TryGetDistanceToObjectPlane(_camera,_target,out var destination))
		{
			_scale = 0.0f;

			return false;
		}

		_scale = destination/source;

		return !float.IsInfinity(_scale) && !float.IsNaN(_scale);
	}

	public static bool TryCastPositionToTargetPlane(this Camera _camera,Transform _transform,Transform _target,out Vector3 _position)
	{
		if(!_camera || !_transform)
		{
			LogTag.System.E($"Camera or Transform null {_camera} or {_transform}");

			_position = default;

			return false;
		}

		return TryCastPositionToTargetPlane(_camera,_transform.position,_target,out _position);
	}

	public static bool TryCastPositionToTargetPlane(this Camera _camera,Vector3 _position1,Transform _target,out Vector3 _position2)
	{
		if(!_camera || !_target)
		{
			LogTag.System.E($"Camera or Target null {_camera} or {_target}");

			_position2 = default;

			return false;
		}

		if(_camera.orthographic)
		{
			_position2 = _position1;

			return true;
		}

		var plane = new Plane(-_camera.transform.forward, _target.position);
		var ray = _camera.ViewportPointToRay(_camera.WorldToViewportPoint(_position1));

		if(plane.Raycast(ray,out var distance))
		{
			_position2 = ray.GetPoint(distance);

			return true;
		}

		_position2 = default;

		return false;
	}

	/// <summary>
	/// Calculate the screen space corners of the rect transform.
	/// </summary>
	public static void CalculateCornerOnScreen(this Camera _camera,RectTransform _transform,Vector3[] _positionArray,float _factor = 1.0f)
	{
		if(!_camera)
		{
			LogTag.System.E("Camera is null");

			return;
		}

		_transform.GetWorldCorners(_positionArray);

		for(var i=0;i<4;i++)
		{
			_positionArray[i] = _camera.WorldToScreenPoint(_positionArray[i])/_factor;
		}
	}
}