using UnityEngine;

public static class Vector2Extension
{
	#region Set
	public static Vector2 SetX(this Vector2 vector,float x = 0.0f)
	{
		return new Vector2(x,vector.y);
	}

	public static Vector2 SetY(this Vector2 vector,float y = 0.0f)
	{
		return new Vector2(vector.x,y);
	}
	#endregion Set

	#region Offset
	public static Vector2 Offset(this Vector2 vector,Vector2 offset)
	{
		return new Vector2(vector.x+offset.x,vector.y+offset.y);
	}

	public static Vector2 OffsetX(this Vector2 vector,float x)
	{
		return vector.Offset(new(x,0.0f));
	}

	public static Vector2 OffsetY(this Vector2 vector,float y)
	{
		return vector.Offset(new(0.0f,y));
	}
    #endregion Offset

	#region Invert
	public static Vector2 Invert(this Vector2 vector)
	{
		return new Vector2(-vector.x,-vector.y);
	}

	public static Vector2 InvertX(this Vector2 vector)
	{
		return new Vector2(-vector.x,vector.y);
	}

	public static Vector2 InvertY(this Vector2 vector)
	{
		return new Vector2(vector.x,-vector.y);
	}
	#endregion Invert

	#region ToVector
	public static Vector3 ToVector3(this Vector2 vector,float z = 0.0f)
	{
		return new(vector.x,vector.y,z);
	}

	public static Vector4 ToVector4(this Vector3 vector,float z = 0.0f,float w = 0.0f)
	{
		return new(vector.x,vector.y,z,w);
	}

	public static string ToVectorString(this Vector2 vector,int decimalPoint = 2)
	{
		var format = $"({"{0:f"}{decimalPoint}}}, {"{1:f"}{decimalPoint}}})";

		return string.Format(format,vector.x,vector.y);
	}
	#endregion ToVector


	public static Vector2 Transpose(this Vector2 vector)
	{
		return new Vector2(vector.y,vector.x);
	}

	public static bool IsEquals(this Vector2 vector1,Vector2 vector2)
	{
		return vector1.x.Approximately(vector2.x) && vector1.y.Approximately(vector2.y);
	}
	
	public static bool IsZero(this Vector2 vector) 
	{
		return vector.x.ApproximatelyZero() && vector.y.ApproximatelyZero();
	}

	public static Vector2 Reciprocal(this Vector2 vector)
	{
		return new Vector2(1.0f/vector.x,1.0f/vector.y);
	}

	public static Vector2 DistanceEach(this Vector2 vector1,Vector2 vector2)
	{
		return new Vector2(Mathf.Abs(vector1.x-vector2.x),Mathf.Abs(vector1.y-vector2.y));
	}

	public static Vector2 Round(this Vector2 vector)
	{
		return new Vector2(Mathf.Round(vector.x),Mathf.Round(vector.y));
	}

	public static Vector2 Ceil(this Vector2 vector)
	{
		return new Vector2(Mathf.Ceil(vector.x),Mathf.Ceil(vector.y));
	}

	public static Vector2 Floor(this Vector2 vector)
	{
		return new Vector2(Mathf.Floor(vector.x),Mathf.Floor(vector.y));
	}

	public static Vector2 Abs(this Vector2 vector)
	{
		return new Vector2(Mathf.Abs(vector.x),Mathf.Abs(vector.y));
	}

	public static Vector2 Clamp(this Vector2 vector,float _min,float _max)
	{
		return new Vector2(Mathf.Clamp(vector.x,_min,_max),Mathf.Clamp(vector.y,_min,_max));
	}

	public static Vector2 Clamp01(this Vector2 vector)
	{
		return new Vector2(Mathf.Clamp01(vector.x),Mathf.Clamp01(vector.y));
	}

	public static float MaxValue(this Vector2 vector)
	{
		return vector.x >= vector.y ? vector.x : vector.y;
	}

	public static float MinValue(this Vector2 vector)
	{
		return vector.x <= vector.y ? vector.x : vector.y;
	}

	public static Vector2 MultiplyEach(this Vector2 vector1,Vector2 vector2)
	{
		return new Vector2(vector1.x*vector2.x,vector1.y*vector2.y);
	}

	public static Vector2 MiddleVector(this Vector2 vector1,Vector2 vector2)
	{
		return new Vector2((vector2.x-vector1.x)/2.0f,(vector2.y-vector1.y)/2.0f);
	}

	public static Vector2 Rotate(this Vector2 vector,float angle)
	{
		return Quaternion.AngleAxis(angle,Vector3.forward)*vector;
	}

	public static float Angle(this Vector2 vector)
	{
		return vector.y > 0.0f ? Vector2.Angle(Vector2.right,vector) : -Vector2.Angle(Vector2.right,vector);
	}

	public static Vector2 RotateAround(this Vector2 point,Vector2 origin,float theta)
	{
		var sin = Mathf.Sin(theta);
		var cos = Mathf.Cos(theta);
		
		point -= origin;

		return new Vector2(point.x*cos-point.y*sin+origin.x,point.x*sin+point.y*cos+origin.y);
	}

	public static (Vector2 Point,int Index) CalculateClosestPoint(this Vector2 point,params Vector2[] pointArray)
	{
		var index = Global.INVALID_INDEX;
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

	public static (Vector2 point,float distance) CalculateClosestPointOnRay(this Vector2 point,Vector2 origin,Vector2 direction)
	{
		var distance = Vector2.Dot(point-origin,direction);

		return (origin+direction*distance,distance);
	}

	// public static (Vector2 point,float distance) CalculateClosestPointOnSegment(this Vector2 point,Vector2 _start,Vector2 _end)
	// {
	// 	var direction = _end-_start;
	// 	var magnitude = direction.magnitude;

	// 	direction.Normalize();

	// 	var distance = Mathf.Clamp(Vector2.Dot(point-_start,direction),0.0f,magnitude);

	// 	return (_start+direction*distance,distance);
	// }

	public static Vector3 TransformInputBasedOnCanvasType(this Vector2 point,RectTransform rectTransform)
	{
		var canvas = rectTransform.FindParentCanvas();

		if(point == Vector2.zero || canvas.renderMode == RenderMode.ScreenSpaceOverlay)
		{
			return point;
		}

		RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform,point,canvas.GetEventCamera(),out var movePos);

		return canvas.transform.TransformPoint(movePos);
	}

	public static Vector2 TransformInputBasedOnCanvasType(this Vector2 input,Canvas canvas)
	{
		return canvas.renderMode != RenderMode.ScreenSpaceOverlay ? input : canvas.GetEventCamera().ScreenToWorldPoint(input);
	}

	public static Vector2 To(this Vector2 source,Vector2 destination)
	{
		return destination-source;
	}
}