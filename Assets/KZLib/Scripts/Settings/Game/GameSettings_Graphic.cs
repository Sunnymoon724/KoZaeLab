using System.Collections;
using KZLib;
using Sirenix.OdinInspector;
using UnityEngine;

public partial class GameSettings : InnerBaseSettings<GameSettings>
{
	[SerializeField,HideInInspector]
	private Vector2Int m_ScreenResolution = new(Global.BASE_WIDTH,Global.BASE_HEIGHT);

	[TabGroup("게임 설정","프로젝트 설정",Order = -10)]
	[TitleGroup("게임 설정/프로젝트 설정/그래픽 설정",BoldTitle = false,Order = GRAPHIC_ORDER)]
	[BoxGroup("게임 설정/프로젝트 설정/그래픽 설정/0",ShowLabel = false),ShowInInspector,LabelText("스크린 해상도")]
	public Vector2Int ScreenResolution { get => m_ScreenResolution; private set => m_ScreenResolution = value; }

	[SerializeField,HideInInspector]
	private bool m_IsFullScreen = true;

	[BoxGroup("게임 설정/프로젝트 설정/그래픽 설정/0",ShowLabel = false),ShowInInspector,LabelText("스크린 최대화")]
	public bool FullScreen { get => m_IsFullScreen; private set => m_IsFullScreen = value; }

	public float ScreenAspect => ScreenResolution.x / (float) ScreenResolution.y;

	[SerializeField,HideInInspector]
	private int m_FrameRate = Global.FRAME_RATE_60;

	[BoxGroup("게임 설정/프로젝트 설정/그래픽 설정/0",ShowLabel = false),ShowInInspector,LabelText("프레임 레이트")]
	public int FrameRate { get => m_FrameRate; private set => m_FrameRate = value; }

	[SerializeField,HideInInspector]
	private long m_GraphicQuality = GraphicsOptionPreset.QUALITY_HIGH;

	[BoxGroup("게임 설정/프로젝트 설정/그래픽 설정/0",ShowLabel = false),ShowInInspector,LabelText("그래픽 퀄리티"),ValueDropdown(nameof(GraphicPresetList))]
	public long GraphicQuality { get => m_GraphicQuality; private set => m_GraphicQuality = value; }

	private void InitializeGraphic()
	{
		ScreenResolution = new Vector2Int(Global.BASE_WIDTH,Global.BASE_HEIGHT);
		FullScreen = true;
		GraphicQuality = GraphicsOptionPreset.QUALITY_HIGH;
		FrameRate = Global.FRAME_RATE_60;

		Screen.SetResolution(m_ScreenResolution.x,m_ScreenResolution.y,FullScreen);
		Application.targetFrameRate = m_FrameRate;
	}

	private static IEnumerable GraphicPresetList
	{
		get
		{
			return new ValueDropdownList<long>()
			{
				{ "QUALITY_LOW", GraphicsOptionPreset.QUALITY_LOW },
				{ "QUALITY_MIDDLE", GraphicsOptionPreset.QUALITY_MIDDLE },
				{ "QUALITY_HIGH", GraphicsOptionPreset.QUALITY_HIGH },
			};
		}
	}

	private class GraphicsOptionPreset
	{
		public static long QUALITY_LOW							= PostEffectQualityOption.POST_EFFECT_QUALITY_LOW
																| SceneGraphicQualityOption.SCENE_GRAPHICS_QUALITY_LOW
																| EffectDisplayQualityOption.EFFECT_DISPLAY_QUALITY_LOW
																| TextureQualityOption.TEXTURE_QUALITY_LOW;

		public static long QUALITY_MIDDLE						= PostEffectQualityOption.POST_EFFECT_QUALITY_MIDDLE
																| SceneGraphicQualityOption.SCENE_GRAPHICS_QUALITY_MIDDLE
																| EffectDisplayQualityOption.EFFECT_DISPLAY_QUALITY_MIDDLE
																| TextureQualityOption.TEXTURE_QUALITY_MIDDLE;

		public static long QUALITY_HIGH							= PostEffectQualityOption.POST_EFFECT_QUALITY_HIGH
																| SceneGraphicQualityOption.SCENE_GRAPHICS_QUALITY_HIGH
																| EffectDisplayQualityOption.EFFECT_DISPLAY_QUALITY_HIGH
																| TextureQualityOption.TEXTURE_QUALITY_HIGH;
	}
}