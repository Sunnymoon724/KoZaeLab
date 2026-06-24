using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Extension methods for <see cref="Camera"/> bounds, culling masks, and projection helpers.
/// </summary>
public static class CameraExtension
{
	/// <summary>
	/// Returns the world-space bounds visible by an orthographic camera at the current screen aspect.
	/// </summary>
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

	/// <summary>
	/// Adds the layer at <paramref name="layerIndex"/> to the camera culling mask.
	/// </summary>
	public static void ShowLayer(this Camera camera,int layerIndex)
	{
		if(!_IsValid(camera))
		{
			return;
		}

		camera.cullingMask = camera.cullingMask.AddFlag(1 << layerIndex);
	}

	/// <summary>
	/// Adds the layers named in <paramref name="layerNameArray"/> to the camera culling mask.
	/// </summary>
	public static void ShowLayer(this Camera camera,params string[] layerNameArray)
	{
		if(!_IsValid(camera))
		{
			return;
		}

		camera.cullingMask = camera.cullingMask.AddFlag(LayerMask.GetMask(layerNameArray));
	}

	/// <summary>
	/// Removes the layer at <paramref name="layerIndex"/> from the camera culling mask.
	/// </summary>
	public static void HideLayer(this Camera camera,int layerIndex)
	{
		if(!_IsValid(camera))
		{
			return;
		}

		camera.cullingMask = camera.cullingMask.RemoveFlag(1 << layerIndex);
	}

	/// <summary>
	/// Removes the layers named in <paramref name="layerNameArray"/> from the camera culling mask.
	/// </summary>
	public static void HideLayer(this Camera camera,params string[] layerNameArray)
	{
		if(!_IsValid(camera))
		{
			return;
		}

		camera.cullingMask = camera.cullingMask.RemoveFlag(LayerMask.GetMask(layerNameArray));
	}

	/// <summary>
	/// Toggles visibility of the layer at <paramref name="layerIndex"/> in the culling mask.
	/// </summary>
	public static void ToggleLayerVisibility(this Camera camera,int layerIndex)
	{
		if(!_IsValid(camera))
		{
			return;
		}

		camera.cullingMask ^= 1 << layerIndex;
	}

	/// <summary>
	/// Toggles visibility of the layers named in <paramref name="layerNameArray"/> in the culling mask.
	/// </summary>
	public static void ToggleLayerVisibility(this Camera camera,params string[] layerNameArray)
	{
		if(!_IsValid(camera))
		{
			return;
		}

		camera.cullingMask ^= LayerMask.GetMask(layerNameArray);
	}

	/// <summary>
	/// Returns whether the layer at <paramref name="layerIndex"/> is included in the culling mask.
	/// </summary>
	public static bool IsLayerShown(this Camera camera,int layerIndex)
	{
		if(!_IsValid(camera))
		{
			return default;
		}

		return camera.cullingMask.HasAnyFlag(1 << layerIndex);
	}

	/// <summary>
	/// Returns whether all layers named in <paramref name="layerNameArray"/> are included in the culling mask.
	/// </summary>
	public static bool IsLayerShown(this Camera camera,params string[] layerNameArray)
	{
		if(!_IsValid(camera))
		{
			return default;
		}

		return camera.cullingMask.HasAllFlags(LayerMask.GetMask(layerNameArray));
	}

	/// <summary>
	/// Shows or hides the layer at <paramref name="layerIndex"/> in the culling mask.
	/// </summary>
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

	/// <summary>
	/// Shows or hides the layers named in <paramref name="layerNameArray"/> in the culling mask.
	/// </summary>
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

	/// <summary>
	/// Casts a viewport-center ray against a plane and returns the hit distance including the near clip plane.
	/// </summary>
	public static bool TryGetDistanceToPlane(this Camera camera,Plane plane,out float distance)
	{
		if(!_IsValid(camera))
		{
			distance = 0.0f;

			return false;
		}

		var ray = camera.ViewportPointToRay(Global.CenterViewportPoint);
		var hit = plane.Raycast(ray,out distance);

		distance += camera.nearClipPlane;

		return hit;
	}

	/// <summary>
	/// Casts a viewport-center ray against a plane through <paramref name="position"/> and returns the hit distance.
	/// </summary>
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

	/// <summary>
	/// Casts a viewport-center ray against a plane through <paramref name="target"/> and returns the hit distance.
	/// </summary>
	public static bool TryGetDistanceToObjectPlane(this Camera camera,Transform target,out float distance)
	{
		if(!_IsValid(camera))
		{
			distance = 0.0f;

			return false;
		}

		if(!target)
		{
			LogChannel.Kit.E("Target is null");

			distance = 0.0f;

			return false;
		}

		return TryGetDistanceToPositionPlane(camera,target.position,out distance);
	}

	/// <summary>
	/// Computes the scale ratio needed to keep an object the same apparent size at two different depths.
	/// </summary>
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
		
		if(!TryGetDistanceToObjectPlane(camera,transform,out var source) || !TryGetDistanceToObjectPlane(camera,target,out var destination))
		{
			scale = 0.0f;

			return false;
		}

		scale = destination/source;

		return !float.IsInfinity(scale) && !float.IsNaN(scale);
	}

	/// <summary>
	/// Projects <paramref name="transform"/>'s position onto the plane facing the camera at <paramref name="target"/>'s depth.
	/// </summary>
	public static bool TryCastPositionToTargetPlane(this Camera camera,Transform transform,Transform target,out Vector3 position)
	{
		if(!_IsValid(camera))
		{
			position = Vector3.zero;

			return false;
		}

		if(!transform)
		{
			LogChannel.Kit.E("Transform is null");

			position = Vector3.zero;

			return false;
		}

		return TryCastPositionToTargetPlane(camera,transform.position,target,out position);
	}

	/// <summary>
	/// Projects a world position onto the plane facing the camera at the target's depth.
	/// </summary>
	public static bool TryCastPositionToTargetPlane(this Camera camera,Vector3 position1,Transform target,out Vector3 position2)
	{
		if(!_IsValid(camera))
		{
			position2 = Vector3.zero;

			return false;
		}

		if(!target)
		{
			LogChannel.Kit.E("Target is null");

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

		for(var i=0;i<Global.RectCornerCount;i++)
		{
			positionArray[i] = camera.WorldToScreenPoint(positionArray[i])/factor;
		}
	}

	/// <summary>
	/// Returns whether <paramref name="position"/> lies inside the camera pixel rect.
	/// </summary>
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
			LogChannel.Kit.E("Camera is null");

			return false;
		}

		return true;
	}
}