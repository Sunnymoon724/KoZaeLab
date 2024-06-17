// using Sirenix.OdinInspector;
// using UnityEngine;

// public class ScanCam : RenderCam
// {
// 	[SerializeField,LabelText("스캔 색상")]
// 	private Color m_ScanColor = Color.white;

// 	[SerializeField,LabelText("스캔 두께"),Range(0.01f,10.0f)]
// 	private float m_ScanWidth = 0.5f;

// 	[SerializeField,LabelText("스캔 속도")]
// 	private float m_Velocity = 5.0f;

// 	[SerializeField,LabelText("최대 길이"),PropertyRange("@this.m_Camera ? this.m_Camera.nearClipPlane : 0.3f","@this.m_Camera ? this.m_Camera.farClipPlane : 1000.0f")]
// 	private float m_MaxDistance = 1000.0f;

// 	[SerializeField,DisplayAsString,LabelText("현재 거리")]
// 	private float m_CurrentDistance = 0.0f;

// 	[SerializeField,ReadOnly,LabelText("스캔 상태")]
// 	private bool m_IsScanning = false;

// 	public float CurrentDistance => m_CurrentDistance;

// 	protected override void Awake()
// 	{
// 		base.Awake();

// 		m_CurrentDistance = 0.0f;
// 		m_IsScanning = false;

// 		m_Camera.depthTextureMode = DepthTextureMode.Depth;

// 		SetData();
// 	}

// 	private void Update()
// 	{
// 		if(m_IsScanning)
// 		{
// 			m_CurrentDistance += Time.deltaTime*m_Velocity;

// 			if(m_CurrentDistance >= m_MaxDistance)
// 			{
// 				m_CurrentDistance = 0.0f;
// 				m_IsScanning = false;
// 			}

// 			m_RenderMaterial.SetFloat("_ScanDistance",m_CurrentDistance);
// 		}		
// 	}

// 	public void StartScan(Color? _color = null,float? _width = null)
// 	{
// 		m_IsScanning = true;
// 		m_CurrentDistance = 0.0f;

// 		SetData(_color,_width);
// 	}

// 	private void SetData(Color? _color = null,float? _width = null)
// 	{
// 		m_RenderMaterial.SetColor("_ScanColor",_color.HasValue ? _color.Value : m_ScanColor);
// 		m_RenderMaterial.SetFloat("_ScanWidth",_width.HasValue ? _width.Value : m_ScanWidth);
// 	}
// }