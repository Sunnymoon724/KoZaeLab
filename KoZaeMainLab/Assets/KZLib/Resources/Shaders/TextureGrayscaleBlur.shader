// 블러 + 그레이 스케일 해주는 쉐이더
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
					fixed4 color	: COLOR;
					float4 vertex	: SV_POSITION;
				};

				sampler2D _MainTex;
				float _BlurSize;

				float _Saturation;

				v2f vert(appdata_t _data)
				{
					v2f result;

					result.vertex = UnityObjectToClipPos(_data.vertex);
					result.texcoord = _data.texcoord;
					result.color = _data.color;

					return result;
				}

				fixed4 frag(v2f _data) : SV_Target
				{
					fixed4 texColor = tex2D(_MainTex,_data.texcoord)*_data.color;

					if(_BlurSize != 0.0)
					{
						fixed4 blur = 0.0;
						float size = _BlurSize*0.01;

						for(float i=-4.0;i<=4.0;i+=1.0)
						{
							for(float j=-4.0;j<=4.0;j+=1.0)
							{
								float2 offset = float2(i*size,j*size);
								blur += tex2D(_MainTex,_data.texcoord+offset)*_data.color;
							}
						}

						texColor = lerp(texColor,blur/81.0,_BlurSize);
					}

					if(_Saturation != 0.0)
					{
						texColor.rgb = lerp(texColor.rgb,dot(texColor.rgb,float3(0.229,0.587,0.114)),_Saturation);
					}

					return texColor;
				}
			ENDCG
		}
	}
}