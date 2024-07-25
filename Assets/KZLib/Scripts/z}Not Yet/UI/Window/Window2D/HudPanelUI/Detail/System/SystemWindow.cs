using TMPro;
using UnityEngine;

namespace HudPanel
{
	public class SystemWindow : BaseComponentUI
	{
		[SerializeField] private TMP_Text m_OperatingSystemText = null;
		[SerializeField] private TMP_Text m_DeviceModelText = null;
		[SerializeField] private TMP_Text m_ProcessorTypeText = null;
		[SerializeField] private TMP_Text m_ProcessorCountText = null;
		[SerializeField] private TMP_Text m_MemorySizeText = null;

		[SerializeField] private TMP_Text m_GraphicDeviceNameText = null;
		[SerializeField] private TMP_Text m_GraphicDeviceVenderText = null;
		[SerializeField] private TMP_Text m_GraphicDeviceVersionText = null;
		[SerializeField] private TMP_Text m_GraphicMemorySizeText = null;
		[SerializeField] private TMP_Text m_GraphicTextureSizeText = null;
		[SerializeField] private TMP_Text m_GraphicShaderLevelText = null;

		[SerializeField] private TMP_Text m_ScreenResolutionText = null;
		[SerializeField] private TMP_Text m_WindowResolutionText = null;

		private void OnEnable()
		{
			m_OperatingSystemText.SetSafeTextMeshPro(SystemInfo.operatingSystem);
			m_DeviceModelText.SetSafeTextMeshPro(SystemInfo.deviceModel);

			m_ProcessorTypeText.SetSafeTextMeshPro(SystemInfo.processorType);
			m_ProcessorCountText.SetSafeTextMeshPro(string.Format("{0} cores",SystemInfo.processorCount));

			m_MemorySizeText.SetSafeTextMeshPro(string.Format("{0} MB",SystemInfo.systemMemorySize));

			m_GraphicDeviceNameText.SetSafeTextMeshPro(SystemInfo.graphicsDeviceName);
			m_GraphicDeviceVenderText.SetSafeTextMeshPro(SystemInfo.graphicsDeviceVendor);
			m_GraphicDeviceVersionText.SetSafeTextMeshPro(SystemInfo.graphicsDeviceVersion);
			m_GraphicMemorySizeText.SetSafeTextMeshPro(string.Format("{0} MB",SystemInfo.graphicsMemorySize));
			m_GraphicTextureSizeText.SetSafeTextMeshPro(string.Format("{0} px",SystemInfo.maxTextureSize));
			m_GraphicShaderLevelText.SetSafeTextMeshPro(string.Format("{0}",SystemInfo.graphicsShaderLevel));

			var resolution = Screen.currentResolution;

			m_ScreenResolutionText.SetSafeTextMeshPro(string.Format("{0}x{1} {2:F3}Hz",resolution.width,resolution.height,resolution.refreshRateRatio));
			m_WindowResolutionText.SetSafeTextMeshPro(string.Format("{0}x{1} {2:F3}Hz {3}dpi",Screen.width,Screen.height,resolution.refreshRateRatio,Screen.dpi));
		}
	}
}