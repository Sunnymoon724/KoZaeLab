Shader "KZLib/TextureGrayscaleBlur"
{
	Properties
	{
		_BlurSize("Blur Size",Range(0.0,1.0)) = 0.8
		[Space]
		_Saturation("Saturation",Range(0.0,1.0)) = 1.0

		[HideInInspector] _StencilComp("Stencil Comparison",Float) = 8.0
		[HideInInspector] _Stencil("Stencil ID",Float) = 0.0
		[HideInInspector] _StencilOp("Stencil Operation",Float) = 0.0
		[HideInInspector] _StencilWriteMask("Stencil Write Mask",Float) = 255.0
		[HideInInspector] _StencilReadMask("Stencil Read Mask",Float) = 255.0

		[HideInInspector] _ColorMask("Color Mask",Float) = 15.0
		[Space]
		_MainTex("Main Texture (RGBA)",2D) = "white" {}
	}

	SubShader
	{
		Tags
		{
			"Queue"				= "Transparent"
			"IgnoreProjector"	= "True"
			"RenderType"		= "Transparent"
			"PreviewType"		= "Plane"
			"CanUseSpriteAtlas"	= "True"
		}

		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp] 
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}

		ColorMask [_ColorMask]

		Pass
		{
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off

			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"

				struct appdata_t
				{
					float4 vertex	: POSITION;
					float4 color	: COLOR;
					float2 texcoord	: TEXCOORD0;
				};

				struct v2f
				{
					float2 texcoord	: TEXCOORD0;
					float4 color	: COLOR;
					float4 vertex	: SV_POSITION;
				};

				sampler2D _MainTex;
				float _BlurSize;

				float _Saturation;

				static const float c_blurSizeScale = 0.01;
				static const float c_blurRadius = 4.0;
				static const float c_blurSampleCount = 81.0; // (c_blurRadius*2+1)^2 = 9*9
				static const float3 c_luminance = float3(0.299, 0.587, 0.114);

				v2f vert(appdata_t input)
				{
					v2f result;

					result.vertex = UnityObjectToClipPos(input.vertex);
					result.texcoord = input.texcoord;
					result.color = input.color;

					return result;
				}

				float4 frag(v2f input) : SV_Target
				{
					float4 texColor = tex2D(_MainTex, input.texcoord) * input.color;

					if(_BlurSize > 0.0)
					{
						float4 blur = 0.0;
						float size = _BlurSize * c_blurSizeScale;

						for(float i=-c_blurRadius;i<=c_blurRadius;i+=1.0)
						{
							for(float j=-c_blurRadius;j<=c_blurRadius;j+=1.0)
							{
								float2 offset = float2(i*size,j*size);
								blur += tex2D(_MainTex, input.texcoord+offset)*input.color;
							}
						}

						texColor = lerp(texColor, blur/c_blurSampleCount, _BlurSize);
					}

					if(_Saturation > 0.0)
					{
						texColor.rgb = lerp(texColor.rgb,dot(texColor.rgb,c_luminance),_Saturation);
					}

					return texColor;
				}
			ENDCG
		}
	}
}