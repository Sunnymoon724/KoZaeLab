using UnityEngine;

public static partial class TransformExtension
{
	public static void ResetTransform(this Transform transform,Transform parent = null)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		if(parent)
		{
			transform.SetParent(parent,true);
		}

		transform.SetLocalPositionAndRotation(Vector3.zero,Quaternion.identity);
		transform.localScale = Vector3.one;
	}

	public static bool IsFront(this Transform transform,Vector3 target)
	{
		if(!_IsValid(transform))
		{
			return false;
		}

		return Vector3.Dot(transform.forward,target-transform.position) >= 0.0f;
	}

	public static bool IsRight(this Transform transform,Vector3 target)
	{
		if(!_IsValid(transform))
		{
			return false;
		}

		return Vector3.Cross(transform.forward,target-transform.position).y >= 0.0f;
	}

	public static void LookAtSlowly(this Transform transform,Transform target,float speed = 1.0f,bool isHoldX = false,bool isHoldY = false,bool isHoldZ = false)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		if(!target)
		{
			LogSvc.System.E("Target is null.");

			return;
		}

		var direction = target.position-transform.position;

		direction.x = isHoldX ? 0.0f : direction.x;
		direction.y = isHoldY ? 0.0f : direction.y;
		direction.z = isHoldZ ? 0.0f : direction.z;

		var rotation = Quaternion.LookRotation(direction);

		transform.rotation = Quaternion.Slerp(transform.rotation,rotation,Time.deltaTime*speed);
	}

	public static bool IsInside(this Transform transform,Collider collider)
	{
		if(!_IsValid(transform))
		{
			return false;
		}

		if(!collider)
		{
			LogSvc.System.E("Collider is null.");

			return false;
		}

		return transform.position.IsInside(collider);
	}

	public static bool IsNearlyFacingTowards(this Transform transform,Vector3 targetPosition,float cosineThreshold = 0.95f,bool ignoreHeight = false)
	{
		if(!_IsValid(transform))
		{
			return false;
		}

		var currentPosition = transform.position;

		if(ignoreHeight)
		{
			targetPosition = new Vector3(targetPosition.x,currentPosition.y,targetPosition.z);
		}

		var directionToTarget = targetPosition - currentPosition;

		return Vector3.Dot(transform.forward,directionToTarget.normalized) >= cosineThreshold;
	}

	public static void LookAt2D(this Transform transform,Vector3 target)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		transform.rotation = Quaternion.LookRotation(Vector3.forward,-(target-transform.position));
		transform.Rotate(Vector3.forward,-90.0f);
	}

	private static bool _IsValid(Transform transform)
	{
		if(!transform)
		{
			LogSvc.System.E("Transform is null");

			return false;
		}

		return true;
	}
}