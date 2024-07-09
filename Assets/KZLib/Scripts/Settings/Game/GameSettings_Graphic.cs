using System.Collections;
using KZLib;
using Sirenix.OdinInspector;
using UnityEngine;

public partial class GameSettings : InnerBaseSettings<GameSettings>
{
	[SerializeField,HideInInspector]
	private Vector2Int m_ScreenResolution = new(Global.BASE_WIDTH,Global.BASE_HEIGHT);

	[TitleGroup("그래픽 설정",BoldTitle = false,Order = 2)]
	[BoxGroup("그래픽 설정/0",ShowLabel = false),ShowInInspector,LabelText("스크린 해상도")]
	public Vector2Int ScreenResolution { get => m_ScreenResolution; private set => m_ScreenResolution = value; }

	[SerializeField,HideInInspector]
	private bool m_IsFullScreen = true;

	[BoxGroup("그래픽 설정/0",ShowLabel = false),ShowInInspector,LabelText("스크린 최대화")]
	public bool FullScreen { get => m_IsFullScreen; private set => m_IsFullScreen = value; }

	public float ScreenAspect => ScreenResolution.x / (float) ScreenResolution.y;

	[SerializeField,HideInInspector]
	private int m_FrameRate = Global.FRAME_RATE_60;

	[BoxGroup("그래픽 설정/0",ShowLabel = false),ShowInInspector,LabelText("프레임 레이트")]
	public int FrameRate { get => m_FrameRate; private set => m_FrameRate = value; }

	[SerializeField,HideInInspector]
	private GraphicsQualityPresetType m_GraphicQualityPreset = GraphicsQualityPresetType.QualityHighest;

	[BoxGroup("그래픽 설정/0",ShowLabel = false),ShowInInspector,LabelText("그래픽 퀄리티 프리셋")]
	public GraphicsQualityPresetType GraphicQualityPreset { get => m_GraphicQualityPreset; private set => m_GraphicQualityPreset = value; }

	private void InitializeGraphic()
	{
		ScreenResolution = new Vector2Int(Global.BASE_WIDTH,Global.BASE_HEIGHT);
		FullScreen = true;
		m_GraphicQualityPreset = GraphicsQualityPresetType.QualityHighest;
		FrameRate = Global.FRAME_RATE_60;

		Screen.SetResolution(m_ScreenResolution.x,m_ScreenResolution.y,FullScreen);
		Application.targetFrameRate = m_FrameRate;
	}
}