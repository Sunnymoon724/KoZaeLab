using KZLib;
using TMPro;
using UnityEngine;

namespace HudPanel
{
	public class AudioProfile : BaseComponentUI
	{
		private const int SPECTRUM_SIZE = 512;

		[SerializeField] private TMP_Text m_DecibelText = null;
		[SerializeField] private AudioGraph m_AudioGraph = null;

		private AudioListener m_AudioListener = null;

		private readonly float[] m_SpectrumArray = new float[SPECTRUM_SIZE];

		private float m_MaxDecibel = 0.0f;

		private bool m_Initialize = false;

		protected override void Awake()
		{
			base.Awake();

			Initialize();
		}

		private void Initialize()
		{
			if(m_Initialize)
			{
				return;
			}

			var camera = CameraMgr.HasInstance ? CameraMgr.In.MainCamera : Camera.main;

			m_AudioListener = camera != null ? camera.GetComponent<AudioListener>() : FindObjectOfType<AudioListener>();

			m_Initialize = true;
		}

		public void Refresh()
		{
			if(m_AudioListener == null)
			{
				Initialize();

				return;
			}

			AudioListener.GetOutputData(m_SpectrumArray,0);

			var sample = 0.0f;

			for(var i=0;i<m_SpectrumArray.Length;i++)
			{
				sample += m_SpectrumArray[i]*m_SpectrumArray[i];
			}

			m_MaxDecibel = Mathf.Clamp(20.0f*Mathf.Log10(Mathf.Sqrt(sample/m_SpectrumArray.Length)),-80.0f,0.0f);

			AudioListener.GetSpectrumData(m_SpectrumArray,0,FFTWindow.Blackman);

			m_DecibelText.SetSafeTextMeshPro(string.Format("{0}",m_MaxDecibel));

			m_AudioGraph.UpdateGraph(m_SpectrumArray);
		}
	}
}