using UnityEngine;

public static class RigidbodyExtension
{
	public static bool IsOverlapping(this Rigidbody rigidbody,LayerMask layerMask,Vector3 offset)
	{
		return IsOverlapping(rigidbody,layerMask,offset,QueryTriggerInteraction.Ignore);
	}

	public static bool IsOverlapping(this Rigidbody rigidbody,LayerMask layerMask,Vector3 offset,QueryTriggerInteraction triggerInteraction)
	{
		return IsOverlapping(rigidbody,layerMask,offset,triggerInteraction,out _);
	}

	public static bool IsOverlapping(this Rigidbody rigidbody,LayerMask layerMask,Vector3 offset,out RaycastHit raycastHit)
	{
		return IsOverlapping(rigidbody,layerMask,offset,QueryTriggerInteraction.Ignore,out raycastHit);
	}

	public static bool IsOverlapping(this Rigidbody rigidbody,LayerMask layerMask,Vector3 offset,QueryTriggerInteraction triggerInteraction,out RaycastHit raycastHit)
	{
		var distance = offset.magnitude;
		var direction = offset.normalized;

		if(rigidbody.SweepTest(direction,out raycastHit,distance,triggerInteraction))
		{
			var hitLayer = raycastHit.collider.gameObject.layer;

			if(layerMask.Contains(hitLayer))
			{
				return true;
			}
		}

		raycastHit = default;
		return false;
	}

	public static void ChangeDirection(this Rigidbody rigidbody,Vector3 direction)
	{
		rigidbody.linearVelocity = direction*rigidbody.linearVelocity.magnitude;
	}
}