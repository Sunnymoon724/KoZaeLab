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
				m_TimeText.SetSafeTextMeshPro($"{value}");
			});

			m_IntensitySlider.onValueChanged.AddListener((value) =>
			{
				m_IntensityText.SetSafeTextMeshPro($"{value}");
			});
		}

		public void OnClickPlay()
		{
			VibrationManager.In.Play(m_IntensitySlider.value,m_TimeSlider.value);
		}
	}
}
