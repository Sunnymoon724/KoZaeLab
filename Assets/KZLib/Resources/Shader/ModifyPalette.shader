Shader "KZLib/ModifyPalette"
{
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
			Name "Universal Forward"
			Tags { "LightMode" = "UniversalForward" }

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

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
				float3 position	: TEXCOORD2;
				float4 vertex	: SV_POSITION;
			};

			TEXTURE2D(_MainTex);
			SAMPLER(sampler_MainTex);

			half4 _PixelColorArray[7];

			v2f vert(appdata_t value)
			{
				v2f output;

				output.texcoord	= value.texcoord;
				output.position	= TransformObjectToWorld(value.vertex.xyz);
				output.normal	= TransformObjectToWorldNormal(value.normal);
				output.vertex	= TransformObjectToHClip(value.vertex.xyz);

				return output;
			}

			half4 frag(v2f input) : SV_Target
			{
				half4 color = SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex,input.texcoord);
				float pixelIndex = floor(input.texcoord.x*256.0);

				int colorIndex = int(pixelIndex)-248;

				if(colorIndex >= 0 && colorIndex < 7)
				{
					half4 paletteColor = _PixelColorArray[colorIndex];

					if(paletteColor.a > 0.0)
					{
						color = paletteColor;
					}
				}

				half3 normal = normalize(input.normal);
				half3 viewDir = normalize(GetCameraPositionWS()-input.position);

				Light mainLight = GetMainLight();

				half3 direction = normalize(mainLight.direction);
				half3 diffuse = saturate(dot(normal,direction))*mainLight.color;

				return half4(color.rgb*diffuse,color.a);
			}
			ENDHLSL
		}
	}
}