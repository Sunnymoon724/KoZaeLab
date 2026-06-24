using Sirenix.OdinInspector;
using UnityEngine;

namespace KZLib.Utilities
{
	/// <summary>
	/// Editor utility that aligns an orthographic camera and a Unity default Plane (10×10 mesh)
	/// so the plane fills the camera view at a given tilt angle and distance.
	/// Invoke via the <c>Set Camera</c> button; does not update at runtime automatically.
	/// </summary>
	public class PlaneToViewFitter : MonoBehaviour
	{
		/// <summary>Unity built-in Plane mesh edge length along X/Z.</summary>
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
		/// <summary>Orthographic half-height (<see cref="Camera.orthographicSize"/>), not full viewport height.</summary>
		[SerializeField]
		private float m_viewHeight = 0.0f;

		/// <summary>
		/// Sets camera orthographic size/position and scales the plane to cover the visible frustum.
		/// Assumes plane lies on XZ (normal +Y) and camera orbits on the YZ plane toward the plane center.
		/// </summary>
		[Button("Set Camera")]
		protected void OnSetCamera()
		{
			if(!m_targetCamera || !m_targetPlane)
			{
				return;
			}

			if(m_viewHeight.ApproximatelyZero())
			{
				return;
			}

			var radian = m_cameraAngle*Mathf.Deg2Rad;
			var sinAngle = Mathf.Sin(radian);

			m_targetCamera.orthographic = true;
			m_targetCamera.orthographicSize = m_viewHeight;

			var aspect = m_targetCamera.aspect;
			// Visible world width / default mesh width → localScale.x
			var planeWidth = m_viewHeight*2.0f*aspect/c_planeMeshSize;
			// Tilt foreshortens depth on screen; divide by sin(θ). At 0° there is no foreshortening.
			var planeHeight = sinAngle.ApproximatelyZero() ? m_viewHeight*2.0f/c_planeMeshSize : m_viewHeight*2.0f/sinAngle/c_planeMeshSize;

			m_targetPlane.localScale = new Vector3(planeWidth,1.0f,planeHeight);
			m_targetPlane.localPosition = Vector3.zero;

			// Offset on YZ plane from plane world center; rotation (θ, -180, 0) faces the plane.
			var cameraOffset = new Vector3(0.0f,Mathf.Sin(radian),Mathf.Cos(radian))*m_cameraDistance;
			var cameraPos = m_targetPlane.position + cameraOffset;

			m_targetCamera.transform.SetPositionAndRotation(cameraPos,Quaternion.Euler(m_cameraAngle,-180.0f,0.0f));
		}
	}
}