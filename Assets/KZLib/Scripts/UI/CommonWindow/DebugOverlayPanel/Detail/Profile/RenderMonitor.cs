using UnityEngine;
using Unity.Profiling;
using TMPro;

namespace KZLib.UI.Widgets.Debug
{
	public class RenderMonitor : MonoBehaviour
	{
		[SerializeField]
		private TMP_Text m_nameText = null;
		[SerializeField]
		private TMP_Text m_descriptionText = null;

		private ProfilerRecorder m_profilerRecorder;
		private string m_statName = null;

		private void Awake()
		{
			m_statName = m_nameText.text;
		}

		private void OnEnable()
		{
			m_profilerRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render,m_statName);
		}

		private void OnDisable()
		{
			m_profilerRecorder.Dispose();
		}

		public void UpdateRender()
		{
			m_descriptionText.SetSafeTextMeshPro(m_profilerRecorder.Valid ? $"{m_profilerRecorder.LastValue}" : "-----");
		}
	}
}