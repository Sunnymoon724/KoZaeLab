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

		protected override void Initialize()
		{
			base.Initialize();

			m_OperatingSystemText.SetSafeTextMeshPro(SystemInfo.operatingSystem);
			m_DeviceModelText.SetSafeTextMeshPro(SystemInfo.deviceModel);

			m_ProcessorTypeText.SetSafeTextMeshPro(SystemInfo.processorType);
			m_ProcessorCountText.SetSafeTextMeshPro($"{SystemInfo.processorCount} cores");

			m_MemorySizeText.SetSafeTextMeshPro($"{SystemInfo.systemMemorySize} MB");

			m_GraphicDeviceNameText.SetSafeTextMeshPro(SystemInfo.graphicsDeviceName);
			m_GraphicDeviceVenderText.SetSafeTextMeshPro(SystemInfo.graphicsDeviceVendor);
			m_GraphicDeviceVersionText.SetSafeTextMeshPro(SystemInfo.graphicsDeviceVersion);
			m_GraphicMemorySizeText.SetSafeTextMeshPro($"{SystemInfo.graphicsMemorySize} MB");
			m_GraphicTextureSizeText.SetSafeTextMeshPro($"{SystemInfo.maxTextureSize} px");
			m_GraphicShaderLevelText.SetSafeTextMeshPro($"{SystemInfo.graphicsShaderLevel}");
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			var resolution = Screen.currentResolution;

			m_ScreenResolutionText.SetSafeTextMeshPro($"{resolution.width}x{resolution.height} {resolution.refreshRateRatio:F3}Hz");
			m_WindowResolutionText.SetSafeTextMeshPro($"{Screen.width}x{Screen.height} {resolution.refreshRateRatio:F3}Hz {Screen.dpi}dpi");
		}
	}
}