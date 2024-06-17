// using Sirenix.OdinInspector;
// using UnityEngine;

// public class MotionBlurCam : RenderCam
// {
// 	private enum SAMPLE_TYPE { Six = 6, Eight = 8, Ten = 10, };

// 	private const string EIGHT_KEYWORD = "EIGHT";
// 	private const string TEN_KEYWORD = "TEN";

// 	[SerializeField,LabelText("모션블러 거리"),Range(0.0f,1.0f)]
// 	private float m_Distance = 0.0f;

// 	[SerializeField,LabelText("모션블러 필터"),Range(1.0f,5.0f)]
// 	private int m_FastFilter = 4;

// 	[SerializeField,LabelText("샘플 갯수")]
// 	private SAMPLE_TYPE m_SampleCount = SAMPLE_TYPE.Six;

// 	private RenderTextureDescriptor m_Descriptor;
// 	private Matrix4x4 m_PreviousViewProjection;
// 	private Matrix4x4 m_ViewProjection;

// 	private void Start()
// 	{
// 		m_PreviousViewProjection = m_Camera.projectionMatrix*m_Camera.worldToCameraMatrix;
// 	}

// 	protected override void OnRenderImage(RenderTexture _source,RenderTexture _destination)
// 	{
// 		m_Descriptor = _source.descriptor;
// 		m_Descriptor.width = Screen.width/m_FastFilter;
// 		m_Descriptor.height = Screen.height/m_FastFilter;
// 		m_Descriptor.depthBufferBits = 0;

// 		switch(m_SampleCount)
// 		{
// 			case SAMPLE_TYPE.Six:
// 			{
// 				m_RenderMaterial.DisableKeyword(EIGHT_KEYWORD);
// 				m_RenderMaterial.DisableKeyword(TEN_KEYWORD);
// 				break;
// 			}

// 			case SAMPLE_TYPE.Eight:
// 			{
// 				m_RenderMaterial.EnableKeyword(EIGHT_KEYWORD);
// 				m_RenderMaterial.DisableKeyword(TEN_KEYWORD);
// 				break;
// 			}
// 			case SAMPLE_TYPE.Ten:
// 			{
// 				m_RenderMaterial.EnableKeyword(EIGHT_KEYWORD);
// 				m_RenderMaterial.EnableKeyword(TEN_KEYWORD);
// 				break;
// 			}
// 		}

// 		m_ViewProjection = m_Camera.projectionMatrix*m_Camera.worldToCameraMatrix;

// 		if(m_ViewProjection == m_PreviousViewProjection)
// 		{
// 			Graphics.Blit(_source, _destination);
// 			return;
// 		}

// 		m_RenderMaterial.SetMatrix("_ViewProjectionMatrix",m_PreviousViewProjection*m_ViewProjection.inverse);
// 		m_RenderMaterial.SetFloat("_Distance",1.0f-m_Distance);

// 		m_PreviousViewProjection = m_ViewProjection;

// 		var render = RenderTexture.GetTemporary(m_Descriptor);

// 		Graphics.Blit(_source,render,m_RenderMaterial,0);

// 		m_RenderMaterial.SetTexture("_BlurTex",render);

// 		Graphics.Blit(_source,_destination,m_RenderMaterial,1);

// 		RenderTexture.ReleaseTemporary(render);
// 	}
// }