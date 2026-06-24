using UnityEngine;

/// <summary>
/// Extension methods for <see cref="Vector2"/> component access, rotation, and canvas input conversion.
/// </summary>
public static class Vector2Extension
{
	#region Set
	/// <summary>
	/// Returns a copy with the X component replaced.
	/// </summary>
	public static Vector2 SetX(this Vector2 vector,float x = 0.0f)
	{
		return new Vector2(x,vector.y);
	}

	/// <summary>
	/// Returns a copy with the Y component replaced.
	/// </summary>
	public static Vector2 SetY(this Vector2 vector,float y = 0.0f)
	{
		return new Vector2(vector.x,y);
	}
	#endregion Set

	#region Offset
	/// <summary>
	/// Returns a copy offset by the same value on both axes.
	/// </summary>
	public static Vector2 Offset(this Vector2 vector,float value)
	{
		return new Vector2(vector.x+value,vector.y+value);
	}

	/// <summary>
	/// Returns a copy offset along the X axis.
	/// </summary>
	public static Vector2 OffsetX(this Vector2 vector,float x)
	{
		return vector+new Vector2(x,0.0f);
	}

	/// <summary>
	/// Returns a copy offset along the Y axis.
	/// </summary>
	public static Vector2 OffsetY(this Vector2 vector,float y)
	{
		return vector+new Vector2(0.0f,y);
	}
    #endregion Offset

	#region Negate
	/// <summary>
	/// Negates all components.
	/// </summary>
	public static Vector2 Negate(this Vector2 vector)
	{
		return new Vector2(-vector.x,-vector.y);
	}

	/// <summary>
	/// Negates the X component.
	/// </summary>
	public static Vector2 NegateX(this Vector2 vector)
	{
		return new Vector2(-vector.x,vector.y);
	}

	/// <summary>
	/// Negates the Y component.
	/// </summary>
	public static Vector2 NegateY(this Vector2 vector)
	{
		return new Vector2(vector.x,-vector.y);
	}
	#endregion Negate

	#region ToVector
	/// <summary>
	/// Promotes to <see cref="Vector3"/> with optional Z.
	/// </summary>
	public static Vector3 ToVector3(this Vector2 vector,float z = 0.0f)
	{
		return new(vector.x,vector.y,z);
	}

	/// <summary>
	/// Promotes to <see cref="Vector4"/> with optional Z and W.
	/// </summary>
	public static Vector4 ToVector4(this Vector2 vector,float z = 0.0f,float w = 0.0f)
	{
		return new(vector.x,vector.y,z,w);
	}

	/// <summary>
	/// Formats the components as <c>(x, y)</c> with fixed decimal places.
	/// </summary>
	public static string ToVectorString(this Vector2 vector,int decimalPoint = 2)
	{
		var format = $"({"{0:f"}{decimalPoint}}}, {"{1:f"}{decimalPoint}}})";

		return string.Format(format,vector.x,vector.y);
	}
	#endregion ToVector


	/// <summary>
	/// Swaps the X and Y components.
	/// </summary>
	public static Vector2 SwapXY(this Vector2 vector)
	{
		return new Vector2(vector.y,vector.x);
	}

	/// <summary>
	/// Returns whether both components are approximately equal.
	/// </summary>
	public static bool AreEqual(this Vector2 vector1,Vector2 vector2)
	{
		return vector1.x.Approximately(vector2.x) && vector1.y.Approximately(vector2.y);
	}
	
	/// <summary>
	/// Returns whether both components are approximately zero.
	/// </summary>
	public static bool IsZero(this Vector2 vector) 
	{
		return vector.x.ApproximatelyZero() && vector.y.ApproximatelyZero();
	}

	/// <summary>
	/// Returns the component-wise reciprocal, treating near-zero components as zero.
	/// </summary>
	public static Vector2 Reciprocal(this Vector2 vector)
	{
		return new Vector2(vector.x.ApproximatelyZero() ? 0.0f : 1.0f/vector.x,vector.y.ApproximatelyZero() ? 0.0f : 1.0f/vector.y);
	}

	/// <summary>
	/// Returns the absolute component-wise distance to another vector.
	/// </summary>
	public static Vector2 DistanceEach(this Vector2 vector1,Vector2 vector2)
	{
		return new Vector2(Mathf.Abs(vector1.x-vector2.x),Mathf.Abs(vector1.y-vector2.y));
	}

	/// <summary>
	/// Rounds each component to the nearest integer.
	/// </summary>
	public static Vector2 Round(this Vector2 vector)
	{
		return new Vector2(Mathf.Round(vector.x),Mathf.Round(vector.y));
	}

	/// <summary>
	/// Applies ceiling to each component.
	/// </summary>
	public static Vector2 Ceil(this Vector2 vector)
	{
		return new Vector2(Mathf.Ceil(vector.x),Mathf.Ceil(vector.y));
	}

	/// <summary>
	/// Applies floor to each component.
	/// </summary>
	public static Vector2 Floor(this Vector2 vector)
	{
		return new Vector2(Mathf.Floor(vector.x),Mathf.Floor(vector.y));
	}

	/// <summary>
	/// Returns the absolute value of each component.
	/// </summary>
	public static Vector2 Abs(this Vector2 vector)
	{
		return new Vector2(Mathf.Abs(vector.x),Mathf.Abs(vector.y));
	}

	/// <summary>
	/// Clamps each component between the given bounds.
	/// </summary>
	public static Vector2 Clamp(this Vector2 vector,float min,float max)
	{
		return new Vector2(Mathf.Clamp(vector.x,min,max),Mathf.Clamp(vector.y,min,max));
	}

	/// <summary>
	/// Clamps each component to the 0–1 range.
	/// </summary>
	public static Vector2 Clamp01(this Vector2 vector)
	{
		return new Vector2(Mathf.Clamp01(vector.x),Mathf.Clamp01(vector.y));
	}

	/// <summary>
	/// Returns the largest component value.
	/// </summary>
	public static float MaxValue(this Vector2 vector)
	{
		return vector.x >= vector.y ? vector.x : vector.y;
	}

	/// <summary>
	/// Returns the smallest component value.
	/// </summary>
	public static float MinValue(this Vector2 vector)
	{
		return vector.x <= vector.y ? vector.x : vector.y;
	}

	/// <summary>
	/// Returns the component-wise product with another vector.
	/// </summary>
	public static Vector2 MultiplyEach(this Vector2 vector1,Vector2 vector2)
	{
		return new Vector2(vector1.x*vector2.x,vector1.y*vector2.y);
	}

	/// <summary>
	/// Returns the linear interpolation between two vectors.
	/// </summary>
	public static Vector2 Lerp(this Vector2 vector1,Vector2 vector2,float time = 0.5f)
	{
		return Vector2.Lerp(vector1,vector2,time);
	}

	/// <summary>
	/// Returns this vector rotated by the given angle in degrees.
	/// </summary>
	public static Vector2 Rotate(this Vector2 vector,float angle)
	{
		return Quaternion.AngleAxis(angle,Vector3.forward)*vector;
	}

	/// <summary>
	/// Returns the signed angle in degrees from the positive X axis to this vector.
	/// </summary>
	public static float Angle(this Vector2 vector)
	{
		return vector.y > 0.0f ? Vector2.Angle(Vector2.right,vector) : -Vector2.Angle(Vector2.right,vector);
	}

	/// <summary>
	/// Rotates this point around an origin by the given angle in radians.
	/// </summary>
	public static Vector2 RotateAround(this Vector2 point,Vector2 origin,float theta)
	{
		var sin = Mathf.Sin(theta);
		var cos = Mathf.Cos(theta);
		
		point -= origin;

		return new Vector2(point.x*cos-point.y*sin+origin.x,point.x*sin+point.y*cos+origin.y);
	}

	/// <summary>
	/// Finds the nearest point in the array and returns it with its index.
	/// </summary>
	public static (Vector2 Point,int Index) CalculateClosestPoint(this Vector2 point,params Vector2[] pointArray)
	{
		var index = Global.InvalidIndex;
		var closestPoint = Vector2.zero;
		var closestDistance = float.MaxValue;

		for(var i=0;i<pointArray.Length;i++)
		{
			var distance = (point-pointArray[i]).sqrMagnitude;

			if(distance < closestDistance)
			{
				closestDistance = distance;

				index = i;
				closestPoint = pointArray[i];
			}
		}

		return (closestPoint,index);
	}

	/// <summary>
	/// Projects this point onto an infinite ray and returns the closest point and ray parameter.
	/// </summary>
	public static (Vector2 Point,float RayParameter) ProjectOntoRay(this Vector2 point,Vector2 origin,Vector2 direction)
	{
		var directionSquared = Vector2.Dot(direction,direction);

		if(directionSquared.ApproximatelyZero())
		{
			return (origin,0.0f);
		}

		var rayParameter = Vector2.Dot(point-origin,direction)/directionSquared;

		return (origin+direction*rayParameter,rayParameter);
	}

	/// <summary>
	/// Projects this point onto a line segment and returns the closest point and segment parameter in the 0–1 range.
	/// </summary>
	public static (Vector2 Point,float RayParameter) ProjectOntoSegment(this Vector2 point,Vector2 start,Vector2 end)
	{
		var direction = end-start;
		var directionSquared = Vector2.Dot(direction,direction);

		if(directionSquared.ApproximatelyZero())
		{
			return (start,0.0f);
		}

		var rayParameter = Mathf.Clamp01(Vector2.Dot(point-start,direction)/directionSquared);

		return (start+direction*rayParameter,rayParameter);
	}

	/// <summary>
	/// Converts screen input to world space based on the parent canvas render mode.
	/// </summary>
	public static Vector3 TransformInputBasedOnCanvasType(this Vector2 point,RectTransform rectTrans)
	{
		var canvas = rectTrans.FindParentCanvas();

		if(!canvas || canvas.renderMode == RenderMode.ScreenSpaceOverlay)
		{
			return point;
		}

		var eventCamera = canvas.GetEventCamera();

		if(!eventCamera)
		{
			return point;
		}

		RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTrans,point,eventCamera,out var localPoint);

		return rectTrans.TransformPoint(localPoint);
	}

	/// <summary>
	/// Converts screen input to world space for the given canvas render mode.
	/// </summary>
	public static Vector3 TransformInputBasedOnCanvasType(this Vector2 input,Canvas canvas)
	{
		if(!canvas || canvas.renderMode == RenderMode.ScreenSpaceOverlay)
		{
			return input;
		}

		var eventCamera = canvas.GetEventCamera();

		if(!eventCamera)
		{
			return input;
		}

		var planeDistance = eventCamera.WorldToScreenPoint(canvas.transform.position).z;

		return eventCamera.ScreenToWorldPoint(new Vector3(input.x,input.y,planeDistance));
	}

	/// <summary>
	/// Returns the displacement from source to destination.
	/// </summary>
	public static Vector2 To(this Vector2 source,Vector2 destination)
	{
		return destination-source;
	}
}