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

				v2f vert(appdata_t _data)
				{
					v2f result;
					result.vertex = UnityObjectToClipPos(_data.vertex);
					result.texcoord = _data.texcoord;
					result.color = _data.color*_Color;

					return result;
				}

				sampler2D _MainTex;

				fixed4 SampleTexture(float2 _data)
				{
					return tex2D(_MainTex,_data);
				}

				uniform float _GraphArray[512];
				uniform float _GraphLength;

				fixed4 frag(v2f _data) : SV_Target
				{
					fixed4 color = _data.color;

					fixed x = _data.texcoord.x;
					fixed y = _data.texcoord.y;

					float value = _GraphArray[floor(x*_GraphLength)];
					float increment = 1.0/(_GraphLength-1.0);

					if(value-y > increment*4.0)
					{
						color.a *= y*0.3/value;
					}
					//else
					//{
					//	color.a = 0;
					//}

					if(y > value)
					{
						color.a = 0.0;
					}

					if(x < 0.03)
					{
						color.a *= 1.0-(0.03-x)/0.03;
					}
					else if(x>0.97)
					{
						color.a *= (1.0-x)/0.03;
					}

					fixed4 result = SampleTexture(_data.texcoord)*color;

					result.rgb *= result.a;

					return result;
				}

				ENDCG
			}
		}
}