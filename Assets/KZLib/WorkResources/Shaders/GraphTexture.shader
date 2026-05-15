Shader "KZLib/GraphTexture"
{
	Properties
	{
		[HideInInspector] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
	}

		SubShader
		{
			Tags
			{ 
				"Queue"				=	"Transparent" 
				"IgnoreProjector"	=	"True" 
				"RenderType"		=	"Transparent" 
				"PreviewType"		=	"Plane"
				"CanUseSpriteAtlas"	=	"True"
			}

			Cull Off
			Lighting Off
			ZWrite Off
			ZTest Off
			Blend One OneMinusSrcAlpha

			Pass
			{
				Name "Default"
				CGPROGRAM

				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile _ PIXELSNAP_ON
				
				#include "UnityCG.cginc"

				struct appdata_t
				{
					float4 vertex    : POSITION;
					float4 color     : COLOR;
					float2 texcoord  : TEXCOORD0;
				};

				struct v2f
				{
					float4 vertex    : SV_POSITION;
					fixed4 color     : COLOR;
					float2 texcoord  : TEXCOORD0;
				};

				fixed4 _Color;

				v2f vert(appdata_t input)
				{
					v2f result;
					result.vertex = UnityObjectToClipPos(input.vertex);
					result.texcoord = input.texcoord;
					result.color = input.color*_Color;

					return result;
				}

				sampler2D _MainTex;

				fixed4 SampleTexture(float2 uv)
				{
					return tex2D(_MainTex,uv);
				}

				uniform float _GraphArray[512];
				uniform float _GraphLength;

				static const float GradientThickness	= 4.0;
				static const float GradientIntensity 	= 0.3;
				static const float FadeRange			= 0.03;

				fixed4 frag(v2f input) : SV_Target
				{
					fixed4 color = input.color;

					float x = input.texcoord.x;
					float y = input.texcoord.y;

					float value = _GraphArray[floor(x*_GraphLength)];

					if(value < 0.0)
					{
						return fixed4(0.0,0.0,0.0,0.0);
					}

					float increment = 1.0/(_GraphLength-1.0);

					if(value > 0.0 && value-y > increment*GradientThickness)
					{
						color.a *= y*GradientIntensity/value;
					}

					if(y > value)
					{
						color.a = 0.0;
					}

					if(x < FadeRange)
					{
						color.a *= 1.0-(FadeRange-x)/FadeRange;
					}
					else if(x > 1.0-FadeRange)
					{
						color.a *= (1.0-x)/FadeRange;
					}

					fixed4 result = SampleTexture(input.texcoord)*color;

					result.rgb *= result.a;

					return result;
				}

				ENDCG
			}
		}
}