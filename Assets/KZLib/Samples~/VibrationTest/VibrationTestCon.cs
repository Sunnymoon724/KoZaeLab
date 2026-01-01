using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KZLib.KZSample
{
	public class VibrationTest : MonoBehaviour
	{
		[SerializeField]
		private Slider m_timeSlider = null;
		[SerializeField]
		private TMP_Text m_timeText = null;
		[Space(5)]
		[SerializeField]
		private Slider m_intensitySlider = null;
		[SerializeField]
		private TMP_Text m_intensityText = null;
		[Space(5)]
		[SerializeField]
		private Button m_playButton = null;

		private void Start()
		{
			m_timeSlider.onValueChanged.SetAction(_SetTimeText);

			m_intensitySlider.onValueChanged.SetAction(_SetIntensityText);

			m_playButton.onClick.SetAction(_OnClickPlay);
		}

		private void _OnClickPlay()
		{
			VibrationManager.In.Play(m_intensitySlider.value,m_timeSlider.value);
		}

		private void _SetTimeText(float value)
		{
			m_timeText.SetSafeTextMeshPro($"{value}");
		}

		private void _SetIntensityText(float value)
		{
			m_intensityText.SetSafeTextMeshPro($"{value}");
		}
	}
}
