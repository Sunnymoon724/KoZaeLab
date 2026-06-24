using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Third-person orbit camera. Keeps a fixed back distance and height from a target; left mouse drag rotates the view.
/// World / main camera behaviour; does not register with <see cref="KZLib.CameraManager"/> as a sub-camera.
/// </summary>
/// <remarks>
/// Mouse drag only (PC / editor). Mobile orbit is out of scope unless a touch path is added later.
/// Each frame: mouse input → pitch clamp → apply rotation → place camera behind the look direction.
/// </remarks>
[RequireComponent(typeof(Camera))]
public class ObserveCamera : BaseComponent
{
	private const float c_minPitch = -85.0f;
	private const float c_maxPitch = 85.0f;
	private const float c_referenceScreenHeight = 1080.0f;

	/// <summary>Distance behind the look direction from the orbit pivot.</summary>
	[SerializeField]
	private float m_backDistance = 5.0f;

	/// <summary>World-space Y offset added to the target position for the orbit pivot.</summary>
	[SerializeField]
	private float m_height = 1.0f;

	/// <summary>Mouse drag sensitivity. Scaled by <see cref="c_referenceScreenHeight"/> / screen height.</summary>
	[SerializeField]
	private float m_rotateSpeed = 0.5f;

	[SerializeField]
	private GameObject m_target = null;

	/// <summary>Orbit angles in degrees: x = pitch, y = yaw.</summary>
	private Vector2 m_rotate = Vector2.zero;

	/// <summary>Previous mouse position while dragging; cleared on release to avoid a rotation jump.</summary>
	private Vector2? m_previousMousePosition = null;

	protected override void _Initialize()
	{
		base._Initialize();

		_SyncRotationFromTransform();
	}

	/// <summary>Runs after target movement so orbit position does not lag one frame behind.</summary>
	private void LateUpdate()
	{
		if(!m_target || m_backDistance <= 0.0f)
		{
			return;
		}

		_ProcessMouseInput();
		_ClampPitch();
		_UpdateRotation();
		_UpdatePosition();
	}

	/// <summary>Places the camera behind the pivot along the current forward vector.</summary>
	private void _UpdatePosition()
	{
		var targetPosition = m_target.transform.position;
		targetPosition.y += m_height;
		transform.position = targetPosition-(transform.forward*m_backDistance);
	}

	private void _UpdateRotation()
	{
		transform.rotation = Quaternion.Euler(m_rotate.x,m_rotate.y,0.0f);
	}

	/// <summary>Accumulates pitch/yaw from left mouse drag via <see cref="Mouse"/>.</summary>
	private void _ProcessMouseInput()
	{
		var mouse = Mouse.current;

		if(mouse == null || !mouse.leftButton.isPressed)
		{
			m_previousMousePosition = null;

			return;
		}

		var mousePosition = mouse.position.ReadValue();

		if(m_previousMousePosition.HasValue)
		{
			var delta = mousePosition-m_previousMousePosition.Value;
			var sensitivity = c_referenceScreenHeight/Mathf.Max(Screen.height,1.0f);

			m_rotate.x += (m_previousMousePosition.Value.y-mousePosition.y)*m_rotateSpeed*sensitivity;
			m_rotate.y -= delta.x*m_rotateSpeed*sensitivity;
		}

		m_previousMousePosition = mousePosition;
	}

	private void _ClampPitch()
	{
		m_rotate.x = Mathf.Clamp(m_rotate.x,c_minPitch,c_maxPitch);
	}

	/// <summary>Assigns a new target, clears drag state, and syncs rotation from the current transform.</summary>
	public void SetTarget(GameObject target)
	{
		m_target = target;
		m_previousMousePosition = null;

		if(m_target)
		{
			_SyncRotationFromTransform();
		}
	}

	/// <summary>Copies the current transform euler angles into <see cref="m_rotate"/> (pitch normalized to [-180, 180]).</summary>
	private void _SyncRotationFromTransform()
	{
		var eulerAngles = transform.rotation.eulerAngles;

		m_rotate.x = eulerAngles.x > 180.0f ? eulerAngles.x-360.0f : eulerAngles.x;
		m_rotate.y = eulerAngles.y;
	}
}