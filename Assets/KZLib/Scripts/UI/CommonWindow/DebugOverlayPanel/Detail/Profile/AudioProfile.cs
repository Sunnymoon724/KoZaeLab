using TMPro;
using UnityEngine;

namespace KZLib.UI.Widgets.Debug
{
	public class AudioProfile : ImmediateOverlayBehaviour
	{
		private const int c_spectrumSize = 512;
		private const float c_dbScale = 20.0f;
		private const float c_minDecibel = -80.0f;

		[SerializeField]
		private TMP_Text m_decibelText = null;
		[SerializeField]
		private AudioGraphImage m_audioGraphImageUI = null;

		private readonly float[] m_spectrumArray = new float[c_spectrumSize];
		private float m_maxDecibel = 0.0f;
		private AudioListener m_currentListener = null;

		private AudioListener CurrentListener
		{
			get
			{
				if(!m_currentListener)
				{
					m_currentListener = FindAnyObjectByType<AudioListener>();
				}

				return m_currentListener;
			}
		}

		public override void Refresh()
		{
			var listener = CurrentListener;

			if(!listener)
			{
				return;
			}

			AudioListener.GetOutputData(m_spectrumArray,0);

			var sample = 0.0f;

			for(var i=0;i<m_spectrumArray.Length;i++)
			{
				sample += m_spectrumArray[i]*m_spectrumArray[i];
			}

			m_maxDecibel = Mathf.Clamp(c_dbScale*Mathf.Log10(Mathf.Sqrt(sample/m_spectrumArray.Length)),c_minDecibel,0.0f);

			AudioListener.GetSpectrumData(m_spectrumArray,0,FFTWindow.Blackman);

			m_decibelText.SetSafeTextMeshPro($"{m_maxDecibel}");

			m_audioGraphImageUI.RefreshGraph(m_spectrumArray);
		}
	}
}