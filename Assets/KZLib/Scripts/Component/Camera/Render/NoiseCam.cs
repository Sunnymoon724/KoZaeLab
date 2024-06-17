// using Sirenix.OdinInspector;
// using UnityEngine;

// public class NoiseCam : RenderCam
// {
// 	private const int PATTERN_STEPS = 24;

// 	[SerializeField,LabelText("노이즈 세기")]
// 	private float m_Intensity = 0.25f;

// 	[SerializeField,LabelText("노이즈 주기")]
// 	private float m_Period = 3.0f;

// 	[SerializeField,Range(1,64),LabelText("노이즈 사이즈")]
// 	private int m_NoiseSize = 4;

// 	private Texture2D m_NoiseTexture = null;

// 	protected override void Awake()
// 	{
// 		base.Awake();

// 		m_NoiseTexture = new Texture2D(m_Camera.pixelWidth/m_NoiseSize,m_Camera.pixelHeight/m_NoiseSize,TextureFormat.Alpha8,false);
// 	}

// 	void Update()
// 	{
// 		SetNoiseTexture();

// 		m_RenderMaterial.SetTexture("_Noise",m_NoiseTexture);
// 		m_RenderMaterial.SetFloat("_Intensity",CalculateIntensity()*m_Intensity);
// 	}

// 	private void SetNoiseTexture()
// 	{
// 		for(var i=0;i<m_NoiseTexture.height;i++)
// 		{
// 			for(var j=0;j<m_NoiseTexture.width;j++)
// 			{
// 				m_NoiseTexture.SetPixel(j,i,new Color(0.0f,0.0f,0.0f,(float)CommonUtility.GetRndFloat()));
// 			}
// 		}

// 		m_NoiseTexture.Apply();
// 	}	

// 	private float CalculateIntensity()
// 	{
// 		var time = Time.realtimeSinceStartup*(PATTERN_STEPS/m_Period)%PATTERN_STEPS;

// 		if(time < 4.0f || time > 6.0f && time < 8.0f)
// 		{
// 			return (Mathf.Sin(Mathf.PI*(time-0.5f))/2.0f)+0.5f;
// 		}
		
// 		if(time > 14.0f && time < 17.0f)
// 		{
// 			return Mathf.Sin(Mathf.PI/3.0f*(time+4.0f));
// 		}

// 		return 0.0f;
// 	}
// }