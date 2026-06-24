using UnityEngine;

/// <summary>
/// Extension methods for <see cref="Rigidbody"/> sweep overlap detection and velocity redirection.
/// </summary>
public static class RigidbodyExtension
{
	/// <summary>
	/// Sweeps the rigidbody along an offset vector and returns whether a collider on the layer mask was hit.
	/// </summary>
	public static bool IsOverlapping(this Rigidbody rigidbody,LayerMask layerMask,Vector3 offset)
	{
		return IsOverlapping(rigidbody,layerMask,offset,QueryTriggerInteraction.Ignore);
	}

	/// <summary>
	/// Sweeps the rigidbody along an offset vector with the given trigger interaction mode.
	/// </summary>
	public static bool IsOverlapping(this Rigidbody rigidbody,LayerMask layerMask,Vector3 offset,QueryTriggerInteraction triggerInteraction)
	{
		return IsOverlapping(rigidbody,layerMask,offset,triggerInteraction,out _);
	}

	/// <summary>
	/// Sweeps the rigidbody along an offset vector and outputs the first hit when found.
	/// </summary>
	public static bool IsOverlapping(this Rigidbody rigidbody,LayerMask layerMask,Vector3 offset,out RaycastHit raycastHit)
	{
		return IsOverlapping(rigidbody,layerMask,offset,QueryTriggerInteraction.Ignore,out raycastHit);
	}

	/// <summary>
	/// Sweeps the rigidbody along an offset vector and returns whether a collider on the layer mask was hit.
	/// </summary>
	public static bool IsOverlapping(this Rigidbody rigidbody,LayerMask layerMask,Vector3 offset,QueryTriggerInteraction triggerInteraction,out RaycastHit raycastHit)
	{
		raycastHit = default;

		if(!_IsValid(rigidbody))
		{
			return false;
		}

		var distance = offset.magnitude;

		if(distance.ApproximatelyZero())
		{
			return false;
		}

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

	/// <summary>
	/// Redirects linear velocity to the given direction while preserving speed.
	/// </summary>
	public static void ChangeDirection(this Rigidbody rigidbody,Vector3 direction)
	{
		if(!_IsValid(rigidbody))
		{
			return;
		}

		rigidbody.linearVelocity = direction*rigidbody.linearVelocity.magnitude;
	}

	private static bool _IsValid(Rigidbody rigidbody)
	{
		if(!rigidbody)
		{
			LogChannel.Kit.E("Rigidbody is null.");

			return false;
		}

		return true;
	}
}