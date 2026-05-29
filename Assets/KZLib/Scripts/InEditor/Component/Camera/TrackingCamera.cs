using Sirenix.OdinInspector;
using UnityEngine;

public class TrackingCamera : MonoBehaviour
{
	[SerializeField]
	private bool m_isImmediate = false;
	[SerializeField]
	private float m_followSpeed = 5.0f;

	[SerializeField,HideInInspector]
	private GameObject m_target = null;


	[ShowInInspector]
	private GameObject Target
	{
		get => m_target;
		set
		{
			m_target = value;

			if(m_target)
			{
				m_offset = transform.position-m_target.transform.position;
			}
		}
	}

	private Vector3 m_offset = Vector3.zero;

	private void Update()
	{
		if(!Target)
		{
			return;
		}

		_UpdatePosition();
	}

	private void _UpdatePosition()
	{
		var newPosition = Target.transform.position+m_offset;

		if(m_isImmediate)
		{
			transform.position = newPosition;
		}
		else
		{
			transform.position = Vector3.Lerp(transform.position,newPosition,m_followSpeed*Time.deltaTime);
		}
	}

	public void SetTarget(GameObject target)
	{
		Target = target;
	}
}