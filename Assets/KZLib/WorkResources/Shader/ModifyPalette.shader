Shader "KZLib/ModifyPalette"
{
	// Voxel/block mesh shader: runtime palette swap + per-face diffuse lighting.
	// No per-block shadow cast/receive; use a separate ground blob/decal if needed later.

	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}

	SubShader
	{
		Tags { "RenderPipeline" = "UniversalRenderPipeline" "RenderType" = "Opaque" }
		LOD 100

		Pass
		{
			// Forward only — intentionally no ShadowCaster pass.
			Name "Universal Forward"
			Tags { "LightMode" = "UniversalForward" }

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/GlobalIllumination.hlsl"

			struct appdata_t
			{
				float4 vertex	: POSITION;
				float2 texcoord	: TEXCOORD0;
				float3 normal	: NORMAL;
			};

			struct v2f
			{
				float2 texcoord	: TEXCOORD0;
				float3 normal	: TEXCOORD1;
				float4 vertex	: SV_POSITION;
			};

			TEXTURE2D(_MainTex);
			SAMPLER(sampler_MainTex);

			// Fixed texture layout: 256px wide, palette key colors at X=248..254 (7 slots).
			// Hardcoded because only project-standard textures are used.
			static const float c_textureWidth = 256.0;
			static const int c_paletteStartIndex = 248;
			static const int c_paletteCount = 7; // Fixed: runtime palette always has 7 entries.

			// _PixelColorArray length must match c_paletteCount. Replaces color only when alpha > 0.
			half4 _PixelColorArray[c_paletteCount];

			v2f vert(appdata_t input)
			{
				v2f output;

				output.texcoord	= input.texcoord;
				output.normal	= TransformObjectToWorldNormal(input.normal);
				output.vertex	= TransformObjectToHClip(input.vertex.xyz);

				return output;
			}

			half4 frag(v2f input) : SV_Target
			{
				half4 color = SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex,input.texcoord);
				// Palette slot from UV.x only (Y ignored).
				float pixelIndex = floor(input.texcoord.x * c_textureWidth);

				int colorIndex = int(pixelIndex) - c_paletteStartIndex;

				if(colorIndex >= 0 && colorIndex < c_paletteCount)
				{
					half4 paletteColor = _PixelColorArray[colorIndex];

					if(paletteColor.a > 0.0)
					{
						color = paletteColor;
					}
				}

				half3 normal = normalize(input.normal);

				// Diffuse + ambient only (SampleSH). No shadow-map sampling on block faces.
				Light mainLight = GetMainLight();

				half3 direction = normalize(mainLight.direction);
				half3 diffuse = saturate(dot(normal,direction))*mainLight.color;
				half3 ambient = SampleSH(normal);
				half3 lighting = diffuse+ambient;

				return half4(color.rgb*lighting,color.a);
			}
			ENDHLSL
		}
	}
}