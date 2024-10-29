using UnityEngine;
using Sirenix.OdinInspector;
using KZLib;

public class ObserveCamera : BaseComponent
{
	[SerializeField] private float m_BackDistance = 5.0f;
	[SerializeField] private float m_height = 1.0f;
	[SerializeField] private float m_rotateSpeed = 0.5f;
	[SerializeField] private GameObject m_Target = null;

	[ShowInInspector,ReadOnly] private Vector2 m_rotate = Vector2.zero;

	private Vector2 m_MousePos	= Vector2.zero;
	
	void Start ()
	{
		m_rotate.y = transform.rotation.eulerAngles.y;
	}
	
	void Update ()
	{
		if(!m_Target)
		{
			return;
		}

		ProcessMouseKey();

		UpdatePosition();
		UpdateRotation();
	}

	private void UpdatePosition()
	{
		var target = m_Target.transform.position;

		target.y += m_height;
		transform.position = target-(transform.forward*m_BackDistance);
	}

	private void UpdateRotation()
	{
		transform.rotation = Quaternion.Euler(m_rotate.x,m_rotate.y,0.0f);
	}

	private void ProcessMouseKey()
	{
		if(Input.GetMouseButton(0))
		{
			if(m_MousePos != Vector2.zero)
			{
				float rotX	= m_MousePos.y-Input.mousePosition.y;
				float rotY	= m_MousePos.x-Input.mousePosition.x;
				
				m_rotate.x += rotX*m_rotateSpeed;
				m_rotate.y -= rotY*m_rotateSpeed;
			}

			UpdateRotation();

			m_MousePos = Input.mousePosition;
		}
		else
		{
			m_MousePos = Vector2.zero;
		}
	}
	
	public void SetTarget(GameObject _object)
	{
		m_Target = _object;
	}
}