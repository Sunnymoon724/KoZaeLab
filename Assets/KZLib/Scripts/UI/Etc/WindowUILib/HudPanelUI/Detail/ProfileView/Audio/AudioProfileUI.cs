using TMPro;
using UnityEngine;

namespace HudPanel
{
	public class AudioProfileUI : BaseComponentUI
	{
		private const int c_spectrumSize = 512;

		[SerializeField]
		private TMP_Text m_decibelText = null;
		[SerializeField]
		private AudioGraphImageUI m_audioGraphImageUI = null;

		private AudioListener m_audioListener = null;

		private readonly float[] m_spectrumArray = new float[c_spectrumSize];

		private float m_maxDecibel = 0.0f;

		protected override void Initialize()
		{
			if(m_audioListener)
			{
				return;
			}

			base.Initialize();

			m_audioListener = FindAnyObjectByType<AudioListener>();
		}

		public void Refresh()
		{
			Initialize();

			AudioListener.GetOutputData(m_spectrumArray,0);

			var sample = 0.0f;

			for(var i=0;i<m_spectrumArray.Length;i++)
			{
				sample += m_spectrumArray[i]*m_spectrumArray[i];
			}

			m_maxDecibel = Mathf.Clamp(20.0f*Mathf.Log10(Mathf.Sqrt(sample/m_spectrumArray.Length)),-80.0f,0.0f);

			AudioListener.GetSpectrumData(m_spectrumArray,0,FFTWindow.Blackman);

			m_decibelText.SetSafeTextMeshPro($"{m_maxDecibel}");

			m_audioGraphImageUI.UpdateGraph(m_spectrumArray);
		}
	}
}