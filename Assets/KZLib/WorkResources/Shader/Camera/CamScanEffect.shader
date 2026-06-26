Shader "KZLib/Cam/ScanEffect"
{
	// URP fullscreen blit: depth-ring scan pulse (sonar / radar style).
	// Typical use: animate _ScanDistance over time for an expanding ring highlight.
	//
	// [TODO] Renderer hookup not wired yet — connect later via one of:
	//   - URP Renderer Feature > Full Screen Pass Renderer Feature (material using this shader)
	//   - Custom ScriptableRenderPass + Blitter
	//
	// Setup when hooking up:
	//   1. Enable Depth Texture on URP Asset or the target camera (required for SampleSceneDepth).
	//   2. Blit source must be bound as _BlitTexture (Blit.hlsl / FullScreenPass path).
	//   3. Drive _ScanDistance from script for the pulse; tune _ScanWidth and _ScanColor on the material.
	Properties
	{
		_ScanDistance("Scan Distance",Range(0.0,1.0)) = 0.5
		_ScanWidth("Scan Width",Range(0.0,1.0)) = 0.05
		_ScanColor("Scan Color",Color) = (0.0,1.0,1.0,1.0)
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
			Name "ScanEffect"

			ZTest Always
			ZWrite Off
			Cull Off

			HLSLPROGRAM
			#pragma vertex Vert
			#pragma fragment Frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderVariablesFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

			CBUFFER_START(UnityPerMaterial)
				float _ScanDistance;
				float _ScanWidth;
				half4 _ScanColor;
			CBUFFER_END

			half4 Frag(Varyings input) : SV_Target
			{
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

				float2 uv = input.texcoord;
				half4 color = SAMPLE_TEXTURE2D_X(_BlitTexture,sampler_LinearClamp,uv);
				float depth = Linear01Depth(SampleSceneDepth(uv),_ZBufferParams);

				if(depth < _ScanDistance && depth > _ScanDistance - _ScanWidth && depth < 1.0)
				{
					half4 scanColor = _ScanColor;
					scanColor *= 1.0h - (_ScanDistance - depth) / _ScanWidth;

					return color + scanColor;
				}

				return color;
			}
			ENDHLSL
		}
	}

	Fallback Off
}