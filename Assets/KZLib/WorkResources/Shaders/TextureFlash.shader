Shader "KZLib/TextureFlash"
{
	Properties
	{
		_FlashColor("Flash Color",Color) = (1.0,1.0,1.0,1.0)
		_FlashAmount("Flash Amount",Range(0.0,1.0)) = 0.0

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
					float4 vertex	: SV_POSITION;
					float4 color	: COLOR;
					float2 texcoord	: TEXCOORD0;
				};

				sampler2D _MainTex;
				float4 _FlashColor;
				float _FlashAmount;

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
					float4 texColor = tex2D(_MainTex,input.texcoord)*input.color;

					texColor.rgb = lerp(texColor.rgb,_FlashColor.rgb,_FlashAmount);

					return texColor;
				}
			ENDCG
		}
	}
}