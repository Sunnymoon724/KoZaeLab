using UnityEngine;
using TMPro;

public class ScanCamCon : MonoBehaviour
{
	[SerializeField]
	private ScanCam m_ScanCam = null;

	[SerializeField]
	private TMP_Text m_DistanceTextMesh = null;

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.Space))
		{
			m_ScanCam.StartScan();
		}

		m_DistanceTextMesh.SetSafeTextMeshPro(string.Format("Distance : {0:0000.000}",m_ScanCam.CurrentDistance));
	}
}