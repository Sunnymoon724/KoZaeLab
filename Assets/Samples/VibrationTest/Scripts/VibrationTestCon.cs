using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KZLib.KZSample
{
	public class VibrationTest : MonoBehaviour
	{
		[SerializeField] Slider m_TimeSlider = null;
		[SerializeField] TMP_Text m_TimeText = null;
		[SerializeField] Slider m_IntensitySlider = null;
		[SerializeField] TMP_Text m_IntensityText = null;

		private void Start()
		{
			m_TimeSlider.onValueChanged.AddListener((value) =>
			{
				m_TimeText.SetSafeTextMeshPro(string.Format("{0}",value));
			});

			m_IntensitySlider.onValueChanged.AddListener((value) =>
			{
				m_IntensityText.SetSafeTextMeshPro(string.Format("{0}",value));
			});
		}

		public void OnClickPlay()
		{
			VibrationMgr.In.Play(m_IntensitySlider.value,m_TimeSlider.value);
		}
	}
}
