using UnityEngine;
using Unity.Profiling;
using TMPro;

namespace KZLib.KZWidget.Debug
{
	public class RenderMonitor : BaseComponentUI
	{
		[SerializeField]
		private TMP_Text m_nameText = null;
		[SerializeField]
		private TMP_Text m_descriptionText = null;

		private ProfilerRecorder m_profilerRecorder;
		private string m_statName = null;

		protected override void Initialize()
		{
			m_statName = m_nameText.text;
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			m_profilerRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render,m_statName);
		}

		protected override void OnDisable()
		{
			base.OnDisable();

			m_profilerRecorder.Dispose();
		}

		public void UpdateRender()
		{
			m_descriptionText.SetSafeTextMeshPro(m_profilerRecorder.Valid ? $"{m_profilerRecorder.LastValue}" : "-----");
		}
	}
}