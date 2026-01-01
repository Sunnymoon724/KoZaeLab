using TMPro;
using UnityEngine;

namespace HudPanel
{
	public class SystemViewUI : BaseComponentUI
	{
		[SerializeField]
		private TMP_Text m_operatingSystemText = null;
		[SerializeField]
		private TMP_Text m_deviceModelText = null;
		[SerializeField]
		private TMP_Text m_memorySizeText = null;
		[SerializeField]
		private TMP_Text m_processorTypeText = null;
		[SerializeField]
		private TMP_Text m_processorCountText = null;

		[SerializeField]
		private TMP_Text m_graphicDeviceNameText = null;
		[SerializeField]
		private TMP_Text m_graphicDeviceVenderText = null;
		[SerializeField]
		private TMP_Text m_graphicDeviceVersionText = null;
		[SerializeField]
		private TMP_Text m_graphicMemorySizeText = null;
		[SerializeField]
		private TMP_Text m_graphicTextureSizeText = null;
		[SerializeField]
		private TMP_Text m_graphicShaderLevelText = null;

		[SerializeField]
		private TMP_Text m_screenResolutionText = null;
		[SerializeField]
		private TMP_Text m_windowResolutionText = null;

		protected override void Initialize()
		{
			base.Initialize();

			m_operatingSystemText.SetSafeTextMeshPro(SystemInfo.operatingSystem);
			m_deviceModelText.SetSafeTextMeshPro(SystemInfo.deviceModel);
			m_memorySizeText.SetSafeTextMeshPro($"{SystemInfo.systemMemorySize} MB");
			m_processorTypeText.SetSafeTextMeshPro(SystemInfo.processorType);
			m_processorCountText.SetSafeTextMeshPro($"{SystemInfo.processorCount} cores");

			m_graphicDeviceNameText.SetSafeTextMeshPro(SystemInfo.graphicsDeviceName);
			m_graphicDeviceVenderText.SetSafeTextMeshPro(SystemInfo.graphicsDeviceVendor);
			m_graphicDeviceVersionText.SetSafeTextMeshPro(SystemInfo.graphicsDeviceVersion);
			m_graphicMemorySizeText.SetSafeTextMeshPro($"{SystemInfo.graphicsMemorySize} MB");
			m_graphicTextureSizeText.SetSafeTextMeshPro($"{SystemInfo.maxTextureSize} px");
			m_graphicShaderLevelText.SetSafeTextMeshPro($"{SystemInfo.graphicsShaderLevel}");
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			var resolution = Screen.currentResolution;

			m_screenResolutionText.SetSafeTextMeshPro($"{resolution.width}x{resolution.height} {resolution.refreshRateRatio:F3}Hz");
			m_windowResolutionText.SetSafeTextMeshPro($"{Screen.width}x{Screen.height} {resolution.refreshRateRatio:F3}Hz {Screen.dpi}dpi");
		}
	}
}