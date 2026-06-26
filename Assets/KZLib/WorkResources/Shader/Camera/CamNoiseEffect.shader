Shader "KZLib/Cam/NoiseEffect"
{
	// URP fullscreen blit: chromatic aberration (RGB channel UV offset) + noise grain from _Noise alpha.
	// Typical use: damage / glitch / low-HP screen feedback (_Intensity animated 0 → target → 0).
	//
	// [TODO] Renderer hookup not wired yet — connect later via one of:
	//   - URP Renderer Feature > Full Screen Pass Renderer Feature (material using this shader)
	//   - Custom ScriptableRenderPass + Blitter
	//
	// Setup when hooking up:
	//   1. Assign a tiling noise texture to _Noise (alpha channel drives grain).
	//   2. Blit source must be bound as _BlitTexture (Blit.hlsl / FullScreenPass path).
	//   3. No depth texture required. URP Volume Chromatic Aberration + Film Grain can substitute if custom tuning is not needed.
	Properties
	{
		_Intensity("Intensity",Range(0.0,1.0)) = 0.5
		_Noise("Noise",2D) = "white" {}
	}

	SubShader
	{
		Tags
		{
			"RenderPipeline" = "UniversalPipeline"
			"RenderType" = "Opaque"
		}

		Pass
		{
			Name "NoiseEffect"

			ZTest Always
			ZWrite Off
			Cull Off

			HLSLPROGRAM
			#pragma vertex Vert
			#pragma fragment Frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

			CBUFFER_START(UnityPerMaterial)
				float _Intensity;
			CBUFFER_END

			TEXTURE2D(_Noise);
			SAMPLER(sampler_Noise);

			half4 Frag(Varyings input) : SV_Target
			{
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

				float2 uv = input.texcoord;
				float2 center = float2(0.5,0.5);
				float2 redUV = uv;
				float2 greenUV = lerp(uv,center,0.05 * _Intensity);
				float2 blueUV = lerp(uv,center,0.1 * _Intensity);

				half4 result = half4(
					SAMPLE_TEXTURE2D_X(_BlitTexture,sampler_LinearClamp,redUV).r,
					SAMPLE_TEXTURE2D_X(_BlitTexture,sampler_LinearClamp,greenUV).g,
					SAMPLE_TEXTURE2D_X(_BlitTexture,sampler_LinearClamp,blueUV).b,
					1.0h);

				result.rgb += half3(
					SAMPLE_TEXTURE2D(_Noise,sampler_Noise,redUV).a * _Intensity,
					SAMPLE_TEXTURE2D(_Noise,sampler_Noise,greenUV).a * _Intensity,
					SAMPLE_TEXTURE2D(_Noise,sampler_Noise,blueUV).a * _Intensity);

				return result;
			}
			ENDHLSL
		}
	}

	Fallback Off
}