using UnityEngine;
using Unity.Profiling;
using TMPro;

namespace HudPanel
{
	public class RenderMonitor : BaseComponentUI
	{
		[SerializeField]
		private TMP_Text m_InfoText = null;

		[SerializeField]
		private string m_StatName = null;

		private ProfilerRecorder m_Recorder;

		protected override void OnEnable()
		{
			base.OnEnable();

			m_Recorder = ProfilerRecorder.StartNew(ProfilerCategory.Render,m_StatName);
		}

		protected override void OnDisable()
		{
			base.OnDisable();

			m_Recorder.Dispose();
		}

		public void UpdateRender()
		{
			m_InfoText.SetSafeTextMeshPro(m_Recorder.Valid ? $"{m_Recorder.LastValue}" : "-----");
		}
	}
}