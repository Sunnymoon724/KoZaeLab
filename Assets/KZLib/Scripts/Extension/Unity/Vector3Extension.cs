using UnityEngine;

public static class Vector3Extension
{
	#region Set
	public static Vector3 SetX(this Vector3 vector,float x = 0.0f)
	{
		return new Vector3(x,vector.y,vector.z);
	}

	public static Vector3 SetY(this Vector3 vector,float y = 0.0f)
	{
		return new Vector3(vector.x,y,vector.z);
	}

	public static Vector3 SetZ(this Vector3 vector,float z = 0.0f)
	{
		return new Vector3(vector.x,vector.y,z);
	}

	public static Vector3 SetXY(this Vector3 vector,float x = 0.0f,float y = 0.0f) 
	{
		return new Vector3(x,y,vector.z);
	}

	public static Vector3 SetXY(this Vector3 vector,Vector2 point)
	{
		return vector.SetXY(point.x,point.y);
	}

	public static Vector3 SetXZ(this Vector3 vector,float x = 0.0f,float z = 0.0f) 
	{
		return new Vector3(x,vector.y,z);
	}

	public static Vector3 SetXZ(this Vector3 vector,Vector2 point)
	{
		return vector.SetXZ(point.x,point.y);
	}

	public static Vector3 SetYZ(this Vector3 vector,float y = 0.0f,float z = 0.0f) 
	{
		return new Vector3(vector.x,y,z);
	}

	public static Vector3 SetYZ(this Vector3 vector,Vector2 point)
	{
		return vector.SetYZ(point.x,point.y);
	}
	#endregion Set

	#region Offset
	public static Vector3 Offset(this Vector3 vector,float value)
	{
		return new Vector3(vector.x+value,vector.y+value,vector.z+value);
	}

	public static Vector3 OffsetXY(this Vector3 vector,Vector2 offset)
	{
		return vector+(Vector3) offset;
	}

	public static Vector3 OffsetXZ(this Vector3 vector,Vector2 offset)
	{
		return vector+new Vector3(offset.x,0.0f,offset.y);
	}

	public static Vector3 OffsetYZ(this Vector3 vector,Vector2 offset)
	{
		return vector+new Vector3(0.0f,offset.x,offset.y);
	}

	public static Vector3 OffsetX(this Vector3 vector,float x)
	{
		return vector+new Vector3(x,0.0f,0.0f);
	}

	public static Vector3 OffsetY(this Vector3 vector,float y)
	{
		return vector+new Vector3(0.0f,y,0.0f);
	}

	public static Vector3 OffsetZ(this Vector3 vector,float z)
	{
		return vector+new Vector3(0.0f,0.0f,z);
	}
	#endregion Offset

	#region Invert
	public static Vector3 Invert(this Vector3 vector)
	{
		return new Vector3(-vector.x,-vector.y,-vector.z);
	}

	public static Vector3 InvertX(this Vector3 vector)
	{
		return new Vector3(-vector.x,vector.y,vector.z);
	}

	public static Vector3 InvertY(this Vector3 vector)
	{
		return new Vector3(vector.x,-vector.y,vector.z);
	}

	public static Vector3 InvertZ(this Vector3 vector)
	{
		return new Vector3(vector.x,vector.y,-vector.z);
	}

	public static Vector3 InvertXY(this Vector3 vector)
	{
		return new Vector3(-vector.x,-vector.y,vector.z);
	}

	public static Vector3 InvertXZ(this Vector3 vector)
	{
		return new Vector3(-vector.x,vector.y,-vector.z);
	}

	public static Vector3 InvertYZ(this Vector3 vector)
	{
		return new Vector3(vector.x,-vector.y,-vector.z);
	}
	#endregion Invert

	#region ToVector
	public static Vector2 ToVector2(this Vector3 vector)
	{
		return new(vector.x,vector.y);
	}

	public static Vector4 ToVector4(this Vector3 vector,float w = 0.0f)
	{
		return new(vector.x,vector.y,vector.z,w);
	}

	public static string ToVectorString(this Vector3 vector,int decimalPoint = 2)
	{
		var format = $"({"{0:f"}{decimalPoint}}}, {"{1:f"}{decimalPoint}}}, {"{2:f"}{decimalPoint}}})";

		return string.Format(format,vector.x,vector.y,vector.z);
	}
	#endregion ToVector

	public static Vector3 PlaneDirection(this Vector3 vector,Vector3 target)
	{
		var pivot = vector.SetY();
		var result = target.SetY();
		
		return (pivot-result).normalized;
	}

	public static float PlaneDistance(this Vector3 vector,Vector3 target)
	{
		var pivot = vector.SetY();
		var result = target.SetY();
		
		return Vector3.Distance(pivot,result);
	}

	public static bool IsEquals(this Vector3 vector1,Vector3 vector2)
	{
		return vector1.x.Approximately(vector2.x) && vector1.y.Approximately(vector2.y) && vector1.z.Approximately(vector2.z);
	}
	
	public static bool IsZero(this Vector3 vector)
	{
		return vector.x.ApproximatelyZero() && vector.y.ApproximatelyZero() && vector.z.ApproximatelyZero();
	}

	public static Vector3 Reciprocal(this Vector3 vector)
	{
		return new Vector3(1.0f/vector.x,1.0f/vector.y,1.0f/vector.z);
	}

	public static Vector3 DistanceEach(this Vector3 vector1,Vector3 vector2)
	{
		return new Vector3(Mathf.Abs(vector1.x-vector2.x),Mathf.Abs(vector1.y-vector2.y),Mathf.Abs(vector1.z-vector2.z));
	}

	public static Vector3 Round(this Vector3 vector)
	{
		return new Vector3(Mathf.Round(vector.x),Mathf.Round(vector.y),Mathf.Round(vector.z));
	}

	public static Vector3 Ceil(this Vector3 vector)
	{
		return new Vector3(Mathf.Ceil(vector.x),Mathf.Ceil(vector.y),Mathf.Ceil(vector.z));
	}

	public static Vector3 Floor(this Vector3 vector)
	{
		return new Vector3(Mathf.Floor(vector.x),Mathf.Floor(vector.y),Mathf.Floor(vector.z));
	}

	public static Vector3 Abs(this Vector3 vector)
	{
		return new Vector3(Mathf.Abs(vector.x),Mathf.Abs(vector.y),Mathf.Abs(vector.z));
	}

	public static Vector3 Clamp(this Vector3 vector,float min,float max)
	{
		return new Vector3(Mathf.Clamp(vector.x,min,max),Mathf.Clamp(vector.y,min,max),Mathf.Clamp(vector.z,min,max));
	}

	public static Vector3 Clamp01(this Vector3 vector)
	{
		return new Vector3(Mathf.Clamp01(vector.x),Mathf.Clamp01(vector.y),Mathf.Clamp01(vector.z));
	}

	public static float MaxValue(this Vector3 vector)
	{
		return vector.x >= vector.y ? (vector.z >= vector.x ? vector.z : vector.x) : (vector.z >= vector.y ? vector.z : vector.y);
	}

	public static float MinValue(this Vector3 vector)
	{
		return vector.x <= vector.y ? (vector.z <= vector.x ? vector.z : vector.x) : (vector.z <= vector.y ? vector.z : vector.y);
	}

	public static Vector3 MultiplyEach(this Vector3 vector1,Vector3 vector2)
	{
		return new Vector3(vector1.x*vector2.x,vector1.y*vector2.y,vector1.z*vector2.z);
	}

	public static Vector3 MiddleVector(this Vector3 vector1,Vector3 vector2)
	{
		return new Vector3((vector2.x-vector1.x)/2.0f,(vector2.y-vector1.y)/2.0f,(vector2.z-vector1.z)/2.0f);
	}

	public static Color ToRGB(this Vector3 hsv)
	{
		return Color.HSVToRGB(hsv.x,hsv.y,hsv.z);
	}

	public static Color ToColor(this Vector3 vector)
	{
		return new Color(vector.x/255.0f,vector.y/255.0f,vector.z/255.0f,1.0f);
	}

	public static float ToAngle(this Vector3 vector)
	{
		return Mathf.Atan2( vector.y, vector.x ) * Mathf.Rad2Deg;
	}

	public static (Vector3 Position,int Index) CalculateGetClosestPosition(this Vector3 position,params Vector3[] positionArray)
	{
		var index = Global.INVALID_INDEX;
		var closestPosition = Vector3.zero;
		var closestDistance = float.MaxValue;

		for(var i=0;i<positionArray.Length;i++)
		{
			var distance = (position-positionArray[i]).sqrMagnitude;

			if(distance < closestDistance)
			{
				closestDistance = distance;

				index = i;
				closestPosition = positionArray[i];
			}
		}

		return (closestPosition,index);
	}

	public static (Vector3 Position,float Distance) CalculateClosestPositionOnRay(this Vector3 position,Vector3 origin,Vector3 direction)
	{
		var distance = Vector3.Dot(position-origin,direction);

		return (origin+direction*distance,distance);
	}

	// public static (Vector3 Position,float Distance) CalculateClosestPositionOnSegment(this Vector3 position,Vector3 start,Vector3 end)
	// {
	// 	var direction = end-start;
	// 	var magnitude = direction.magnitude;

	// 	direction.Normalize();

	// 	var distance = Mathf.Clamp(Vector3.Dot(position-start,direction),0.0f,magnitude);

	// 	return (start+direction*distance,distance);
	// }

	public static Vector2 CalculateAnglesTo(this Vector3 vector,Vector3 compare)
	{
		return new Vector2(-Mathf.Asin(Vector3.Cross(compare,vector).y)*Mathf.Rad2Deg,-Mathf.Asin(Vector3.Cross(compare,vector).x)*Mathf.Rad2Deg);
	}
	public static Vector3 RotateAround(this Vector3 position,Vector3 pivot,Quaternion rotation)
	{
		return rotation*(position-pivot)+pivot;
	}
	
	public static Vector3 ApplyTransform(this Vector3 vector,Transform transform)
	{
		return vector.Transform(transform.position,transform.rotation,transform.lossyScale);
	}
	
	public static Vector3 Transform(this Vector3 vector,Vector3 position,Quaternion rotation,Vector3 _scale)
	{
		vector = Vector3.Scale(vector,new Vector3(_scale.x,_scale.y,_scale.z));
		vector = vector.RotateAround(Vector3.zero,rotation);
		vector += position;

		return vector;
	}

	public static Vector3 InverseApplyTransform(this Vector3 vector,Transform transform)
	{
		return vector.InverseTransform(transform.position,transform.rotation,transform.lossyScale);
	}

	public static Vector3 InverseTransform(this Vector3 vector,Vector3 position,Quaternion rotation,Vector3 scale)
	{
		vector -= position;
		vector = vector.RotateAround(Vector3.zero,Quaternion.Inverse(rotation));
		vector = Vector3.Scale(vector,scale.Reciprocal());

		return vector;
	}

	public static Vector3 ToLocalPosition(Vector3 position,Camera worldCam,Camera uiCam,Transform relativeTo)
	{
		position = worldCam.WorldToViewportPoint(position);
		position = uiCam.ViewportToWorldPoint(position);

		if(!relativeTo || !relativeTo.parent)
		{
			return position;
		}

		return relativeTo.parent.InverseTransformPoint(position);
	}

	public static Vector3 VectorByTransform(this Vector3 vector,Transform target)
	{
		var matrix = Matrix4x4.TRS(Vector3.zero,target.rotation,Vector3.one);
		var result = matrix.MultiplyPoint3x4(vector);

		result += target.position;

		return result;
	}

	public static Vector3 SmoothDampAngle(this Vector3 vector,Vector3 target,ref Vector3 velocity,float smoothTime)
	{
		return new Vector3(Mathf.SmoothDampAngle(vector.x,target.x,ref velocity.x,smoothTime),Mathf.SmoothDampAngle(vector.y,target.y,ref velocity.y,smoothTime),Mathf.SmoothDampAngle(vector.z,target.z,ref velocity.z,smoothTime));
	}

	public static bool IsInside(this Vector3 vector,Collider collider)
	{
		return vector == collider.ClosestPoint(vector);
	}

	public static Vector3 To(this Vector3 source,Vector3 destination)
	{
		return destination-source;
	}

	public static Vector3 TransformPoint(this Vector3 position,Transform transform,SpaceType spaceType)
	{
		var (Position,Rotation,Scale) = _LockTransformToSpace(transform,spaceType);
		var resultPoint = transform.TransformPoint(position);

		transform.SetPositionAndRotation(Position,Rotation);
		transform.localScale = Scale;

		return resultPoint;
	}

	public static Vector3 InverseTransformPoint(this Vector3 _position,Transform _transform,SpaceType spaceType)
	{
		var (Position,Rotation,Scale) = _LockTransformToSpace(_transform,spaceType);
		var position = _transform.InverseTransformPoint(_position);

		_transform.SetPositionAndRotation(Position,Rotation);
		_transform.localScale = Scale;

		return position;
	}

	private static (Vector3 Position,Quaternion Rotation,Vector3 Scale) _LockTransformToSpace(Transform transform,SpaceType spaceType)
	{
		transform.GetPositionAndRotation(out var position,out var rotation);

        var scale = transform.localScale;

		if(spaceType == SpaceType.xy)
		{
			position.z = 0.0f;
		}
		else if(spaceType == SpaceType.xz)
		{
			position.y = 0.0f;
		}

		var max = Mathf.Max(scale.x,scale.y,scale.z);

		transform.SetPositionAndRotation(position,rotation);
		transform.localScale = Vector3.one*max;

		return (position,rotation,scale);
	}
}