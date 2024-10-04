using System;
using System.Collections.Generic;

namespace KZLib
{
	[Flags]
	public enum GraphicQualityType : long
	{
		Invalid							= 0L,

		// Scene
		SceneLowFlag					= 1L << 0,
		CameraFarHalf					= 1L << 1,
		UnitShadowEnable				= 1L << 2,

		// Effect
		EffectLowFlag					= 1L << 10,

		// Texture
		TextureLowFlag					= 1L << 20,
		AnisotropicFilteringEnable		= 1L << 21,
		VerticalSyncEnable				= 1L << 22,
		AntiAliasingEnable				= 1L << 23,
	}

	public enum SceneQualityType : long
	{
		SceneQualityLowest				= 0L,
		SceneQualityLow					= GraphicQualityType.SceneLowFlag
										| GraphicQualityType.CameraFarHalf,
		SceneQualityMiddle				= GraphicQualityType.UnitShadowEnable | SceneQualityLow,
		SceneQualityHigh				= SceneQualityMiddle,
		SceneQualityHighest				= SceneQualityHigh,

		SceneBitOverwriteMask			= ~(SceneQualityHighest | GraphicQualityType.SceneLowFlag),
	}

	public enum EffectQualityType : long
	{
		EffectQualityLowest				= 0L,
		EffectQualityLow				= GraphicQualityType.EffectLowFlag,
		EffectQualityMiddle				= EffectQualityLow,
		EffectQualityHigh				= EffectQualityMiddle,
		EffectQualityHighest			= EffectQualityHigh,

		EffectBitOverwriteMask			= ~(EffectQualityHighest | GraphicQualityType.EffectLowFlag),
	}

	public enum TextureQualityType : long
	{
		TextureQualityLowest			= 0L,
		TextureQualityLow				= GraphicQualityType.TextureLowFlag,
		TextureQualityMiddle			= GraphicQualityType.AnisotropicFilteringEnable | TextureQualityLow,
		TextureQualityHigh				= GraphicQualityType.VerticalSyncEnable | TextureQualityMiddle,
		TextureQualityHighest			= TextureQualityHigh,

		TextureBitOverwriteMask			= ~(TextureQualityHighest | GraphicQualityType.TextureLowFlag),
	}

	// public struct TextureQualityOption
	// {
	// 	public static long TextureQualityLowest		= 0L;

	// 	public static long TextureQualityLow		= TextureQualityLowest;

	// 	public static long TextureQualityMiddle		= TextureQualityLow;

	// 	public static long TextureQualityHigh		= TextureQualityMiddle;

	// 	public static long TextureQualityHighest	= TextureQualityHigh;

	// 	public static long TextureBitOverwriteMask	= ~(TEXTURE_QUALITY_HIGHEST | EGraphicsQualityOption.TEXTURE_LOW_FLAG);
	// }

	// public enum GraphicsQualityPresetType : long
	// {
	// 	QualityLowest = SceneQualityType.SceneQualityLowest | EffectQualityType.EffectQualityLowest | TextureQualityType.TextureQualityLowest,
	// 	QualityLow = SceneQualityType.SceneQualityLow | EffectQualityType.EffectQualityLow | TextureQualityType.TextureQualityLow,
	// 	QualityMiddle = SceneQualityType.SceneQualityMiddle | EffectQualityType.EffectQualityMiddle | TextureQualityType.TextureQualityMiddle,
	// 	QualityHigh = SceneQualityType.SceneQualityHigh | EffectQualityType.EffectQualityHigh | TextureQualityType.TextureQualityHigh,
	// 	QualityHighest = SceneQualityType.SceneQualityHighest | EffectQualityType.EffectQualityHighest | TextureQualityType.TextureQualityHighest,
	// }

	// public struct GraphicsQualityPreset
	// {
	// 	public static long QualityLowest = EPostEffectQualityOption.POST_EFFECT_QUALITY_LOWEST
	// 					| ESceneGraphicsQualityOption.SCENE_GRAPHICS_QUALITY_LOWEST
	// 					| EEffectDisplayQualityOption.EFFECT_DISPLAY_QUALITY_LOWEST
	// 					| TextureQualityOption.TextureQualityLowest
	// 					| EGraphicsQualityOption.FIXED_SCREEN_RESOLUTION_1080;

	// 	public static long QualityLow = EPostEffectQualityOption.POST_EFFECT_QUALITY_LOW
	// 					| ESceneGraphicsQualityOption.SCENE_GRAPHICS_QUALITY_LOW
	// 					| EEffectDisplayQualityOption.EFFECT_DISPLAY_QUALITY_LOW
	// 					| ETextureQualityOption.TEXTURE_QUALITY_LOW
	// 					| EGraphicsQualityOption.FIXED_SCREEN_RESOLUTION_1280;

	// 	public static long QualityMiddle = EPostEffectQualityOption.POST_EFFECT_QUALITY_MIDDLE
	// 					| ESceneGraphicsQualityOption.SCENE_GRAPHICS_QUALITY_MIDDLE
	// 					| EEffectDisplayQualityOption.EFFECT_DISPLAY_QUALITY_MIDDLE
	// 					| ETextureQualityOption.TEXTURE_QUALITY_MIDDLE
	// 					| EGraphicsQualityOption.FIXED_SCREEN_RESOLUTION_1600;

	// 	public static long QualityHigh = EPostEffectQualityOption.POST_EFFECT_QUALITY_HIGH
	// 					| ESceneGraphicsQualityOption.SCENE_GRAPHICS_QUALITY_HIGH
	// 					| EEffectDisplayQualityOption.EFFECT_DISPLAY_QUALITY_HIGH
	// 					| ETextureQualityOption.TEXTURE_QUALITY_HIGH
	// 					| EGraphicsQualityOption.FIXED_SCREEN_RESOLUTION_1920;

	// 	public static long QualityHighest = EPostEffectQualityOption.POST_EFFECT_QUALITY_HIGHEST
	// 					| ESceneGraphicsQualityOption.SCENE_GRAPHICS_QUALITY_HIGHEST
	// 					| EEffectDisplayQualityOption.EFFECT_DISPLAY_QUALITY_HIGHEST
	// 					| ETextureQualityOption.TEXTURE_QUALITY_HIGHEST
	// 					| EGraphicsQualityOption.FULL_SCREEN_RESOLUTION;

	// 	// public static bool ChkPresetOver(int limitPreset, long lGraphicOption)
	// 	// {
	// 	// 	switch (limitPreset)
	// 	// 	{
	// 	// 		case 1:
	// 	// 			if (IsFlagAny(QUALITY_LOW, lGraphicOption) || IsFlagAny(QUALITY_MIDDLE, lGraphicOption) || IsFlagAny(QUALITY_HIGH, lGraphicOption) ||
	// 	// 				IsFlagAny(QUALITY_HIGHEST, lGraphicOption))
	// 	// 				return true;
	// 	// 			break;
	// 	// 		case 2:
	// 	// 			if (IsFlagAny(QUALITY_MIDDLE, lGraphicOption) || IsFlagAny(QUALITY_HIGH, lGraphicOption) ||
	// 	// 				IsFlagAny(QUALITY_HIGHEST, lGraphicOption))
	// 	// 				return true;
	// 	// 			break;
	// 	// 		case 3:
	// 	// 			if (IsFlagAny(QUALITY_HIGH, lGraphicOption) ||
	// 	// 				IsFlagAny(QUALITY_HIGHEST, lGraphicOption))
	// 	// 				return true;
	// 	// 			break;
	// 	// 		case 4:
	// 	// 			if (IsFlagAny(QUALITY_HIGHEST, lGraphicOption))
	// 	// 				return true;
	// 	// 			break;
	// 	// 	}

	// 	// 	return false;
	// 	// }

	// 	// private static bool IsFlagAny(long targetOption, long lGraphicOption)
	// 	// {
	// 	// 	return (targetOption & lGraphicOption) != 0;
	// 	// }
	// };
}