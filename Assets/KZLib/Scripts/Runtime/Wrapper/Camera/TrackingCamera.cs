using UnityEngine;

/// <summary>
/// Follows a target while preserving the world-space offset captured when the target is assigned.
/// World / main camera behaviour; does not register with <see cref="KZLib.CameraManager"/> as a sub-camera.
/// </summary>
/// <remarks>
/// Offset is taken from <c>transform.position - target.position</c> on play start (<see cref="_Initialize"/>),
/// <see cref="SetTarget"/>, or <see cref="RecalculateOffset"/>. Call <see cref="RecalculateOffset"/> after teleporting this camera without changing the target.
/// </remarks>
[RequireComponent(typeof(Camera))]
public class TrackingCamera : BaseComponent
{
	/// <summary>When true, snaps to the follow position every frame with no smoothing.</summary>
	[SerializeField]
	private bool m_isImmediate = false;

	/// <summary>Exponential follow strength. Higher values converge faster. Ignored when <see cref="m_isImmediate"/> is true.</summary>
	[SerializeField]
	private float m_followSpeed = 5.0f;

	[SerializeField]
	private GameObject m_target = null;

	/// <summary>World offset from the target maintained while following.</summary>
	private Vector3 m_offset = Vector3.zero;

	protected override void _Initialize()
	{
		base._Initialize();

		_RefreshOffset();
	}

	/// <summary>Runs after target movement so the camera does not lag one frame behind.</summary>
	private void LateUpdate()
	{
		if(!m_target)
		{
			return;
		}

		_UpdatePosition();
	}

	private void _UpdatePosition()
	{
		var newPosition = m_target.transform.position+m_offset;

		if(m_isImmediate)
		{
			transform.position = newPosition;

			return;
		}

		var blend = 1.0f-Mathf.Exp(-Mathf.Max(0.0f,m_followSpeed)*Time.deltaTime);
		transform.position = Vector3.Lerp(transform.position,newPosition,blend);
	}

	/// <summary>Assigns a new target and recalculates offset from the current transform position.</summary>
	public void SetTarget(GameObject target)
	{
		m_target = target;
		_RefreshOffset();
	}

	/// <summary>Recomputes offset from the current transform and target positions without changing the target.</summary>
	public void RecalculateOffset()
	{
		_RefreshOffset();
	}

	private void _RefreshOffset()
	{
		if(m_target)
		{
			m_offset = transform.position-m_target.transform.position;
		}
		else
		{
			m_offset = Vector3.zero;
		}
	}
}