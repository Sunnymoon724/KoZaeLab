Shader "KZLib/GraphTexture"
{
	// UI graph fill shader. UV.x maps to sample index, UV.y is normalized height (0 = bottom).
	// GraphImage pushes heights into _GraphArray; fragments below the curve stay visible.

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
			}

			Cull Off
			Lighting Off
			ZWrite Off
			ZTest [unity_GUIZTestMode]
			Blend One OneMinusSrcAlpha // Premultiplied alpha; frag multiplies rgb by alpha

			Pass
			{
				Name "Default"
				CGPROGRAM

				#pragma vertex vert
				#pragma fragment frag
				
				#include "UnityCG.cginc"

				// Must match GraphImage.MaxGraphLength
				#define GRAPH_ARRAY_MAX 512

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

				// Heights per column, uploaded by GraphImage.SetFloatArray
				uniform float _GraphArray[GRAPH_ARRAY_MAX];
				uniform float _GraphLength;

				// Gradient band depth (in sample steps), strength, and horizontal edge fade width
				static const float GradientThickness	= 4.0;
				static const float GradientIntensity 	= 0.3;
				static const float FadeRange			= 0.03;

				fixed4 frag(v2f input) : SV_Target
				{
					fixed4 color = input.color;

					float x = input.texcoord.x;
					float y = input.texcoord.y;

					// increment uses (_GraphLength - 1); need at least two samples
					if(_GraphLength < 2.0)
					{
						return fixed4(0.0,0.0,0.0,0.0);
					}

					// Clamp index so x == 1 does not read past the last element
					int lastIndex = (int)_GraphLength-1;
					int index = min((int)floor(x*_GraphLength),lastIndex);
					float value = _GraphArray[index];

					// Negative values are sentinels (e.g. AudioGraphImage gap markers)
					if(value < 0.0)
					{
						return fixed4(0.0,0.0,0.0,0.0);
					}

					float increment = 1.0/(_GraphLength-1.0);

					// Darken the fill slightly below the curve (nearest-neighbor columns)
					if(value > 0.0 && value-y > increment*GradientThickness)
					{
						color.a *= y*GradientIntensity/value;
					}

					// Hide pixels above the curve; keep the area under the graph
					if(y > value)
					{
						color.a = 0.0;
					}

					// Soft fade at left/right edges
					if(x < FadeRange)
					{
						color.a *= 1.0-(FadeRange-x)/FadeRange;
					}
					else if(x > 1.0-FadeRange)
					{
						color.a *= (1.0-x)/FadeRange;
					}

					fixed4 result = SampleTexture(input.texcoord)*color;

					result.rgb *= result.a; // Premultiply for Blend One OneMinusSrcAlpha

					return result;
				}

				ENDCG
			}
		}
}