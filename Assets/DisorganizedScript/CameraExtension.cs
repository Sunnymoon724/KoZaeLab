using UnityEngine;

public static class CameraExtension
{
	private static readonly Vector3 s_CenterViewportPoint = new Vector3(0.5f,0.5f,1.0f);

	public static Bounds OrthographicBounds(this Camera _camera)
	{
		var aspect = (float)Screen.width/(float)Screen.height;
		var height = _camera.orthographicSize*2.0f;

		return new Bounds(_camera.transform.position,new Vector3(height*aspect,height,0.0f));
	}

	public static void ShowLayer(this Camera _camera,int _layerIndex)
	{
		_camera.cullingMask |= (1 << _layerIndex);
	}

	public static void ShowLayer(this Camera _camera,params string[] _layerNameArray)
	{
		_camera.cullingMask |= LayerMask.GetMask(_layerNameArray);
	}

	public static void HideLayer(this Camera _camera,int _layerIndex)
	{
		_camera.cullingMask &= ~(1 << _layerIndex);
	}

	public static void HideLayer(this Camera _camera,params string[] _layerNameArray)
	{
		_camera.cullingMask &= ~LayerMask.GetMask(_layerNameArray);
	}

	public static void ToggleLayerVisibility(this Camera _camera,int _layerIndex)
	{
		_camera.cullingMask ^= (1 << _layerIndex);
	}

	public static void ToggleLayerVisibility(this Camera _camera,params string[] _layerNameArray)
	{
		_camera.cullingMask ^= LayerMask.GetMask(_layerNameArray);
	}

	public static bool IsLayerShown(this Camera _camera,int _layerIndex)
	{
		return (_camera.cullingMask & (1 << _layerIndex)) > 0;
	}

	public static bool IsLayerShown(this Camera _camera,params string[] _layerNameArray)
	{
		return (_camera.cullingMask & LayerMask.GetMask(_layerNameArray)) > 0;
	}

	public static void SetLayerVisibility(this Camera _camera,bool _isShow,int _layerIndex)
	{
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
		var ray = _camera.ViewportPointToRay(s_CenterViewportPoint);
		var hit = _plane.Raycast(ray,out _distance);

		_distance += _camera.nearClipPlane;

		return hit;
	}
	
	public static bool TryGetDistanceToPointPlane(this Camera _camera, Vector3 _position,out float _distance)
	{
		var plane = new Plane(-_camera.transform.forward,_position);

		return TryGetDistanceToPlane(_camera,plane,out _distance);
	}
	
	public static bool TryGetDistanceToObjectPlane(this Camera _camera,Transform _transform,out float _distance)
	{
		if(_transform == null)
		{
			_distance = 0;

			return false;
		}

		return TryGetDistanceToPointPlane(_camera,_transform.position,out _distance);
	}
	
	public static bool TryGetScaleForConsistentSize(this Camera _camera,Transform _transform,Transform _target,out float _scale)
	{
		if(_camera.orthographic)
		{
			_scale = 1;

			return true;
		}
		
		if(!TryGetDistanceToObjectPlane(_camera,_transform,out var source) || TryGetDistanceToObjectPlane(_camera,_target,out var destination))
		{
			_scale = 0;
			return false;
		}

		_scale = destination/source;

		return !float.IsInfinity(_scale) && !float.IsNaN(_scale);
	}
	
	public static bool TryCastPositionToTargetPlane(this Camera _camera,Transform _transform,Transform _target,out Vector3 _position)
	{
		if(!_transform || !_target)
		{
			_position = default(Vector3);

			return false;
		}

		return TryCastPositionToTargetPlane(_camera,_transform.position,_target,out _position);
	}

	public static bool TryCastPositionToTargetPlane(this Camera _camera,Vector3 _position1,Transform _target,out Vector3 _position2)
	{
		if(!_target)
		{
			_position2 = default(Vector3);

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

		_position2 = default(Vector3);

		return false;
	}

	/// <summary>
	/// RectTransform의 네 모서리 영역의 화면좌표를 계산한다.
	/// </summary>
	public static void CalcCornerOnScreen(this Camera _camera,RectTransform _transform,Vector3[] _positionArray,float _factor = 1.0f)
	{
		_transform.GetWorldCorners(_positionArray);

		for(var i=0;i<4;i++)
		{
			_positionArray[i] = _camera.WorldToScreenPoint(_positionArray[i])/_factor;
		}
	}
}