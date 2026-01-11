using UnityEngine;
using Sirenix.OdinInspector;

public class ObserveCamera : BaseComponent
{
	[SerializeField]
	private float m_backDistance = 5.0f;
	[SerializeField]
	private float m_height = 1.0f;
	[SerializeField]
	private float m_rotateSpeed = 0.5f;
	[SerializeField]
	private GameObject m_target = null;

	[ShowInInspector,ReadOnly]
	private Vector2 m_rotate = Vector2.zero;

	private Vector3 m_mousePosition = Vector2.zero;

	protected override void _Initialize()
	{
		m_rotate.y = transform.rotation.eulerAngles.y;
	}

	private void Update()
	{
		if(!m_target)
		{
			return;
		}

		_ProcessMouseKey();

		_UpdatePosition();
		_UpdateRotation();
	}

	private void _UpdatePosition()
	{
		var target = m_target.transform.position;

		target.y += m_height;
		transform.position = target-(transform.forward*m_backDistance);
	}

	private void _UpdateRotation()
	{
		transform.rotation = Quaternion.Euler(m_rotate.x,m_rotate.y,0.0f);
	}

	private void _ProcessMouseKey()
	{
		if(Input.GetMouseButton(0))
		{
			if(m_mousePosition != Vector3.zero)
			{
				float rotX	= m_mousePosition.y-Input.mousePosition.y;
				float rotY	= m_mousePosition.x-Input.mousePosition.x;
				
				m_rotate.x += rotX*m_rotateSpeed;
				m_rotate.y -= rotY*m_rotateSpeed;
			}

			_UpdateRotation();

			m_mousePosition = Input.mousePosition;
		}
		else
		{
			m_mousePosition = Vector2.zero;
		}
	}
	
	public void SetTarget(GameObject target)
	{
		m_target = target;
	}
}