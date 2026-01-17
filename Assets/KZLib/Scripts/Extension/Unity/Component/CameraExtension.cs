using UnityEngine;
using UnityEngine.EventSystems;

public static class CameraExtension
{
	public static Bounds OrthographicBounds(this Camera camera)
	{
		if(!_IsValid(camera))
		{
			return default;
		}

		var aspect = Screen.width / (float)Screen.height;
		var height = camera.orthographicSize*2.0f;

		return new Bounds(camera.transform.position,new Vector3(height*aspect,height,0.0f));
	}

	public static void ShowLayer(this Camera camera,int layerIndex)
	{
		if(!_IsValid(camera))
		{
			return;
		}

		camera.cullingMask = camera.cullingMask.AddFlag(1 << layerIndex);
	}

	public static void ShowLayer(this Camera camera,params string[] layerNameArray)
	{
		if(!_IsValid(camera))
		{
			return;
		}

		camera.cullingMask = camera.cullingMask.AddFlag(LayerMask.GetMask(layerNameArray));
	}

	public static void HideLayer(this Camera camera,int layerIndex)
	{
		if(!_IsValid(camera))
		{
			return;
		}

		camera.cullingMask = camera.cullingMask.RemoveFlag(1 << layerIndex);
	}

	public static void HideLayer(this Camera camera,params string[] layerNameArray)
	{
		if(!_IsValid(camera))
		{
			return;
		}

		camera.cullingMask = camera.cullingMask.RemoveFlag(LayerMask.GetMask(layerNameArray));
	}

	public static void ToggleLayerVisibility(this Camera camera,int layerIndex)
	{
		if(!_IsValid(camera))
		{
			return;
		}

		camera.cullingMask ^= 1 << layerIndex;
	}

	public static void ToggleLayerVisibility(this Camera camera,params string[] layerNameArray)
	{
		if(!_IsValid(camera))
		{
			return;
		}

		camera.cullingMask ^= LayerMask.GetMask(layerNameArray);
	}

	public static bool IsLayerShown(this Camera camera,int layerIndex)
	{
		if(!_IsValid(camera))
		{
			return default;
		}

		return camera.cullingMask.HasFlag(1 << layerIndex);
	}

	public static bool IsLayerShown(this Camera camera,params string[] layerNameArray)
	{
		if(!_IsValid(camera))
		{
			return default;
		}

		return camera.cullingMask.HasFlag(LayerMask.GetMask(layerNameArray));
	}

	public static void SetLayerVisibility(this Camera camera,bool isShow,int layerIndex)
	{
		if(!_IsValid(camera))
		{
			return;
		}

		if(isShow)
		{
			camera.ShowLayer(layerIndex);
		} 
		else
		{
			camera.HideLayer(layerIndex);
		}
	}

	public static void SetLayerVisibility(this Camera camera,bool isShow,params string[] layerNameArray)
	{
		if(!_IsValid(camera))
		{
			return;
		}

		if(isShow)
		{
			camera.ShowLayer(layerNameArray);
		} 
		else
		{
			camera.HideLayer(layerNameArray);
		}
	}

	public static bool TryGetDistanceToPlane(this Camera camera,Plane plane,out float distance)
	{
		if(!_IsValid(camera))
		{
			distance = 0.0f;

			return false;
		}

		var ray = camera.ViewportPointToRay(Global.CENTER_VIEWPORT_POINT);
		var hit = plane.Raycast(ray,out distance);

		distance += camera.nearClipPlane;

		return hit;
	}

	public static bool TryGetDistanceToPositionPlane(this Camera camera,Vector3 position,out float distance)
	{
		if(!_IsValid(camera))
		{
			distance = 0.0f;

			return false;
		}

		var plane = new Plane(-camera.transform.forward,position);

		return TryGetDistanceToPlane(camera,plane,out distance);
	}

	public static bool TryGetDistanceToObjectPlane(this Camera camera,Transform target,out float distance)
	{
		if(!_IsValid(camera))
		{
			distance = 0.0f;

			return false;
		}

		if(!target)
		{
			LogChannel.System.E("Target is null");

			distance = 0.0f;

			return false;
		}

		return TryGetDistanceToPositionPlane(camera,target.position,out distance);
	}

	public static bool TryGetScaleForConsistentSize(this Camera camera,Transform transform,Transform target,out float scale)
	{
		if(!_IsValid(camera))
		{
			scale = 0.0f;

			return false;
		}

		if(camera.orthographic)
		{
			scale = 1.0f;

			return true;
		}
		
		if(!TryGetDistanceToObjectPlane(camera,transform,out var source) || TryGetDistanceToObjectPlane(camera,target,out var destination))
		{
			scale = 0.0f;

			return false;
		}

		scale = destination/source;

		return !float.IsInfinity(scale) && !float.IsNaN(scale);
	}

	public static bool TryCastPositionToTargetPlane(this Camera camera,Transform transform,Transform target,out Vector3 position)
	{
		if(!_IsValid(camera))
		{
			position = Vector3.zero;

			return false;
		}

		if(!transform)
		{
			LogChannel.System.E("Transform is null");

			position = Vector3.zero;

			return false;
		}

		return TryCastPositionToTargetPlane(camera,transform.position,target,out position);
	}

	public static bool TryCastPositionToTargetPlane(this Camera camera,Vector3 position1,Transform target,out Vector3 position2)
	{
		if(!_IsValid(camera))
		{
			position2 = Vector3.zero;

			return false;
		}

		if(!target)
		{
			LogChannel.System.E("Target is null");

			position2 = Vector3.zero;

			return false;
		}

		if(camera.orthographic)
		{
			position2 = position1;

			return true;
		}

		var plane = new Plane(-camera.transform.forward, target.position);
		var ray = camera.ViewportPointToRay(camera.WorldToViewportPoint(position1));

		if(plane.Raycast(ray,out var distance))
		{
			position2 = ray.GetPoint(distance);

			return true;
		}

		position2 = Vector3.zero;

		return false;
	}

	/// <summary>
	/// Transform the screen space corners of the rect transform.
	/// </summary>
	public static void TransformCornersToScreen(this Camera camera,RectTransform rectTrans,Vector3[] positionArray,float factor = 1.0f)
	{
		if(!_IsValid(camera))
		{
			return;
		}

		rectTrans.GetWorldCorners(positionArray);

		for(var i=0;i<4;i++)
		{
			positionArray[i] = camera.WorldToScreenPoint(positionArray[i])/factor;
		}
	}

	public static bool IsInsideViewport(this Camera camera,Vector2 position)
	{
		if(!_IsValid(camera))
		{
			return false;
		}

		return camera.pixelRect.Contains(position);
	}

	private static bool _IsValid(Camera camera)
	{
		if(!camera)
		{
			LogChannel.System.E("Camera is null");

			return false;
		}

		return true;
	}
}