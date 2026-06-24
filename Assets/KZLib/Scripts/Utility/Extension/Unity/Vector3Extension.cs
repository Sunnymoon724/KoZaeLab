using UnityEngine;

/// <summary>
/// Extension methods for <see cref="Vector3"/> transforms, plane math, and spatial queries.
/// </summary>
public static class Vector3Extension
{
	#region Set
	/// <summary>
	/// Returns a copy with the X component replaced.
	/// </summary>
	public static Vector3 SetX(this Vector3 vector,float x = 0.0f)
	{
		return new Vector3(x,vector.y,vector.z);
	}

	/// <summary>
	/// Returns a copy with the Y component replaced.
	/// </summary>
	public static Vector3 SetY(this Vector3 vector,float y = 0.0f)
	{
		return new Vector3(vector.x,y,vector.z);
	}

	/// <summary>
	/// Returns a copy with the Z component replaced.
	/// </summary>
	public static Vector3 SetZ(this Vector3 vector,float z = 0.0f)
	{
		return new Vector3(vector.x,vector.y,z);
	}

	/// <summary>
	/// Returns a copy with the X and Y components replaced.
	/// </summary>
	public static Vector3 SetXY(this Vector3 vector,float x = 0.0f,float y = 0.0f) 
	{
		return new Vector3(x,y,vector.z);
	}

	/// <summary>
	/// Returns a copy with the X and Y components replaced from a <see cref="Vector2"/>.
	/// </summary>
	public static Vector3 SetXY(this Vector3 vector,Vector2 point)
	{
		return vector.SetXY(point.x,point.y);
	}

	/// <summary>
	/// Returns a copy with the X and Z components replaced.
	/// </summary>
	public static Vector3 SetXZ(this Vector3 vector,float x = 0.0f,float z = 0.0f) 
	{
		return new Vector3(x,vector.y,z);
	}

	/// <summary>
	/// Returns a copy with the X and Z components replaced from a <see cref="Vector2"/>.
	/// </summary>
	public static Vector3 SetXZ(this Vector3 vector,Vector2 point)
	{
		return vector.SetXZ(point.x,point.y);
	}

	/// <summary>
	/// Returns a copy with the Y and Z components replaced.
	/// </summary>
	public static Vector3 SetYZ(this Vector3 vector,float y = 0.0f,float z = 0.0f) 
	{
		return new Vector3(vector.x,y,z);
	}

	/// <summary>
	/// Returns a copy with the Y and Z components replaced from a <see cref="Vector2"/>.
	/// </summary>
	public static Vector3 SetYZ(this Vector3 vector,Vector2 point)
	{
		return vector.SetYZ(point.x,point.y);
	}
	#endregion Set

	#region Offset
	/// <summary>
	/// Returns a copy offset by the same value on all axes.
	/// </summary>
	public static Vector3 Offset(this Vector3 vector,float value)
	{
		return new Vector3(vector.x+value,vector.y+value,vector.z+value);
	}

	/// <summary>
	/// Returns a copy offset along the X and Y axes.
	/// </summary>
	public static Vector3 OffsetXY(this Vector3 vector,Vector2 offset)
	{
		return vector+(Vector3) offset;
	}

	/// <summary>
	/// Returns a copy offset along the X and Z axes.
	/// </summary>
	public static Vector3 OffsetXZ(this Vector3 vector,Vector2 offset)
	{
		return vector+new Vector3(offset.x,0.0f,offset.y);
	}

	/// <summary>
	/// Returns a copy offset along the Y and Z axes.
	/// </summary>
	public static Vector3 OffsetYZ(this Vector3 vector,Vector2 offset)
	{
		return vector+new Vector3(0.0f,offset.x,offset.y);
	}

	/// <summary>
	/// Returns a copy offset along the X axis.
	/// </summary>
	public static Vector3 OffsetX(this Vector3 vector,float x)
	{
		return vector+new Vector3(x,0.0f,0.0f);
	}

	/// <summary>
	/// Returns a copy offset along the Y axis.
	/// </summary>
	public static Vector3 OffsetY(this Vector3 vector,float y)
	{
		return vector+new Vector3(0.0f,y,0.0f);
	}

	/// <summary>
	/// Returns a copy offset along the Z axis.
	/// </summary>
	public static Vector3 OffsetZ(this Vector3 vector,float z)
	{
		return vector+new Vector3(0.0f,0.0f,z);
	}
	#endregion Offset

	#region Negate
	/// <summary>
	/// Negates all components.
	/// </summary>
	public static Vector3 Negate(this Vector3 vector)
	{
		return new Vector3(-vector.x,-vector.y,-vector.z);
	}

	/// <summary>
	/// Negates the X component.
	/// </summary>
	public static Vector3 NegateX(this Vector3 vector)
	{
		return new Vector3(-vector.x,vector.y,vector.z);
	}

	/// <summary>
	/// Negates the Y component.
	/// </summary>
	public static Vector3 NegateY(this Vector3 vector)
	{
		return new Vector3(vector.x,-vector.y,vector.z);
	}

	/// <summary>
	/// Negates the Z component.
	/// </summary>
	public static Vector3 NegateZ(this Vector3 vector)
	{
		return new Vector3(vector.x,vector.y,-vector.z);
	}

	/// <summary>
	/// Negates the X and Y components.
	/// </summary>
	public static Vector3 NegateXY(this Vector3 vector)
	{
		return new Vector3(-vector.x,-vector.y,vector.z);
	}

	/// <summary>
	/// Negates the X and Z components.
	/// </summary>
	public static Vector3 NegateXZ(this Vector3 vector)
	{
		return new Vector3(-vector.x,vector.y,-vector.z);
	}

	/// <summary>
	/// Negates the Y and Z components.
	/// </summary>
	public static Vector3 NegateYZ(this Vector3 vector)
	{
		return new Vector3(vector.x,-vector.y,-vector.z);
	}
	#endregion Negate

	#region ToVector
	/// <summary>
	/// Reduces to <see cref="Vector2"/> using X and Y.
	/// </summary>
	public static Vector2 ToVector2(this Vector3 vector)
	{
		return new(vector.x,vector.y);
	}

	/// <summary>
	/// Promotes to <see cref="Vector4"/> with optional W.
	/// </summary>
	public static Vector4 ToVector4(this Vector3 vector,float w = 0.0f)
	{
		return new(vector.x,vector.y,vector.z,w);
	}

	/// <summary>
	/// Formats the components as <c>(x, y, z)</c> with fixed decimal places.
	/// </summary>
	public static string ToVectorString(this Vector3 vector,int decimalPoint = 2)
	{
		var format = $"({"{0:f"}{decimalPoint}}}, {"{1:f"}{decimalPoint}}}, {"{2:f"}{decimalPoint}}})";

		return string.Format(format,vector.x,vector.y,vector.z);
	}
	#endregion ToVector

	/// <summary>
	/// Returns the normalized horizontal direction from this position to the target (Y flattened to zero).
	/// </summary>
	public static Vector3 PlaneDirection(this Vector3 vector,Vector3 target)
	{
		var pivot = vector.SetY();
		var result = target.SetY();
		
		return (result-pivot).normalized;
	}

	/// <summary>
	/// Returns the horizontal distance between this position and the target (Y flattened to zero).
	/// </summary>
	public static float PlaneDistance(this Vector3 vector,Vector3 target)
	{
		var pivot = vector.SetY();
		var result = target.SetY();
		
		return Vector3.Distance(pivot,result);
	}

	/// <summary>
	/// Returns whether all components are approximately equal.
	/// </summary>
	public static bool AreEqual(this Vector3 vector1,Vector3 vector2)
	{
		return vector1.x.Approximately(vector2.x) && vector1.y.Approximately(vector2.y) && vector1.z.Approximately(vector2.z);
	}
	
	/// <summary>
	/// Returns whether all components are approximately zero.
	/// </summary>
	public static bool IsZero(this Vector3 vector)
	{
		return vector.x.ApproximatelyZero() && vector.y.ApproximatelyZero() && vector.z.ApproximatelyZero();
	}

	/// <summary>
	/// Returns the component-wise reciprocal, treating near-zero components as zero.
	/// </summary>
	public static Vector3 Reciprocal(this Vector3 vector)
	{
		return new Vector3(vector.x.ApproximatelyZero() ? 0.0f : 1.0f/vector.x,vector.y.ApproximatelyZero() ? 0.0f : 1.0f/vector.y,vector.z.ApproximatelyZero() ? 0.0f : 1.0f/vector.z);
	}

	/// <summary>
	/// Returns the absolute component-wise distance to another vector.
	/// </summary>
	public static Vector3 DistanceEach(this Vector3 vector1,Vector3 vector2)
	{
		return new Vector3(Mathf.Abs(vector1.x-vector2.x),Mathf.Abs(vector1.y-vector2.y),Mathf.Abs(vector1.z-vector2.z));
	}

	/// <summary>
	/// Rounds each component to the nearest integer.
	/// </summary>
	public static Vector3 Round(this Vector3 vector)
	{
		return new Vector3(Mathf.Round(vector.x),Mathf.Round(vector.y),Mathf.Round(vector.z));
	}

	/// <summary>
	/// Applies ceiling to each component.
	/// </summary>
	public static Vector3 Ceil(this Vector3 vector)
	{
		return new Vector3(Mathf.Ceil(vector.x),Mathf.Ceil(vector.y),Mathf.Ceil(vector.z));
	}

	/// <summary>
	/// Applies floor to each component.
	/// </summary>
	public static Vector3 Floor(this Vector3 vector)
	{
		return new Vector3(Mathf.Floor(vector.x),Mathf.Floor(vector.y),Mathf.Floor(vector.z));
	}

	/// <summary>
	/// Returns the absolute value of each component.
	/// </summary>
	public static Vector3 Abs(this Vector3 vector)
	{
		return new Vector3(Mathf.Abs(vector.x),Mathf.Abs(vector.y),Mathf.Abs(vector.z));
	}

	/// <summary>
	/// Clamps each component between the given bounds.
	/// </summary>
	public static Vector3 Clamp(this Vector3 vector,float min,float max)
	{
		return new Vector3(Mathf.Clamp(vector.x,min,max),Mathf.Clamp(vector.y,min,max),Mathf.Clamp(vector.z,min,max));
	}

	/// <summary>
	/// Clamps each component to the 0–1 range.
	/// </summary>
	public static Vector3 Clamp01(this Vector3 vector)
	{
		return new Vector3(Mathf.Clamp01(vector.x),Mathf.Clamp01(vector.y),Mathf.Clamp01(vector.z));
	}

	/// <summary>
	/// Returns the largest component value.
	/// </summary>
	public static float MaxValue(this Vector3 vector)
	{
		return vector.x >= vector.y ? (vector.z >= vector.x ? vector.z : vector.x) : (vector.z >= vector.y ? vector.z : vector.y);
	}

	/// <summary>
	/// Returns the smallest component value.
	/// </summary>
	public static float MinValue(this Vector3 vector)
	{
		return vector.x <= vector.y ? (vector.z <= vector.x ? vector.z : vector.x) : (vector.z <= vector.y ? vector.z : vector.y);
	}

	/// <summary>
	/// Returns the component-wise product with another vector.
	/// </summary>
	public static Vector3 MultiplyEach(this Vector3 vector1,Vector3 vector2)
	{
		return new Vector3(vector1.x*vector2.x,vector1.y*vector2.y,vector1.z*vector2.z);
	}

	/// <summary>
	/// Returns the linear interpolation between two vectors.
	/// </summary>
	public static Vector3 Lerp(this Vector3 vector1,Vector3 vector2,float time = 0.5f)
	{
		return Vector3.Lerp(vector1,vector2,time);
	}

	/// <summary>
	/// Converts an HSV vector (hue, saturation, value) to an RGB <see cref="Color"/>.
	/// </summary>
	public static Color ToRGB(this Vector3 hsv)
	{
		return Color.HSVToRGB(hsv.x,hsv.y,hsv.z);
	}

	/// <summary>
	/// Converts byte-scaled RGB components to a fully opaque <see cref="Color"/>.
	/// </summary>
	public static Color ToColor(this Vector3 vector)
	{
		return new Color(vector.x/Global.ColorMaxValue,vector.y/Global.ColorMaxValue,vector.z/Global.ColorMaxValue,1.0f);
	}

	/// <summary>
	/// Returns the angle in degrees from the positive X axis in the XY plane.
	/// </summary>
	public static float ToAngle(this Vector3 vector)
	{
		return Mathf.Atan2(vector.y,vector.x)*Mathf.Rad2Deg;
	}

	/// <summary>
	/// Finds the nearest position in the array and returns it with its index.
	/// </summary>
	public static (Vector3 Position,int Index) CalculateGetClosestPosition(this Vector3 position,params Vector3[] positionArray)
	{
		var index = Global.InvalidIndex;
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

	/// <summary>
	/// Projects this position onto an infinite ray and returns the closest point and ray parameter.
	/// </summary>
	public static (Vector3 Position,float RayParameter) ProjectOntoRay(this Vector3 position,Vector3 origin,Vector3 direction)
	{
		var directionSquared = Vector3.Dot(direction,direction);

		if(directionSquared.ApproximatelyZero())
		{
			return (origin,0.0f);
		}

		var rayParameter = Vector3.Dot(position-origin,direction)/directionSquared;

		return (origin+direction*rayParameter,rayParameter);
	}

	/// <summary>
	/// Projects this position onto a line segment and returns the closest point and segment parameter in the 0–1 range.
	/// </summary>
	public static (Vector3 Position,float RayParameter) ProjectOntoSegment(this Vector3 position,Vector3 start,Vector3 end)
	{
		var direction = end-start;
		var directionSquared = Vector3.Dot(direction,direction);

		if(directionSquared.ApproximatelyZero())
		{
			return (start,0.0f);
		}

		var rayParameter = Mathf.Clamp01(Vector3.Dot(position-start,direction)/directionSquared);

		return (start+direction*rayParameter,rayParameter);
	}

	/// <summary>
	/// Returns pitch and yaw (degrees) of the rotation from this direction toward <paramref name="targetDirection"/>.
	/// X = pitch, Y = yaw. Angles are wrapped to -180..180.
	/// </summary>
	public static Vector2 ToPitchYawToward(this Vector3 direction,Vector3 targetDirection)
	{
		if(direction.sqrMagnitude < Mathf.Epsilon || targetDirection.sqrMagnitude < Mathf.Epsilon)
		{
			return Vector2.zero;
		}

		var rotation = Quaternion.FromToRotation(direction.normalized,targetDirection.normalized);
		var euler = rotation.eulerAngles;

		return new Vector2(euler.x.ToWrapAngle(),euler.y.ToWrapAngle());
	}
	/// <summary>
	/// Rotates this position around a pivot by the given rotation.
	/// </summary>
	public static Vector3 RotateAround(this Vector3 position,Vector3 pivot,Quaternion rotation)
	{
		return rotation*(position-pivot)+pivot;
	}
	
	/// <summary>
	/// Applies the transform's position, rotation, and scale to this vector.
	/// </summary>
	public static Vector3 ApplyTransform(this Vector3 vector,Transform transform)
	{
		return vector.Transform(transform.position,transform.rotation,transform.lossyScale);
	}
	
	/// <summary>
	/// Applies scale, rotation, and translation to this local-space vector.
	/// </summary>
	public static Vector3 Transform(this Vector3 vector,Vector3 position,Quaternion rotation,Vector3 scale)
	{
		vector = Vector3.Scale(vector,new Vector3(scale.x,scale.y,scale.z));
		vector = vector.RotateAround(Vector3.zero,rotation);
		vector += position;

		return vector;
	}

	/// <summary>
	/// Reverses the transform's position, rotation, and scale on this vector.
	/// </summary>
	public static Vector3 InverseApplyTransform(this Vector3 vector,Transform transform)
	{
		return vector.InverseTransform(transform.position,transform.rotation,transform.lossyScale);
	}

	/// <summary>
	/// Reverses scale, rotation, and translation applied by <see cref="Transform"/>.
	/// </summary>
	public static Vector3 InverseTransform(this Vector3 vector,Vector3 position,Quaternion rotation,Vector3 scale)
	{
		vector -= position;
		vector = vector.RotateAround(Vector3.zero,Quaternion.Inverse(rotation));
		vector = Vector3.Scale(vector,scale.Reciprocal());

		return vector;
	}

	/// <summary>
	/// Converts a world position from one camera space to local space relative to a UI transform.
	/// </summary>
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

	/// <summary>
	/// Transforms a point by the target's rotation and world position.
	/// </summary>
	public static Vector3 TransformPointBy(this Vector3 vector,Transform target)
	{
		var matrix = Matrix4x4.TRS(Vector3.zero,target.rotation,Vector3.one);
		var result = matrix.MultiplyPoint3x4(vector);

		result += target.position;

		return result;
	}

	/// <summary>
	/// Smoothly damps each Euler angle component toward the target angles.
	/// </summary>
	public static Vector3 SmoothDampAngle(this Vector3 vector,Vector3 target,ref Vector3 velocity,float smoothTime)
	{
		return new Vector3(Mathf.SmoothDampAngle(vector.x,target.x,ref velocity.x,smoothTime),Mathf.SmoothDampAngle(vector.y,target.y,ref velocity.y,smoothTime),Mathf.SmoothDampAngle(vector.z,target.z,ref velocity.z,smoothTime));
	}

	/// <summary>
	/// Returns whether this point lies inside the collider (not merely within its bounds).
	/// </summary>
	public static bool IsInside(this Vector3 vector,Collider collider)
	{
		if(!collider.bounds.Contains(vector))
		{
			return false;
		}

		return vector == collider.ClosestPoint(vector);
	}

	/// <summary>
	/// Returns the displacement from source to destination.
	/// </summary>
	public static Vector3 To(this Vector3 source,Vector3 destination)
	{
		return destination-source;
	}

	/// <summary>
	/// Transforms a local point using a space matrix that may flatten an axis or unify scale.
	/// </summary>
	public static Vector3 TransformPoint(this Vector3 position,Transform transform,SpaceType spaceType)
	{
		return _BuildSpaceMatrix(transform,spaceType).MultiplyPoint3x4(position);
	}

	/// <summary>
	/// Inverse-transforms a world point using a space matrix that may flatten an axis or unify scale.
	/// </summary>
	public static Vector3 InverseTransformPoint(this Vector3 position,Transform transform,SpaceType spaceType)
	{
		return _BuildSpaceMatrix(transform,spaceType).inverse.MultiplyPoint3x4(position);
	}

	private static Matrix4x4 _BuildSpaceMatrix(Transform transform,SpaceType spaceType)
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

		return Matrix4x4.TRS(position,rotation,Vector3.one*max);
	}
}