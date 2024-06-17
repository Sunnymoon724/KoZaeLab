namespace KZLib
{
	public static class GraphicQualityOption
	{
		public static long INVALID								= 0L;
		public static long FLAG_BEGIN							= 1L;

		public static long BLOOM_ENABLE							= FLAG_BEGIN << 1;
		public static long SCREEN_DISTORTION_ENABLE				= FLAG_BEGIN << 2;
		public static long LENS_FLARE_ENABLE					= FLAG_BEGIN << 3;
		public static long RADIAL_BLUR_ENABLE					= FLAG_BEGIN << 4;
		public static long MOTION_BLUR_ENABLE					= FLAG_BEGIN << 5;


		public static long UNIT_PROJECTION_SHADOW_ENABLE		= FLAG_BEGIN << 10;
		public static long FOLIAGE_ANIMATION_ENABLE				= FLAG_BEGIN << 11;
		public static long WORLD_PROP_ALPHA_BLEND_ENABLE		= FLAG_BEGIN << 12;
		public static long WORLD_PROP_FADE_IN_OUT_ENABLE		= FLAG_BEGIN << 13;
		public static long LIGHT_MAP_COLOR_BLEND_TO_UNIT_ENABLE = FLAG_BEGIN << 14;


		public static long ANISOTROPIC_FILTERING_ENABLE			= FLAG_BEGIN << 19;
		public static long TEXTURE_QUALITY_FULL					= FLAG_BEGIN << 20;
		public static long VERTICAL_SYNC_ENABLE					= FLAG_BEGIN << 21;

		public static long DISPLAY_HIT_EFFECT					= FLAG_BEGIN << 25;
		public static long DISPLAY_PLAYER_SUB_EFFECT			= FLAG_BEGIN << 26;
		

		public static long FLAG_END								= FLAG_BEGIN << 31;
	}

	public static class PostEffectQualityOption
	{
		public static long POST_EFFECT_QUALITY_LOW				= 0L;

		public static long POST_EFFECT_QUALITY_MIDDLE			= GraphicQualityOption.BLOOM_ENABLE
																| GraphicQualityOption.SCREEN_DISTORTION_ENABLE
																| GraphicQualityOption.LENS_FLARE_ENABLE;

		public static long POST_EFFECT_QUALITY_HIGH				= GraphicQualityOption.BLOOM_ENABLE
																| GraphicQualityOption.SCREEN_DISTORTION_ENABLE
																| GraphicQualityOption.LENS_FLARE_ENABLE
																| GraphicQualityOption.RADIAL_BLUR_ENABLE
																| GraphicQualityOption.MOTION_BLUR_ENABLE;
	}

	public static class SceneGraphicQualityOption
	{
		public static long SCENE_GRAPHICS_QUALITY_LOW			= 0L;

		public static long SCENE_GRAPHICS_QUALITY_MIDDLE		= GraphicQualityOption.UNIT_PROJECTION_SHADOW_ENABLE
																| GraphicQualityOption.FOLIAGE_ANIMATION_ENABLE;

		public static long SCENE_GRAPHICS_QUALITY_HIGH			= GraphicQualityOption.UNIT_PROJECTION_SHADOW_ENABLE
																| GraphicQualityOption.FOLIAGE_ANIMATION_ENABLE
																| GraphicQualityOption.WORLD_PROP_ALPHA_BLEND_ENABLE
																| GraphicQualityOption.WORLD_PROP_FADE_IN_OUT_ENABLE
																| GraphicQualityOption.LIGHT_MAP_COLOR_BLEND_TO_UNIT_ENABLE;
	}

	public class TextureQualityOption
	{
		public static long TEXTURE_QUALITY_LOW					= 0L;

		public static long TEXTURE_QUALITY_MIDDLE				= GraphicQualityOption.ANISOTROPIC_FILTERING_ENABLE;

		public static long TEXTURE_QUALITY_HIGH					= GraphicQualityOption.ANISOTROPIC_FILTERING_ENABLE
																| GraphicQualityOption.VERTICAL_SYNC_ENABLE
																| GraphicQualityOption.TEXTURE_QUALITY_FULL;
	}

	public static class EffectDisplayQualityOption
	{
		public static long EFFECT_DISPLAY_QUALITY_LOW			= 0L;

		public static long EFFECT_DISPLAY_QUALITY_MIDDLE		= GraphicQualityOption.DISPLAY_HIT_EFFECT;

		public static long EFFECT_DISPLAY_QUALITY_HIGH			= GraphicQualityOption.DISPLAY_PLAYER_SUB_EFFECT
																| GraphicQualityOption.DISPLAY_HIT_EFFECT;
	}
}