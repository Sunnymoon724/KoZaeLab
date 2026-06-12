using Sirenix.OdinInspector;
using UnityEngine;

namespace KZLib.Utilities
{
	public class PlaneToViewFitter : MonoBehaviour
	{
		private const float c_planeMeshSize = 10.0f;

		[SerializeField,Required]
		private Camera m_targetCamera = null;
		[SerializeField,Required]
		private Transform m_targetPlane = null;

		[Space(10)]
		[SerializeField]
		private float m_cameraDistance = 0.0f;
		[SerializeField]
		private float m_cameraAngle = 0.0f;
		[SerializeField]
		private float m_viewHeight = 0.0f;

		[Button("Set Camera")]
		protected void OnSetCamera()
		{
			if(!m_targetCamera || !m_targetPlane)
			{
				return;
			}

			var radian = m_cameraAngle*Mathf.Deg2Rad;
			var sinAngle = Mathf.Sin(radian);

			m_targetCamera.orthographic = true;
			m_targetCamera.orthographicSize = m_viewHeight;

			var cameraPos = new Vector3(0.0f,Mathf.Sin(radian),Mathf.Cos(radian))*m_cameraDistance;
			m_targetCamera.transform.SetPositionAndRotation(cameraPos,Quaternion.Euler(m_cameraAngle,-180.0f,0.0f));

			var aspect = m_targetCamera.aspect;
			var planeWidth = m_viewHeight*2.0f*aspect/c_planeMeshSize;
			var planeHeight = sinAngle.ApproximatelyZero() ? 0.0f : m_viewHeight*2.0f/sinAngle/c_planeMeshSize;

			m_targetPlane.localScale = new Vector3(planeWidth,1.0f,planeHeight);
			m_targetPlane.localPosition = Vector3.zero;
		}
	}
}